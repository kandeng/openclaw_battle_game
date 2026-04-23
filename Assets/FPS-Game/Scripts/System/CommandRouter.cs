using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Routes incoming WebSocket commands to appropriate game controllers
/// Translates high-level commands into PlayerController input
/// </summary>
public static class CommandRouter
{
    /// <summary>
    /// Execute command received from OpenClaw agent
    /// </summary>
    public static void Execute(AgentCommand command, string agentId)
    {
        // Find player controller for this agent
        PlayerRoot player = GetPlayerForAgent(agentId);
        if (player == null)
        {
            Debug.LogWarning($"[CommandRouter] No player found for agent: {agentId}");
            return;
        }
        
        // Validate command
        if (!ValidateCommand(command))
        {
            Debug.LogWarning($"[CommandRouter] Invalid command: {command.commandType}");
            return;
        }
        
        // Route command based on type
        switch (command.commandType.ToUpper())
        {
            case "MOVE":
                ExecuteMove(player, command);
                break;
                
            case "LOOK":
                ExecuteLook(player, command);
                break;
                
            case "SHOOT":
                ExecuteShoot(player, command);
                break;
                
            case "JUMP":
                ExecuteJump(player, command);
                break;
                
            case "RELOAD":
                ExecuteReload(player, command);
                break;
                
            case "STOP":
                ExecuteStop(player, command);
                break;
                
            case "SWITCH_WEAPON":
                ExecuteSwitchWeapon(player, command);
                break;
                
            default:
                Debug.LogWarning($"[CommandRouter] Unknown command type: {command.commandType}");
                break;
        }
    }
    
    /// <summary>
    /// Validate incoming command
    /// </summary>
    static bool ValidateCommand(AgentCommand command)
    {
        // Check timestamp (reject commands older than 5 seconds)
        double commandAge = Time.time - command.timestamp;
        if (commandAge > 5.0)
        {
            Debug.LogWarning($"[CommandRouter] Command too old: {commandAge:F2}s");
            return false;
        }
        
        // Validate command type
        if (string.IsNullOrEmpty(command.commandType))
        {
            Debug.LogWarning("[CommandRouter] Empty command type");
            return false;
        }
        
        // Validate data based on command type
        switch (command.commandType.ToUpper())
        {
            case "MOVE":
                if (command.data.direction.magnitude > 1.5f) // Allow slight overflow
                {
                    Debug.LogWarning("[CommandRouter] Invalid move direction magnitude");
                    return false;
                }
                break;
                
            case "LOOK":
                if (Mathf.Abs(command.data.pitch) > 90f ||
                    Mathf.Abs(command.data.yaw) > 180f)
                {
                    Debug.LogWarning("[CommandRouter] Look angles out of range");
                    return false;
                }
                break;
        }
        
        return true;
    }
    
    /// <summary>
    /// Execute movement command
    /// </summary>
    static void ExecuteMove(PlayerRoot player, AgentCommand command)
    {
        // command.data.direction: Vector3 (x, y, z) normalized
        Vector3 moveDir = command.data.direction;
        
        // Feed into AIInputFeeder (same as bot control)
        player.AIInputFeeder.OnMove?.Invoke(moveDir);
    }
    
    /// <summary>
    /// Execute look/aim command
    /// </summary>
    static void ExecuteLook(PlayerRoot player, AgentCommand command)
    {
        // command.data.euler: Vector3 (pitch, yaw, roll)
        Vector3 lookEuler = command.data.euler;
        
        // Feed into AIInputFeeder
        player.AIInputFeeder.OnLook?.Invoke(lookEuler);
    }
    
    /// <summary>
    /// Execute shoot command
    /// </summary>
    static void ExecuteShoot(PlayerRoot player, AgentCommand command)
    {
        // command.data.duration: How long to shoot (seconds)
        float duration = command.data.duration;
        
        // Start shooting
        player.AIInputFeeder.OnAttack?.Invoke(true);
        
        // Stop after duration
        if (duration > 0)
        {
            CoroutineManager.Instance.StartCoroutine(StopShootAfterDelay(player, duration));
        }
    }
    
    /// <summary>
    /// Execute jump command
    /// </summary>
    static void ExecuteJump(PlayerRoot player, AgentCommand command)
    {
        // Trigger jump
        player.AIInputFeeder.OnJump?.Invoke(true);
        
        // Reset after short delay
        CoroutineManager.Instance.StartCoroutine(ResetJumpAfterDelay(player));
    }
    
    /// <summary>
    /// Execute reload command
    /// </summary>
    static void ExecuteReload(PlayerRoot player, AgentCommand command)
    {
        // Start reload
        if (player.PlayerReload != null && !player.PlayerReload.IsReloading)
        {
            player.PlayerReload.StartReload();
        }
    }
    
    /// <summary>
    /// Execute stop movement command
    /// </summary>
    static void ExecuteStop(PlayerRoot player, AgentCommand command)
    {
        // Stop all movement
        player.AIInputFeeder.OnMove?.Invoke(Vector3.zero);
        player.AIInputFeeder.OnAttack?.Invoke(false);
    }
    
    /// <summary>
    /// Execute weapon switch command
    /// </summary>
    static void ExecuteSwitchWeapon(PlayerRoot player, AgentCommand command)
    {
        // command.data.weaponIndex: Which weapon to switch to
        int weaponIndex = (int)command.data.weaponIndex;
        
        if (player.PlayerInventory != null)
        {
            player.PlayerInventory.SwitchToWeapon(weaponIndex);
        }
    }
    
    /// <summary>
    /// Stop shooting after delay
    /// </summary>
    static IEnumerator StopShootAfterDelay(PlayerRoot player, float delay)
    {
        yield return new WaitForSeconds(delay);
        player.AIInputFeeder.OnAttack?.Invoke(false);
    }
    
    /// <summary>
    /// Reset jump input after delay
    /// </summary>
    static IEnumerator ResetJumpAfterDelay(PlayerRoot player)
    {
        yield return new WaitForSeconds(0.1f);
        player.AIInputFeeder.OnJump?.Invoke(false);
    }
    
    /// <summary>
    /// Get player controller for agent
    /// In WebSocket mode, there's typically one local player
    /// </summary>
    static PlayerRoot GetPlayerForAgent(string agentId)
    {
        // For single agent controlling local player
        return FindObjectOfType<PlayerRoot>();
    }
}

/// <summary>
/// Helper for running coroutines from static context
/// </summary>
public class CoroutineManager : MonoBehaviour
{
    public static CoroutineManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
