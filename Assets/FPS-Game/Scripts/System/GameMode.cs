/// <summary>
/// Defines the operational mode of the game
/// </summary>
public enum GameMode
{
    /// <summary>
    /// Traditional multiplayer: Auth → Lobby → Relay → NGO
    /// </summary>
    Multiplayer,
    
    /// <summary>
    /// WebSocket mode: Direct AI agent control, no networking services
    /// </summary>
    WebSocketAgent,
    
    /// <summary>
    /// Local single-player for testing
    /// </summary>
    SinglePlayer
}
