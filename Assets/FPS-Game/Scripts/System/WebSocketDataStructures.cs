using System;
using UnityEngine;

/// <summary>
/// Data structures for WebSocket communication between Unity and OpenClaw agents
/// </summary>

// ==========================================
// INBOUND: Commands from OpenClaw to Unity
// ==========================================

[Serializable]
public class AgentCommand
{
    /// <summary>
    /// Command type: MOVE, LOOK, SHOOT, JUMP, RELOAD, STOP, SWITCH_WEAPON
    /// </summary>
    public string commandType;
    
    /// <summary>
    /// Command-specific data
    /// </summary>
    public CommandData data;
    
    /// <summary>
    /// Agent identifier (for multi-agent scenarios)
    /// </summary>
    public string agentId;
    
    /// <summary>
    /// Command timestamp (for synchronization)
    /// </summary>
    public double timestamp;
}

[Serializable]
public class CommandData
{
    // For MOVE command
    public float x;  // Movement direction X
    public float y;  // Movement direction Y
    public float z;  // Movement direction Z
    
    // For LOOK command
    public float pitch;  // Camera pitch (X rotation)
    public float yaw;    // Camera yaw (Y rotation)
    public float roll;   // Camera roll (Z rotation, usually 0)
    
    // For SHOOT, WAIT commands
    public float duration;  // How long to execute (seconds)
    
    // For SWITCH_WEAPON command
    public float weaponIndex;  // Which weapon to switch to
    
    /// <summary>
    /// Helper property to get direction vector
    /// </summary>
    public Vector3 direction
    {
        get => new Vector3(x, y, z);
        set { x = value.x; y = value.y; z = value.z; }
    }
    
    /// <summary>
    /// Helper property to get euler angles
    /// </summary>
    public Vector3 euler
    {
        get => new Vector3(pitch, yaw, roll);
        set { pitch = value.x; yaw = value.y; roll = value.z; }
    }
}

// ==========================================
// OUTBOUND: Game state from Unity to OpenClaw
// ==========================================

[Serializable]
public class GameStateSnapshot
{
    /// <summary>
    /// Unity Time.time when snapshot was captured
    /// </summary>
    public float timestamp;
    
    /// <summary>
    /// Unity frame count
    /// </summary>
    public int frameCount;
    
    /// <summary>
    /// Local player state
    /// </summary>
    public PlayerState player;
    
    /// <summary>
    /// Array of enemy/bot states
    /// </summary>
    public EnemyState[] enemies;
    
    /// <summary>
    /// General game information
    /// </summary>
    public GameInfo gameInfo;
}

[Serializable]
public class PlayerState
{
    // Position and movement
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 velocity;
    
    // Health
    public float health;
    public float maxHealth;
    
    // Ammo
    public int currentAmmo;
    public int maxAmmo;
    public bool isReloading;
    
    // Stats
    public int kills;
    public int deaths;
    
    // Weapon
    public string currentWeapon;
    
    // State
    public bool isGrounded;
    public string movementState;  // "Idle", "Walking", "Running", "Airborne"
}

[Serializable]
public class EnemyState
{
    public string id;
    public Vector3 position;
    public Vector3 rotation;
    public float health;
    public float distance;  // Distance from player
    public bool isVisible;  // Line of sight check
    public bool isAlive;
    public Vector3 lastSeenPosition;
}

[Serializable]
public class GameInfo
{
    public string gameMode;
    public float matchTime;
    public float maxMatchTime;
    public bool isGameActive;
    public int killLimit;
    public string currentMap;
    public ZoneInfo zoneInfo;
}

[Serializable]
public class ZoneInfo
{
    public string currentZone;
    public string[] nearbyZones;
    public bool zoneFullyScanned;
}
