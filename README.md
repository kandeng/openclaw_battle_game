# AI Controlled Battle Game 

This game is a 3D multiplayer FPS modified from the open‑source GitHub repository [Kieeran/FPS-Game](https://github.com/Kieeran/FPS-Game). 

We have embedded a WebSocket server within the game to connect with the Openclaw gateway. All game characters are controlled exclusively through two programmatic paths: (1) a WebSocket client connecting to the embedded WebSocket server, and (2) a debug console for local C# calls. Keyboard, mouse, and gamepad input have been fully removed.

Since the original relay, lobby, authentication and network management modules are no longer required, we have removed them, along with numerous unused game assets.

The size of [Kieeran/FPS-Game](https://github.com/Kieeran/FPS-Game) is 4.3G. And the size of our repo is 1.2G.


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
- **Network Synchronization**: Unity Netcode for GameObjects (NGO v2.11.0)
- **WebSocket API**: Primary control path — external AI agent sends JSON commands
- **Debug Console**: Secondary control path — C# API for local/editor testing
- **Unified Command Layer**: `CommandDispatcher` → `IPlayerCommandAPI` (6-method interface)
- **Advanced Pathfinding**: Dijkstra + NavMesh hybrid system
- **No Human Input**: Keyboard, mouse, and gamepad input fully removed

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
git clone https://github.com/kandeng/AI-Controlled-battle-game.git
cd AI-Controlled-battle-game

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
   - ✅ Netcode for GameObjects (2.11.0+)
   - ✅ Universal Render Pipeline (17.0.0+)
   - ✅ Cinemachine (3.x)
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

### Unity Setup (required for all use cases)

1. Open Unity Editor and load: `Assets/FPS-Game/Scenes/MainScenes/Play Scene.unity`
2. Verify these GameObjects / components exist in the scene (they should already be there):
   - **WebSocketServerManager** — port 8080, auto-start ✅
   - **CommandDispatcher** — routes all commands
   - **PlayerCommandAPI** — multi-agent player resolver
   - **CoroutineManager**
3. Click **Play** (▶️)
4. Confirm in the Console: `[WebSocketServer] Server started on ws://0.0.0.0:8080/agent`

> There are no keyboard/mouse controls. The game can only be driven by the WebSocket commands described below, or by `DebugConsole` C# calls (Use Case 3).

---

### Use Case 1 — Single Script Controls One Character

One Python (or JavaScript) script connects as `agent_01`, binds to the first available player slot, and drives that character. The Unity display automatically follows that character’s first-person camera.

#### Quick start (Python)

```bash
# Install dependency (once)
pip install websockets

# Run
cd Test/python
python single_agent.py
```

**What the script does:**
1. Connects to `ws://localhost:8080/agent` with `agentId = "agent_01"`
2. Sends `SET_VIEW` — Unity camera follows this agent’s character
3. Exercises all 6 controls: MOVE, LOOK, SHOOT, RELOAD, SWITCH_WEAPON, AIM

**Expected console output:**
```
[agent_01] Connecting to ws://localhost:8080/agent ...
[agent_01] Connected – session xxxxxxxx
[agent_01] → SET_VIEW {'viewTargetAgentId': 'agent_01'}
--- Move forward ---
[agent_01] → MOVE {'x': 0.0, 'z': 1.0}
[agent_01] HP:100/100 Ammo:30/30 State:Running Enemies:3
...
```

#### Quick start (JavaScript)

```bash
cd Test/javascript
npm install   # first time only
node single_agent.js
```

#### Available commands

| `commandType` | Description | `data` fields |
|---|---|---|
| `MOVE` | Move character | `x` (strafe), `z` (forward) — normalized |
| `LOOK` | Set absolute camera angle | `pitch`, `yaw` (degrees) |
| `SHOOT` | Fire / stop firing | `active` (bool), `duration` (s, optional) |
| `RELOAD` | Reload current weapon | — |
| `SWITCH_WEAPON` | Change weapon slot | `weaponIndex` (0-based) |
| `AIM` | Enter / exit ADS | `active` (bool) |
| `STOP` | Stop movement | — |
| `SET_VIEW` | Point display at agent | `viewTargetAgentId` (default = sender) |

---

### Use Case 2 — Two Scripts, Two Characters

Two agents connect in separate terminals. Each is automatically bound to a different non-bot player slot. They can be on the same team or opposite teams and can see each other in the game world.

#### Run in two terminals

```bash
# Terminal 1 — agent_01 (claims the display by default)
cd Test/python
python dual_agents.py --agent agent_01

# Terminal 2 — agent_02
cd Test/python
python dual_agents.py --agent agent_02
```

Or run both concurrently inside a single process:

```bash
cd Test/python
python dual_agents.py
```

JavaScript equivalents:

```bash
# Terminal 1
cd Test/javascript && npm install
npm run dual-agent01        # node dual_agents.js --agent agent_01

# Terminal 2
cd Test/javascript
npm run dual-agent02        # node dual_agents.js --agent agent_02

# Both in one process
npm run dual
```

#### How the camera view is decided

| Action | Result |
|--------|--------|
| First script to connect sends `SET_VIEW` | Display follows that agent’s character |
| Second script sends `SET_VIEW` with its own `agentId` | Display switches to the second character |
| Either script sends `SET_VIEW` with `viewTargetAgentId` set to the other agent | Display switches to that character |
| No script sends `SET_VIEW` | Unity keeps the last view, or the view is unset |

**Manual override via the in-game dropdown:**

In Unity Editor and Development builds a **View Selector** dropdown appears in the top-right corner of the game window. Click the button to expand it and select any non-bot player to follow.

```
[ View: Player Local  ▾ ]   ← click to open
  ● Player Local               ← currently selected (highlighted blue)
    Player Local (2)
```

#### Code excerpt (Python)

```python
# agent_01 starts and claims the view
await set_view(ws1, "agent_01")          # follow agent_01

# Later, hand the view to agent_02
await set_view(ws1, "agent_01", target_id="agent_02")

# Or from agent_02’s connection:
await set_view(ws2, "agent_02")          # follow agent_02
```

---

### Use Case 3 — Console / Interactive Mode

Type commands in a terminal; the in-game character responds immediately. No pre-scripted scenario — full manual control.

#### Quick start (Python)

```bash
pip install websockets   # once
cd Test/console
python console.py
```

#### Quick start (JavaScript)

```bash
cd Test/console
npm install   # once
node console.js
# or: npm start
```

#### Console session example

```
Connecting to ws://localhost:8080/agent as 'console' ...
Connected. Type 'help' for commands, 'quit' to exit.

cmd> fwd
  → MOVE {"x":0,"z":1}
cmd> look 0 90
  → LOOK {"pitch":0,"yaw":90}
cmd> shoot
  → SHOOT {"active":true,"duration":0.4}
cmd> reload
  → RELOAD
cmd> sw 1
  → SWITCH_WEAPON {"weaponIndex":1}
cmd> aim on
  → AIM {"active":true}
cmd> stop
  → STOP
cmd> quit
```

#### Full command reference

| Command | Description |
|---|---|
| `fwd` / `back` / `left` / `right` | Move in that direction |
| `stop` | Stop movement |
| `look <pitch> <yaw>` | Set absolute camera angle (degrees) |
| `shoot` | Fire a short burst (0.4 s) |
| `reload` | Reload current weapon |
| `sw <slot>` | Switch weapon slot (0-based) |
| `aim on` / `aim off` | Enter / exit ADS |
| `view` | Claim display for this console session |
| `help` | Show help |
| `quit` | Disconnect and exit |

#### Option B — Unity Editor OnGUI panel

While in Play mode, a collapsible panel appears in the Game window (bottom-left). It exposes buttons for MOVE, STOP, LOOK, SHOOT, RELOAD, SWITCH_WEAPON, AIM.

#### Option C — C# script / unit test calls

```csharp
// Any MonoBehaviour, or from the Unity Console window
DebugConsole.Instance.Move(new Vector2(0, 1));     // forward
DebugConsole.Instance.Look(0f, 45f);               // turn right 45°
DebugConsole.Instance.Shoot(true);                 // start firing
DebugConsole.Instance.Shoot(false);                // stop firing
DebugConsole.Instance.Reload();
DebugConsole.Instance.SwitchWeapon(2);             // pistol
DebugConsole.Instance.Aim(true);
DebugConsole.Instance.Aim(false);
```

Every call goes through the same `CommandDispatcher` used by the WebSocket path, so behavior is identical.

---

### Option 4 — Multiplayer (Network Play)

**Note**: The Lobby/Relay/Authentication systems have been removed. Multiplayer now uses direct host/client connections.

#### Host a Game

1. Open Unity Editor
2. Open scene: `Assets/FPS-Game/Scenes/MainScenes/Play Scene.unity`
3. In **InGameManager**, set Game Mode to `Multiplayer`
4. Click **Play**
5. The game starts as host — clients can join via direct IP

#### Join a Game

1. Open Unity Editor on another machine
2. Open the same scene, set Game Mode to `Multiplayer`
3. Click **Play** and connect to the host’s IP address


## Building Standalone Executables

### Build for Windows

1. **File** → **Build Settings**
2. Select **Windows** platform
3. Choose **x86_64** architecture
4. Click **Switch Platform** (if not already selected)
5. Add scenes to build:
   - `Assets/FPS-Game/Scenes/MainScenes/Play Scene.unity`
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
5. Add scenes to build:
   - `Assets/FPS-Game/Scenes/MainScenes/Play Scene.unity`
6. Click **Build**
7. Choose output folder
8. Wait for build to complete (5-15 minutes)

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
│   │   │       └── Play Scene.unity   # Main gameplay (only scene needed)
│   │   ├── Scripts/             # All game source code
│   │   │   ├── Player/          # Player controller and systems
│   │   │   ├── Bot/             # AI bot implementation
│   │   │   ├── System/          # Game managers and utilities
│   │   │   ├── TacticalAI/      # Zone and pathfinding systems
│   │   │   └── Network/         # Networking code
│   │   ├── Sound/               # Audio clips
│   │   ├── Sprites/             # UI images and icons
│   │   └── World/               # Level design assets
│   ├── Plugins/                 # Third-party DLLs
│   │   └── websocket-sharp.dll  # WebSocket library for AI agent
│   ├── Behavior Designer/       # AI behavior tree framework
│   └── TextMesh Pro/            # Text rendering system
├── Packages/                    # Unity package dependencies
├── ProjectSettings/             # Unity project configuration
├── Test/                        # WebSocket client examples
│   ├── python/                  # Python agents
│   │   ├── single_agent.py      # Use Case 1: single agent
│   │   └── dual_agents.py       # Use Case 2: two agents
│   ├── javascript/              # JavaScript agents
│   │   ├── single_agent.js      # Use Case 1: single agent
│   │   ├── dual_agents.js       # Use Case 2: two agents
│   │   └── package.json         # npm scripts + ws dependency
│   └── console/                 # Interactive console (Use Case 3)
│       ├── console.py           # Python REPL
│       ├── console.js           # JavaScript REPL
│       └── package.json         # ws dependency
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

**Note**: Unity Services (Lobby, Relay, Authentication) have been removed. The game now uses direct peer-to-peer connections via Unity Netcode for GameObjects.

**Multiplayer Setup**:
1. Host starts game in Play.unity scene
2. Host's IP address is displayed in console
3. Clients connect directly to host's IP
4. No Unity account or internet connection required for LAN play

---

## Testing

### Manual Testing via DebugConsole

The `DebugConsole` component (in `Assets/FPS-Game/Scripts/Debug/DebugConsole.cs`) provides a GUI panel in Editor/Development builds and 6 public C# methods:

```csharp
DebugConsole.Instance.Move(new Vector2(0, 1));   // move forward
DebugConsole.Instance.Look(0f, 90f);             // turn right
DebugConsole.Instance.Shoot(true);               // start firing
DebugConsole.Instance.Reload();                  // reload
DebugConsole.Instance.SwitchWeapon(1);           // slot index 0-based
DebugConsole.Instance.Aim(true);                 // aim down sights
```

1. **Movement**: Send MOVE command via WebSocket or `DebugConsole.Move()`
2. **Weapon System**: Test SHOOT, RELOAD, SWITCH_WEAPON commands
3. **AI Bots**: Observe patrol, combat, pathfinding
4. **Network**: Test host/join, synchronization
5. **UI**: Test menus, scoreboard, HUD

### WebSocket Testing

**Python:**
```bash
pip install websockets   # once

# Use Case 1
cd Test/python
python single_agent.py

# Use Case 2 — both in one process
python dual_agents.py

# Use Case 2 — separate terminals
# Terminal 1:
python dual_agents.py --agent agent_01
# Terminal 2:
python dual_agents.py --agent agent_02
```

**JavaScript:**
```bash
cd Test/javascript
npm install   # once

# Use Case 1
npm run single

# Use Case 2 — both in one process
npm run dual

# Use Case 2 — separate terminals
# Terminal 1:
npm run dual-agent01
# Terminal 2:
npm run dual-agent02
```

**Console / Interactive (Use Case 3):**
```bash
# Python
cd Test/console
python console.py

# JavaScript
cd Test/console
npm install   # once
node console.js
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
**Error**: "Cannot connect to host"  
**Fix**:
1. Verify host is running Play.unity scene
2. Check both host and client are on same network
3. Verify host's IP address is correct
4. Check firewall isn't blocking port 7777 (default Netcode port)

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
