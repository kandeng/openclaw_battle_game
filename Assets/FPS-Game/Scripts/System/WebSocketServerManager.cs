using System;
using System.Collections.Generic;
using UnityEngine;

// Note: These namespaces require websocket-sharp library
// Install via Unity Package Manager: https://github.com/sta/websocket-sharp.git
using WebSocketSharp;
using WebSocketSharp.Server;

/// <summary>
/// Manages WebSocket server for OpenClaw agent integration
/// Handles bi-directional communication: receives commands, broadcasts game state
/// 
/// REQUIRES: websocket-sharp library
/// Installation: See WebSocket/README_WEBSOCKET_INSTALLATION.md
/// </summary>
public class WebSocketServerManager : MonoBehaviour
{
    public static WebSocketServerManager Instance { get; private set; }
    
    [Header("Server Configuration")]
    [SerializeField] int port = 8080;
    [SerializeField] string endpoint = "/agent";
    
    [Header("Broadcast Settings")]
    [SerializeField] float broadcastInterval = 0.1f; // 10 Hz (100ms)
    [SerializeField] bool autoStart = true;
    
    private WebSocketServer server;
    private float broadcastTimer;
    private List<AgentSession> activeSessions = new();
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    void Start()
    {
        if (autoStart)
        {
            Initialize();
        }
    }
    
    void Update()
    {
        // Broadcast game state at fixed interval
        broadcastTimer += Time.deltaTime;
        if (broadcastTimer >= broadcastInterval)
        {
            broadcastTimer = 0;
            BroadcastGameState();
        }
    }
    
    void OnDestroy()
    {
        Stop();
    }
    
    /// <summary>
    /// Initialize and start WebSocket server
    /// </summary>
    public void Initialize()
    {
        try
        {
            string url = $"ws://0.0.0.0:{port}";
            server = new WebSocketServer(url);
            
            // Add WebSocket endpoint
            server.AddWebSocketService<AgentWebSocketHandler>(endpoint, () =>
            {
                var handler = new AgentWebSocketHandler();
                handler.OnAgentConnected += HandleAgentConnected;
                handler.OnAgentDisconnected += HandleAgentDisconnected;
                handler.OnCommandReceived += HandleCommandReceived;
                return handler;
            });
            
            server.Start();
            Debug.Log($"[WebSocketServer] Server started on {url}{endpoint}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[WebSocketServer] Failed to start server: {ex.Message}");
            Debug.LogError("[WebSocketServer] Make sure websocket-sharp library is installed!");
        }
    }
    
    /// <summary>
    /// Stop WebSocket server
    /// </summary>
    public void Stop()
    {
        if (server != null && server.IsListening)
        {
            server.Stop();
            Debug.Log("[WebSocketServer] Server stopped");
        }
    }
    
    /// <summary>
    /// Handle new agent connection
    /// </summary>
    void HandleAgentConnected(string sessionId)
    {
        var session = new AgentSession
        {
            Id = sessionId,
            ConnectedAt = Time.time,
            LastCommandTime = Time.time
        };
        
        activeSessions.Add(session);
        Debug.Log($"[WebSocketServer] Agent connected: {sessionId} (Total: {activeSessions.Count})");
    }
    
    /// <summary>
    /// Handle agent disconnection
    /// </summary>
    void HandleAgentDisconnected(string sessionId)
    {
        activeSessions.RemoveAll(s => s.Id == sessionId);
        Debug.Log($"[WebSocketServer] Agent disconnected: {sessionId} (Total: {activeSessions.Count})");
    }
    
