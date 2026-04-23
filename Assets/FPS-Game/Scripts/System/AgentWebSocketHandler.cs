using System;
using UnityEngine;

// Note: These namespaces require websocket-sharp library
using WebSocketSharp;
using WebSocketSharp.Server;

/// <summary>
/// WebSocket behavior handler for agent connections
/// Manages individual agent sessions
/// 
/// REQUIRES: websocket-sharp library
/// </summary>
public class AgentWebSocketHandler : WebSocketBehavior
{
    // Events for server manager to subscribe
    public event Action<string> OnAgentConnected;
    public event Action<string> OnAgentDisconnected;
    public event Action<string, string> OnCommandReceived;
    
    protected override void OnOpen()
    {
        string sessionId = ID;
        Debug.Log($"[AgentWS] Connection opened: {sessionId} from {Context.UserEndPoint}");
        
        // Notify server manager
        OnAgentConnected?.Invoke(sessionId);
        
        // Send welcome message
        string welcome = "{\"type\":\"welcome\",\"message\":\"Connected to Unity WebSocket Server\",\"sessionId\":\"" + sessionId + "\"}";
        Send(welcome);
    }
    
    protected override void OnMessage(MessageEventArgs e)
    {
        string sessionId = ID;
        
        try
        {
            // Forward to server manager
            OnCommandReceived?.Invoke(sessionId, e.Data);
        }
        catch (Exception ex)
        {
            string error = $"{{\"type\":\"error\",\"message\":\"{ex.Message}\"}}";
            Send(error);
            
            Debug.LogError($"[AgentWS] Error processing message: {ex.Message}");
        }
    }
    
    protected override void OnClose(CloseEventArgs e)
    {
        string sessionId = ID;
        Debug.Log($"[AgentWS] Connection closed: {sessionId} - {e.Reason}");
        
        // Notify server manager
        OnAgentDisconnected?.Invoke(sessionId);
    }
    
    protected override void OnError(ErrorEventArgs e)
    {
        Debug.LogError($"[AgentWS] WebSocket error: {e.Message}");
    }
}
