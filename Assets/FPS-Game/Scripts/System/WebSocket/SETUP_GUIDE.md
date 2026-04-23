# WebSocket Integration Setup Guide

## Overview

This guide walks you through setting up the WebSocket integration for AI agent control of the Unity FPS game.

## Part 1: Unity Setup

### Step 1: Install websocket-sharp Library

**Option A: Unity Package Manager (Recommended)**

1. Open Unity Editor
2. Go to `Window > Package Manager`
3. Click `+` button (top-left)
4. Select `Add package from git URL...`
5. Enter: `https://github.com/sta/websocket-sharp.git`
6. Click `Add`
7. Wait for import to complete

**Option B: Manual DLL Import**

1. Download websocket-sharp from: https://github.com/sta/websocket-sharp/releases
2. Download the latest release (e.g., `1.0.3-rc11`)
3. Extract `websocket-sharp.dll` from the release
4. Copy `websocket-sharp.dll` to: `Assets/Plugins/`
5. Unity will automatically import it

**Verify Installation:**
- Open any C# script in Unity
- Add `using WebSocketSharp;` at the top
- No compile errors = Success!

### Step 2: Configure Game Mode

1. Open `Play Scene` in Unity
2. Find the `InGameManager` GameObject in the hierarchy
3. In the Inspector, locate `Game Mode Configuration`
4. Set **Game Mode** to: `WebSocketAgent`

### Step 3: Add WebSocket Components

1. In the same `InGameManager` GameObject, add these components:
   - `WebSocketServerManager` (Component > Scripts > WebSocketServerManager)
   - `CoroutineManager` (Component > Scripts > CoroutineManager)

2. Configure `WebSocketServerManager`:
   - **Port**: `8080` (default)
   - **Endpoint**: `/agent` (default)
   - **Broadcast Interval**: `0.1` (10 Hz)
   - **Auto Start**: ✓ (checked)

### Step 4: Play the Game

1. Press **Play** in Unity
2. Check Console for:
   ```
   [InGameManager] Initializing WebSocket Agent Mode
   [WebSocketServer] Server started on ws://0.0.0.0:8080/agent
   ```
3. The WebSocket server is now running!

## Part 2: Test Client Setup

### Step 1: Install Node.js Dependencies

```bash
cd Test/
npm install
```

### Step 2: Run Basic Test

```bash
# Start the test client (receives game states)
npm run dev
```

Expected output:
```
[UnityClient] Connecting to ws://localhost:8080/agent...
[UnityClient] ✓ Connected to Unity game
[UnityClient] Connected to Unity WebSocket Server
[UnityClient] Session ID: xxxxxxxx
```

### Step 3: Run Command Tests

```bash
# Test movement
npm run test-move

# Test shooting
npm run test-shoot

# Test full scenario
npm run test-full
```

## Part 3: Integration Testing

### Test 1: Verify Connection

1. Start Unity game (Play mode)
2. Run test client: `npm run dev`
3. Check Unity Console for:
   ```
   [AgentWS] Connection opened: xxxxxxxx
   [WebSocketServer] Agent connected: xxxxxxxx
   ```

### Test 2: Verify Game State Streaming

1. With test client running
2. You should see periodic game state updates:
   ```
   [Game State] Health: 100/100 | Ammo: 30/30 | State: Idle | Enemies: 5
   ```

### Test 3: Send Commands

Run the movement test:
```bash
npm run test-move
```

Watch the Unity game - the player should:
- Move forward
- Move backward
- Move left
- Move right
- Jump

### Test 4: Verify Bi-directional Communication

1. Start test client: `npm run dev`
2. Manually move the player in Unity (if using keyboard)
3. Check test client console - game state should update with new position
4. Run test script: `npm run test-shoot`
5. Check Unity Console - commands should be received and executed

## Troubleshooting

### Issue: "Cannot find namespace 'WebSocketSharp'"

**Solution:** websocket-sharp library is not installed
- Follow Step 1 above
- Verify the library is in `Assets/Plugins/` or installed via Package Manager

### Issue: "Connection refused" in test client

**Solution:**
1. Make sure Unity is in Play mode
2. Check WebSocket server started (see Unity Console)
3. Verify port 8080 is not in use by another application:
   ```bash
   lsof -i :8080
   ```

### Issue: Commands received but not executed

**Solution:**
1. Check `PlayerRoot` exists in scene
2. Verify `AIInputFeeder` is attached to player
3. Check Unity Console for command routing errors
4. Ensure game mode is `WebSocketAgent` (not `Multiplayer`)

### Issue: No game state updates

**Solution:**
1. Check `WebSocketServerManager` is active in scene
2. Verify `BroadcastInterval` is set (default: 0.1)
3. Check Unity Console for broadcasting errors
4. Make sure player is spawned in scene

## Architecture Overview

```
┌─────────────────────────────────────────┐
│         Unity FPS Game                  │
│  ┌──────────────────────────────────┐   │
│  │  WebSocketServerManager          │   │
│  │  - Listens on port 8080          │   │
│  │  - Broadcasts game state (10Hz)  │   │
│  └──────────┬───────────────────────┘   │
│             │                            │
│  ┌──────────▼───────────────────────┐   │
│  │  CommandRouter                   │   │
│  │  - Parses commands               │   │
│  │  - Routes to PlayerController    │   │
│  └──────────┬───────────────────────┘   │
│             │                            │
│  ┌──────────▼───────────────────────┐   │
│  │  AIInputFeeder                   │   │
│  │  - Injects input to player       │   │
│  └──────────────────────────────────┘   │
└─────────────────────────────────────────┘
             │ WebSocket (ws://localhost:8080)
             ▼
┌─────────────────────────────────────────┐
│      OpenClaw Agent / Test Client       │
│  - Sends commands (MOVE, SHOOT, etc.)  │
│  - Receives game state updates         │
│  - LLM converts natural language       │
└─────────────────────────────────────────┘
```

## Next Steps

After verifying the test client works:

1. **Implement OpenClaw Plugin**
   - See WIKI.md Section 10 for OpenClaw plugin code
   - Create `UnityGameAgent` plugin
   - Implement command generation logic

2. **Add LLM Integration**
   - Implement `HighLevelCommandProcessor`
   - Connect to your LLM API
   - Convert natural language to command sequences

3. **Enhance Game State**
   - Add more enemy data (health, weapons, behavior)
   - Include environmental info (obstacles, cover positions)
   - Add minimap/radar information

4. **Implement Multi-Agent Support**
   - Spawn multiple AI-controlled players
   - Route commands by `agentId`
   - Implement team coordination

## Support

- See `WIKI.md` Section 10 for complete architecture documentation
- Check `Test/README.md` for test client usage
- Review Unity Console logs for debugging information

## Files Created

### Unity Scripts (Assets/FPS-Game/Scripts/System/)
- `GameMode.cs` - Game mode enum
- `WebSocketDataStructures.cs` - Data models
- `WebSocketServerManager.cs` - WebSocket server
- `AgentWebSocketHandler.cs` - Connection handler
- `CommandRouter.cs` - Command routing logic
- `CoroutineManager.cs` - Coroutine helper

### Test Client (Test/)
- `src/UnityWebSocketClient.ts` - Main client class
- `src/index.ts` - Basic test
- `src/test-move.ts` - Movement test
- `src/test-shoot.ts` - Shooting test
- `src/test-full-scenario.ts` - Full scenario test
- `package.json` - Dependencies
- `tsconfig.json` - TypeScript config
- `README.md` - Documentation
