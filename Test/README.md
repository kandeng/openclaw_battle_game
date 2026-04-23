# Unity FPS Game - WebSocket Test Client

TypeScript WebSocket client for testing OpenClaw agent integration with Unity FPS game.

## Prerequisites

- Node.js 18+ 
- Unity game running with WebSocket server enabled

## Installation

```bash
# Install dependencies
npm install
```

## Quick Start

### 1. Start Unity Game

1. Open Unity project
2. Set `InGameManager` game mode to **WebSocketAgent**
3. Install websocket-sharp library (see `Assets/FPS-Game/Scripts/System/WebSocket/README_WEBSOCKET_INSTALLATION.md`)
4. Play the game - WebSocket server will start on `ws://localhost:8080/agent`

### 2. Run Test Client

```bash
# Basic test - connect and receive game states
npm run dev

# Or build and run
npm run build
npm start
```

## Test Scripts

### Test 1: Movement Commands
```bash
npm run test-move
```
**What it does:**
- Moves forward for 2 seconds
- Moves backward for 1 second
- Moves left for 1 second
- Moves right for 1 second
- Jumps

### Test 2: Shooting Commands
```bash
npm run test-shoot
```
**What it does:**
- Looks forward and shoots
- Looks left (-90°) and shoots
- Looks right (90°) and shoots
- Looks up (45°) and shoots
- Continuous fire for 2 seconds

### Test 3: Full Scenario
```bash
npm run test-full
```
**What it does:**
- Phase 1: Move forward while scanning area
- Phase 2: Detect and engage enemy (simulated)
- Phase 3: Reload weapon
- Phase 4: Take cover
- Phase 5: Final area scan

## Usage in Your Code

```typescript
import { UnityWebSocketClient } from './UnityWebSocketClient';

async function main() {
  const client = new UnityWebSocketClient('ws://localhost:8080/agent');
  
  // Connect to Unity
  await client.connect();
  
  // Move forward
  await client.move([0, 0, 1], 2.0);
  
  // Look around
  await client.look(0, 45);
  
  // Shoot
  await client.shoot(0.5);
  
  // Jump
  await client.jump();
  
  // Reload
  await client.reload();
  
  // Get game state
  const state = client.getGameState();
  console.log('Player health:', state.player.health);
  
  // Disconnect
  client.disconnect();
}

main();
```

## API Reference

### UnityWebSocketClient

#### Constructor
```typescript
new UnityWebSocketClient(uri: string = 'ws://localhost:8080/agent')
```

#### Methods

| Method | Description | Parameters |
|--------|-------------|------------|
| `connect()` | Connect to Unity server | - |
| `disconnect()` | Disconnect from server | - |
| `move(direction, duration)` | Move in direction | `[x, y, z]`, seconds |
| `look(pitch, yaw, roll)` | Look direction | degrees |
| `shoot(duration)` | Shoot weapon | seconds |
| `jump()` | Jump | - |
| `reload()` | Reload weapon | - |
| `stop()` | Stop all actions | - |
| `switchWeapon(index)` | Switch weapon | weapon index |
| `getGameState()` | Get current state | - |
| `getConnected()` | Check connection | - |

#### Events

```typescript
// Override to handle game state updates
protected onGameStateUpdate(state: GameStateSnapshot): void
```

## Message Format

### Command (Client → Unity)
```json
{
  "commandType": "MOVE",
  "data": {
    "x": 0,
    "y": 0,
    "z": 1
  },
  "agentId": "test_client_01",
  "timestamp": 1234567890
}
```

### Game State (Unity → Client)
```json
{
  "timestamp": 123.45,
  "frameCount": 7410,
  "player": {
    "position": { "x": 0, "y": 1, "z": 0 },
    "rotation": { "x": 0, "y": 0, "z": 0 },
    "health": 100,
    "maxHealth": 100,
    "currentAmmo": 30,
    "maxAmmo": 30,
    "movementState": "Running"
  },
  "enemies": [...],
  "gameInfo": {
    "matchTime": 120.5,
    "isGameActive": true
  }
}
```

## Available Commands

| Command | Description | Data Fields |
|---------|-------------|-------------|
| `MOVE` | Move player | `x, y, z` (direction) |
| `LOOK` | Look direction | `pitch, yaw, roll` (degrees) |
| `SHOOT` | Shoot weapon | `duration` (seconds) |
| `JUMP` | Jump | - |
| `RELOAD` | Reload weapon | - |
| `STOP` | Stop all | - |
| `SWITCH_WEAPON` | Change weapon | `weaponIndex` |

## Troubleshooting

### Connection Refused
**Problem:** `ECONNREFUSED` error  
**Solution:** 
- Make sure Unity game is running
- Check game mode is set to `WebSocketAgent`
- Verify websocket-sharp library is installed

### No Game State Updates
**Problem:** Connected but no state received  
**Solution:**
- Check Unity Console for WebSocket errors
- Verify `WebSocketServerManager` is active in scene
- Check if player exists in scene

### Command Not Executing
**Problem:** Commands sent but no action  
**Solution:**
- Check Unity Console for command routing errors
- Verify `PlayerRoot` exists and is accessible
- Check command format matches expected structure

## Project Structure

```
Test/
├── src/
│   ├── UnityWebSocketClient.ts    # Main client class
│   ├── index.ts                   # Basic connection test
│   ├── test-move.ts               # Movement test
│   ├── test-shoot.ts              # Shooting test
│   └── test-full-scenario.ts      # Full scenario test
├── package.json
├── tsconfig.json
└── README.md
```

## Development

```bash
# Build TypeScript
npm run build

# Run in development mode
npm run dev

# Run specific test
npm run test-move
npm run test-shoot
npm run test-full
```

## License

MIT