    /// <summary>
    /// Handle incoming command from agent
    /// </summary>
    void HandleCommandReceived(string sessionId, string commandJson)
    {
        try
        {
            // Parse command
            AgentCommand command = JsonUtility.FromJson<AgentCommand>(commandJson);
            
            // Update session tracking
            var session = activeSessions.Find(s => s.Id == sessionId);
            if (session != null)
            {
                session.LastCommandTime = Time.time;
                session.CommandsReceived++;
            }
            
            // Route command to appropriate controller
            CommandRouter.Execute(command, sessionId);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[WebSocketServer] Error processing command: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Broadcast game state to all connected agents
    /// </summary>
    void BroadcastGameState()
    {
        if (activeSessions.Count == 0 || server == null) return;
        
        try
        {
            // Capture game state snapshot
            GameStateSnapshot snapshot = CaptureGameState();
            
            // Serialize to JSON
            string json = JsonUtility.ToJson(snapshot);
            
            // Broadcast to all connected agents
            server.WebServices[endpoint].Sessions.Broadcast(json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[WebSocketServer] Error broadcasting state: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Capture current game state for broadcasting
    /// </summary>
    GameStateSnapshot CaptureGameState()
    {
        var snapshot = new GameStateSnapshot
        {
            timestamp = Time.time,
            frameCount = Time.frameCount,
            player = CapturePlayerState(),
            enemies = CaptureEnemyStates(),
            gameInfo = CaptureGameInfo()
        };
        
        return snapshot;
    }
    
    /// <summary>
    /// Capture local player state
    /// </summary>
    PlayerState CapturePlayerState()
    {
        PlayerRoot player = FindObjectOfType<PlayerRoot>();
        if (player == null) return null;
        
        return new PlayerState
        {
            position = player.transform.position,
            rotation = player.transform.eulerAngles,
            velocity = player.CharacterController.velocity,
            health = player.Health?.Value ?? 100,
            maxHealth = player.Health?.MaxValue ?? 100,
            currentAmmo = player.PlayerInventory?.GetCurrentAmmo() ?? 0,
            maxAmmo = player.PlayerInventory?.GetMaxAmmo() ?? 30,
            isReloading = player.PlayerReload?.IsReloading ?? false,
            kills = 0,  // Get from scoreboard or game manager
            deaths = 0,
            currentWeapon = player.PlayerInventory?.GetCurrentWeaponName() ?? "Unknown",
            isGrounded = player.PlayerController.Grounded,
            movementState = GetMovementState(player)
        };
    }
    
    /// <summary>
    /// Capture all enemy/bot states
    /// </summary>
    EnemyState[] CaptureEnemyStates()
    {
        PlayerRoot[] allPlayers = FindObjectsByType<PlayerRoot>(FindObjectsSortMode.None);
        PlayerRoot localPlayer = FindObjectOfType<PlayerRoot>();
        
        List<EnemyState> enemies = new();
        
        foreach (PlayerRoot player in allPlayers)
        {
            // Skip local player
            if (player == localPlayer) continue;
            
            // Only include bots/enemies (skip other human players in WebSocket mode)
            if (!player.IsCharacterBot()) continue;
            
            // Calculate distance and visibility
            float distance = Vector3.Distance(localPlayer.transform.position, player.transform.position);
            bool isVisible = CheckLineOfSight(localPlayer.transform, player.transform);
            
            enemies.Add(new EnemyState
            {
                id = player.GetBotID() ?? player.gameObject.name,
                position = player.transform.position,
                rotation = player.transform.eulerAngles,
                health = player.Health?.Value ?? 100,
                distance = distance,
                isVisible = isVisible,
                isAlive = player.Health?.Value > 0 ?? false,
                lastSeenPosition = isVisible ? player.transform.position : player.transform.position
            });
        }
        
        return enemies.ToArray();
    }
    
    /// <summary>
    /// Capture general game information
    /// </summary>
    GameInfo CaptureGameInfo()
    {
        TimePhaseCounter timeCounter = FindObjectOfType<TimePhaseCounter>();
        KillCountChecker killChecker = FindObjectOfType<KillCountChecker>();
        
        return new GameInfo
        {
            gameMode = "WebSocketAgent",
            matchTime = timeCounter?.CurrentTime ?? 0,
            maxMatchTime = timeCounter?.MaxTime ?? 600,
            isGameActive = !InGameManager.Instance.IsGameEnd,
            killLimit = killChecker?.KillLimit ?? 20,
            currentMap = "Italy",
            zoneInfo = CaptureZoneInfo()
        };
    }
    
    /// <summary>
    /// Capture current zone information for AI reasoning
    /// </summary>
    ZoneInfo CaptureZoneInfo()
    {
        PlayerRoot player = FindObjectOfType<PlayerRoot>();
        if (player == null || player.CurrentZoneData == null) return null;
        
        return new ZoneInfo
        {
            currentZone = player.CurrentZoneData.zoneID.ToString(),
            nearbyZones = GetNearbyZoneIds(player.CurrentZoneData),
            zoneFullyScanned = player.CurrentZoneData.isChecked
        };
    }
    
    /// <summary>
    /// Check line of sight between two transforms
    /// </summary>
    bool CheckLineOfSight(Transform from, Transform to)
    {
        Vector3 direction = to.position - from.position;
        float distance = direction.magnitude;
        
        if (Physics.Raycast(from.position, direction.normalized, out RaycastHit hit, distance))
        {
            // Check if hit the target
            return hit.transform.root == to.root;
        }
        
        return false;
    }
    
    /// <summary>
    /// Get player movement state
    /// </summary>
    string GetMovementState(PlayerRoot player)
    {
        if (!player.PlayerController.Grounded) return "Airborne";
        
        Vector3 velocity = player.CharacterController.velocity;
        float speed = new Vector3(velocity.x, 0, velocity.z).magnitude;
        
        if (speed < 0.1f) return "Idle";
        if (speed < 3.0f) return "Walking";
        return "Running";
    }
    
    /// <summary>
    /// Get nearby zone IDs
    /// </summary>
    string[] GetNearbyZoneIds(ZoneData currentZone)
    {
        if (currentZone == null) return new string[0];
        
        List<string> nearbyZones = new();
        foreach (PortalPoint portal in currentZone.portals)
        {
            if (portal.zoneDataA.zoneID != currentZone.zoneID)
            {
                nearbyZones.Add(portal.zoneDataA.zoneID.ToString());
            }
            if (portal.zoneDataB.zoneID != currentZone.zoneID)
            {
                nearbyZones.Add(portal.zoneDataB.zoneID.ToString());
            }
        }
        
        return nearbyZones.ToArray();
    }
}

/// <summary>
/// Represents an active agent session
/// </summary>
[Serializable]
public class AgentSession
{
    public string Id;
    public float ConnectedAt;
    public float LastCommandTime;
    public int CommandsReceived;
}
