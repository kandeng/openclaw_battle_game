# Openclaw Battle Game 

This game is a 3D multiplayer FPS modified from the open‑source GitHub repository [Kieeran/FPS-Game](https://github.com/Kieeran/FPS-Game). 

We have embedded a WebSocket server within the game to connect with the Openclaw gateway. Human players send commands via instant messaging applications such as Telegram, and Openclaw agents remotely control in‑game characters on behalf of the player.

Since the original relay, lobby, authentication and network management modules are no longer required, we have removed them, along with numerous unused game assets.

---

## Features

### Core Gameplay
- **Multiplayer Combat**: Real-time FPS combat with up to 16 players
- **AI Bot System**: Intelligent bots with hybrid FSM-Behavior Tree AI
- **Weapon System**: Rifle, Sniper, Pistol, Melee, and Grenade
- **Dynamic Maps**: Zone-based tactical map with strategic positioning
- **Scoreboard System**: Real-time kill/death tracking and match statistics

### Technical Features
- **Server-Authoritative Architecture**: Anti-cheat design with host validation
- **Network Synchronization**: Unity Netcode for GameObjects (NGO)
- **AI-Powered Development**: Unity AI Assistant integration
- **WebSocket API**: External AI agent control interface
- **Advanced Pathfinding**: Dijkstra + NavMesh hybrid system

---

## System Requirements

### Minimum Requirements
- **Unity Version**: Unity 6000.4 LTS or later
- **Operating System**: Windows 10/11, Ubuntu 20.04+
- **RAM**: 8 GB
- **Storage**: 10 GB available space
- **Network**: Broadband Internet connection (for multiplayer)

### Recommended Requirements
- **Unity Version**: Unity 6000.4 LTS (latest update)
- **Operating System**: Windows 11, Ubuntu 22.04+
- **RAM**: 16 GB
- **Storage**: 15 GB SSD
- **GPU**: DirectX 11 compatible with 2GB VRAM

---

## Installation Guide

### Step 1: Install Unity 6000.4 LTS

1. **Download Unity Hub**: https://unity.com/download
2. **Install Unity Hub** and create/sign in to your Unity account
3. **Install Unity 6000.4 LTS**:
   - Open Unity Hub → Installs tab → Install Editor
   - Select **Unity 6000.4 LTS**
   - Add modules:
     - ✅ Windows Build Support (IL2CPP)
     - ✅ Linux Build Support (if targeting Linux)
     - ✅ Documentation
4. Wait for installation to complete (~5-10 GB)

### Step 2: Clone or Download Project

```bash
# If using Git
git clone https://github.com/kandeng/openclaw_battle_game.git
cd openclaw_battle_game-main

# Or extract the project folder if downloaded as archive
```

### Step 3: Open Project in Unity

1. Open **Unity Hub**
2. Click **Open** → **Open project from disk**
3. Navigate to the project folder
4. Select the folder and click **Open**
5. **First launch** will take 5-15 minutes as Unity imports assets and compiles scripts
6. Wait for the Unity Editor to fully load

### Step 4: Install Required Packages

The project uses Unity Package Manager. Packages will auto-install on first open.

**Verify packages are installed**:
1. Window → Package Manager
2. Check these packages are present:
   - ✅ Netcode for GameObjects (1.12.0+)
   - ✅ Universal Render Pipeline (17.0.0+)
   - ✅ Input System (1.11.0+)
   - ✅ Cinemachine (2.12.0+)
   - ✅ AI Navigation (2.0.0+)

**If packages are missing**:
```
Window → Package Manager → + → Add package by name
```

### Step 5: Install WebSocket Library (For AI Agent Control)

**Required only if using WebSocket AI agent feature**

1. Download websocket-sharp: https://github.com/sta/websocket-sharp/releases
2. Extract `websocket-sharp.dll`
3. Copy to: `Assets/Plugins/websocket-sharp.dll`
4. Unity will auto-import the library

**Verify installation**: No compile errors in Console window

---

## Running the Game

### Option 1: Single Player (Offline Testing)

1. Open Unity Editor
2. Open scene: `Assets/FPS-Game/Scenes/MainScenes/Play Scene`
3. Click **Play** button (▶️) at top center
4. Game starts immediately with AI bots

**Controls**:
- **WASD**: Move
- **Mouse**: Look around
- **Left Click**: Shoot
- **Right Click**: Aim down sights
- **R**: Reload
- **Space**: Jump
- **1, 2, 3**: Switch weapons
- **Shift**: Sprint
- **Ctrl**: Crouch

### Option 2: Multiplayer (Network Play)

#### Host a Game

1. Open Unity Editor
2. Open scene: `Assets/FPS-Game/Scenes/MainScenes/Sign In`
3. Click **Play**
4. Sign in with Unity account
5. Click **Create Lobby**
6. Configure game settings:
   - Max players: 2-16
   - Bot count: 0-8
   - Map: Italy
   - Kill limit: 20-50
7. Click **Start Game**

#### Join a Game

1. Open Unity Editor (on another computer or second instance)
2. Open scene: `Assets/FPS-Game/Scenes/MainScenes/Sign In`
3. Click **Play**
4. Sign in with Unity account
5. Browse available lobbies
6. Select a lobby and click **Join**
7. Wait for game to start

### Option 3: WebSocket AI Agent Mode

**For external AI agent control via WebSocket**

#### Setup Unity Side

1. Open scene: `Assets/FPS-Game/Scenes/MainScenes/Play Scene`
2. Find **InGameManager** GameObject in hierarchy
3. In Inspector, set **Game Mode** to `WebSocketAgent`
4. Add component: **WebSocketServerManager**
5. Add component: **CoroutineManager**
6. Configure WebSocketServerManager:
   - Port: 8080
   - Endpoint: /agent
   - Broadcast Interval: 0.1
   - Auto Start: ✅
7. Click **Play**

#### Run Test Client

```bash
# Open terminal
cd Test/

# Install dependencies (first time only)
npm install

# Run basic test (receive game states)
npm run dev

# Run movement test
npm run test-move

# Run shooting test
npm run test-shoot

# Run full scenario test
npm run test-full
```

**Verify connection**:
- Unity Console: `[WebSocketServer] Server started on ws://0.0.0.0:8080/agent`
- Test client: `[UnityClient] ✓ Connected to Unity game`

---

## Building Standalone Executables

### Build for Windows

1. **File** → **Build Settings**
2. Select **Windows** platform
3. Choose **x86_64** architecture
4. Click **Switch Platform** (if not already selected)
5. Add scenes to build:
   - `Sign In`
   - `Lobby List`
   - `Lobby Room`
   - `Play Scene`
6. Click **Build**
7. Choose output folder
8. Wait for build to complete (5-15 minutes)

**Run the build**:
```bash
# Navigate to build folder
cd Build/Windows/

# Run the executable
./FPSGame.exe
```

### Build for Linux

1. **File** → **Build Settings**
2. Select **Linux** platform
3. Choose **x86_64** architecture
4. Click **Switch Platform**
5. Configure same as Windows build
6. Click **Build**

**Run the build**:
```bash
cd Build/Linux/
chmod +x FPSGame.x86_64
./FPSGame.x86_64
```

---

## Project Structure

```
FPS-Game-20260423/
├── Assets/
│   ├── FPS-Game/
│   │   ├── Animations/          # Character and weapon animations
│   │   ├── Audio/               # Sound effects and music
│   │   ├── Materials/           # Game materials and textures
│   │   ├── Models/              # 3D models (characters, weapons, environment)
│   │   ├── Prefabs/             # Reusable game objects
│   │   ├── Scenes/              # Game scenes
│   │   │   └── MainScenes/      # Main game scenes
│   │   │       ├── Sign In      # Authentication scene
│   │   │       ├── Lobby List   # Lobby browser
│   │   │       ├── Lobby Room   # Multiplayer lobby
│   │   │       └── Play Scene   # Main gameplay
│   │   ├── Scripts/             # All game source code
│   │   │   ├── Player/          # Player controller and systems
│   │   │   ├── Bot/             # AI bot implementation
│   │   │   ├── System/          # Game managers and utilities
│   │   │   ├── TacticalAI/      # Zone and pathfinding systems
│   │   │   └── Network/         # Networking code
│   │   ├── Sound/               # Audio clips
│   │   ├── Sprites/             # UI images and icons
│   │   └── World/               # Level design assets
│   ├── Behavior Designer/       # AI behavior tree framework
│   └── TextMesh Pro/            # Text rendering system
├── Packages/                    # Unity package dependencies
├── ProjectSettings/             # Unity project configuration
├── Test/                        # WebSocket test client
│   ├── src/                     # TypeScript source
│   └── package.json
├── README.md                    # This file
└── WIKI.md                      # Technical documentation
```

---

## Configuration

### Game Settings

Edit game parameters in Unity Inspector:

**InGameManager** (Play Scene):
- Game Mode: Multiplayer | WebSocketAgent | SinglePlayer
- Kill Limit: 20-50
- Match Duration: 5-15 minutes
- Bot Count: 0-8

**PlayerController** (Player Prefab):
- Move Speed: 2.0-6.0
- Sprint Speed: 5.0-8.0
- Jump Height: 1.0-2.0
- Gravity: -15.0 to -25.0

**WebSocketServerManager** (Play Scene):
- Port: 8080 (default)
- Broadcast Rate: 10 Hz (0.1s interval)
- Auto Start: Enabled

### Network Configuration

**Unity Services Setup** (for multiplayer):
1. Edit → Project Settings → Services
2. Link Unity Project (or create new)
3. Enable services:
   - ✅ Authentication
   - ✅ Relay
   - ✅ Lobby
4. Configure region settings

---

## Testing

### Manual Testing

1. **Player Movement**: Test WASD, jump, sprint, crouch
2. **Weapon System**: Test shooting, reloading, switching
3. **AI Bots**: Observe patrol, combat, pathfinding
4. **Network**: Test host/join, synchronization
5. **UI**: Test menus, scoreboard, HUD

### WebSocket Testing

```bash
cd Test/
npm install

# Test 1: Basic connection
npm run dev

# Test 2: Movement commands
npm run test-move

# Test 3: Shooting commands
npm run test-shoot

# Test 4: Full scenario
npm run test-full
```

### Performance Testing

**In Unity Editor**:
- Window → Analysis → Profiler
- Monitor: FPS, CPU, Memory, Rendering

**In Build**:
- Enable stats: Game window → Stats button
- Check: FPS, Draw calls, Triangles, VRAM

---

## Troubleshooting

### Common Issues

#### Unity Won't Open Project
**Error**: "Incompatible Unity version"  
**Fix**: Install Unity 6000.4 LTS or later

#### Compile Errors After Import
**Fix**:
1. Wait for full import to complete (check bottom-right status bar)
2. Window → Package Manager → Reset Packages to defaults
3. Restart Unity Editor

#### WebSocket Connection Failed
**Error**: "Connection refused"  
**Fix**:
1. Verify Unity game is running in Play mode
2. Check port 8080 is not in use
3. Verify websocket-sharp.dll is in Assets/Plugins/

#### Multiplayer Not Working
**Error**: "Cannot connect to lobby"  
**Fix**:
1. Verify Unity Services are enabled
2. Check internet connection
3. Verify Authentication, Relay, Lobby services active

#### Poor Performance
**Symptoms**: Low FPS, stuttering  
**Fix**:
1. Reduce quality settings: Edit → Project Settings → Quality
2. Lower resolution in build settings
3. Disable post-processing effects
4. Check GPU drivers are up to date

---

## Documentation

- **[WIKI.md](./WIKI.md)** - Complete technical documentation:
  - System architecture
  - Workflow and game flow
  - Dataflow diagrams
  - AI bot system design
  - Networking architecture
  - Design patterns
  - Zone system

---

## License

This project was developed as a graduation thesis. All rights reserved.

---

## Credits

- **Game Engine**: Unity 6000.4 LTS
- **Networking**: Unity Netcode for GameObjects, Unity Relay, Unity Lobby
- **AI Framework**: Behavior Designer
- **AI Assistant**: Unity AI Assistant
- **Assets**: Unity Asset Store, Mixamo, OpenGameArt

---

## Support

For issues, questions, or contributions:
1. Check [WIKI.md](./WIKI.md) for technical details
2. Review troubleshooting section above
3. Check Unity Console for error messages
4. Refer to Unity documentation: https://docs.unity3d.com/

---

**Last Updated**: 2026-04-23  
**Unity Version**: 6000.4 LTS  
**Project Status**: Complete
