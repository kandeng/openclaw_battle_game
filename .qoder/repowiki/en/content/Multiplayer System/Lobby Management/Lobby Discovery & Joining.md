# Lobby Discovery & Joining

<cite>
**Referenced Files in This Document**
- [LobbyManager.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyManager.cs)
- [LobbyListUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyListUI.cs)
- [LobbyListSingleUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyListSingleUI.cs)
- [LobbyCreateUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyCreateUI.cs)
- [LobbyUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyUI.cs)
- [SlotManager.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/SlotManager.cs)
- [Relay.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/Relay.cs)
- [LobbyRelayChecker.cs](file://Assets/FPS-Game/Scripts/System/LobbyRelayChecker.cs)
- [PlayerNetwork.cs](file://Assets/FPS-Game/Scripts/Player/PlayerNetwork.cs)
</cite>

## Table of Contents
1. [Introduction](#introduction)
2. [Project Structure](#project-structure)
3. [Core Components](#core-components)
4. [Architecture Overview](#architecture-overview)
5. [Detailed Component Analysis](#detailed-component-analysis)
6. [Dependency Analysis](#dependency-analysis)
7. [Performance Considerations](#performance-considerations)
8. [Troubleshooting Guide](#troubleshooting-guide)
9. [Conclusion](#conclusion)
10. [Appendices](#appendices)

## Introduction
This document explains the lobby discovery and joining mechanisms implemented in the project. It covers how lobbies are listed, filtered, sorted, and paginated; how players join via lobby code, direct selection, or programmatic actions; and how the system handles join permissions, private lobbies, full lobbies, and join failures. It also provides guidance on optimizing discovery, caching strategies, and designing user-friendly lobby browser interfaces.

## Project Structure
The lobby system spans several scripts under the Lobby Script module and integrates with Relay and player networking systems:
- Lobby discovery and management: LobbyManager
- UI for listing and selecting lobbies: LobbyListUI, LobbyListSingleUI
- UI for creating lobbies: LobbyCreateUI
- UI for the joined lobby room: LobbyUI, SlotManager
- Relay hosting/joining: Relay
- Relay connection verification: LobbyRelayChecker
- Player networking and lobby-aware behavior: PlayerNetwork

```mermaid
graph TB
LM["LobbyManager.cs"]
LLUI["LobbyListUI.cs"]
LLSUI["LobbyListSingleUI.cs"]
LCUI["LobbyCreateUI.cs"]
LUI["LobbyUI.cs"]
SM["SlotManager.cs"]
RL["Relay.cs"]
LRC["LobbyRelayChecker.cs"]
PN["PlayerNetwork.cs"]
LLUI --> LM
LLSUI --> LM
LCUI --> LM
LUI --> LM
SM --> LM
LM --> RL
LM --> PN
LRC --> LM
```

**Diagram sources**
- [LobbyManager.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyManager.cs)
- [LobbyListUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyListUI.cs)
- [LobbyListSingleUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyListSingleUI.cs)
- [LobbyCreateUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyCreateUI.cs)
- [LobbyUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyUI.cs)
- [SlotManager.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/SlotManager.cs)
- [Relay.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/Relay.cs)
- [LobbyRelayChecker.cs](file://Assets/FPS-Game/Scripts/System/LobbyRelayChecker.cs)
- [PlayerNetwork.cs](file://Assets/FPS-Game/Scripts/Player/PlayerNetwork.cs)

**Section sources**
- [LobbyManager.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyManager.cs)
- [LobbyListUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyListUI.cs)
- [LobbyListSingleUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyListSingleUI.cs)
- [LobbyCreateUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyCreateUI.cs)
- [LobbyUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyUI.cs)
- [SlotManager.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/SlotManager.cs)
- [Relay.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/Relay.cs)
- [LobbyRelayChecker.cs](file://Assets/FPS-Game/Scripts/System/LobbyRelayChecker.cs)
- [PlayerNetwork.cs](file://Assets/FPS-Game/Scripts/Player/PlayerNetwork.cs)

## Core Components
- LobbyManager: Central orchestrator for authentication, lobby lifecycle, listing, joining, polling, and game start signaling via Relay.
- LobbyListUI: Renders the lobby browser, triggers refresh, and handles code-based join.
- LobbyListSingleUI: Individual row renderer for a lobby with join action.
- LobbyCreateUI: Collects parameters to create a new lobby.
- LobbyUI and SlotManager: Joined lobby UI and player/slot management.
- Relay: Creates/Joins Relay allocations and configures NetworkManager transport.
- LobbyRelayChecker: Periodically verifies all players have connected to the Relay.
- PlayerNetwork: Integrates lobby data with runtime networking.

Key responsibilities:
- Discovery: Query open lobbies with AvailableSlots filter, sort by Created descending, and cap count.
- Joining: By code, by direct selection, or programmatic join.
- Filtering: Only show open lobbies with available slots.
- Permissions: Host-only controls (start game, kick, bot adjustments).
- Failure handling: Private lobby errors, accessibility changes, and re-entry to UI.

**Section sources**
- [LobbyManager.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyManager.cs)
- [LobbyListUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyListUI.cs)
- [LobbyListSingleUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyListSingleUI.cs)
- [LobbyCreateUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyCreateUI.cs)
- [LobbyUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyUI.cs)
- [SlotManager.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/SlotManager.cs)
- [Relay.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/Relay.cs)
- [LobbyRelayChecker.cs](file://Assets/FPS-Game/Scripts/System/LobbyRelayChecker.cs)
- [PlayerNetwork.cs](file://Assets/FPS-Game/Scripts/Player/PlayerNetwork.cs)

## Architecture Overview
The lobby discovery and joining pipeline connects UI, manager, Unity Services, and Relay.

```mermaid
sequenceDiagram
participant UI_List as "LobbyListUI.cs"
participant Manager as "LobbyManager.cs"
participant LobbiesSvc as "Unity Services Lobbies"
participant RelaySvc as "Relay.cs"
UI_List->>Manager : "RefreshLobbyList()"
Manager->>LobbiesSvc : "QueryLobbiesAsync(filters, order, count)"
LobbiesSvc-->>Manager : "QueryResponse(results)"
Manager-->>UI_List : "OnLobbyListChanged(results)"
UI_List->>Manager : "JoinLobbyByCode(code)"
Manager->>LobbiesSvc : "JoinLobbyByCodeAsync(code)"
LobbiesSvc-->>Manager : "Lobby"
Manager-->>UI_List : "OnJoinedLobby(lobby)"
```

**Diagram sources**
- [LobbyListUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyListUI.cs)
- [LobbyManager.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyManager.cs)

## Detailed Component Analysis

### Lobby Listing and Discovery
- Query filters:
  - Filters for open lobbies only: AvailableSlots > 0.
- Sorting:
  - Order by Created descending to show newest lobbies first.
- Pagination:
  - Count set to 25 per page; auto-refresh loop runs periodically.

```mermaid
flowchart TD
Start(["RefreshLobbyList"]) --> BuildQuery["Build QueryLobbiesOptions<br/>Filters: AvailableSlots > 0<br/>Order: Created desc<br/>Count: 25"]
BuildQuery --> CallAPI["Call QueryLobbiesAsync()"]
CallAPI --> Success{"Success?"}
Success --> |Yes| InvokeEvent["Invoke OnLobbyListChanged(results)"]
Success --> |No| LogError["Log exception"]
InvokeEvent --> End(["Done"])
LogError --> End
```

**Diagram sources**
- [LobbyManager.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyManager.cs)

**Section sources**
- [LobbyManager.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyManager.cs)

### Joining Mechanisms
- By lobby code:
  - UI captures code and delegates to LobbyManager.JoinLobbyByCode.
- By direct selection:
  - Each row exposes a join button; clicking invokes LobbyManager.JoinLobby(lobby).
- Programmatic joining:
  - Called internally after successful query or when a user selects a lobby row.

```mermaid
sequenceDiagram
participant UI_Row as "LobbyListSingleUI.cs"
participant Manager as "LobbyManager.cs"
participant LobbiesSvc as "Unity Services Lobbies"
UI_Row->>Manager : "JoinLobby(lobby)"
Manager->>LobbiesSvc : "JoinLobbyByIdAsync(lobby.Id)"
LobbiesSvc-->>Manager : "Lobby"
Manager-->>UI_Row : "OnJoinedLobby(lobby)"
```

**Diagram sources**
- [LobbyListSingleUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyListSingleUI.cs)
- [LobbyManager.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyManager.cs)

**Section sources**
- [LobbyListSingleUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyListSingleUI.cs)
- [LobbyManager.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyManager.cs)

### Filtering Logic for Open Lobbies with Available Slots
- The query explicitly filters to exclude full lobbies by requiring AvailableSlots > 0.
- This ensures the UI only displays lobbies suitable for immediate joining.

```mermaid
flowchart TD
A["Query Options"] --> B["Filter: AvailableSlots > 0"]
B --> C["Sort: Created desc"]
C --> D["Count: 25"]
D --> E["Return results"]
```

**Diagram sources**
- [LobbyManager.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyManager.cs)

**Section sources**
- [LobbyManager.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyManager.cs)

### Handling Join Requests and Permissions
- Host-only controls:
  - Start game, kick players, adjust bot count.
- Join flow:
  - After successful join, UI navigates to the lobby room scene and updates the joined lobby state.
- Polling and heartbeat:
  - Host pings heartbeat periodically.
  - Clients poll the lobby to detect kicks, updates, and start signals.

```mermaid
sequenceDiagram
participant Host as "Host Client"
participant Manager as "LobbyManager.cs"
participant RelaySvc as "Relay.cs"
participant Clients as "Other Clients"
Host->>Manager : "StartGame()"
Manager->>RelaySvc : "CreateRelay(playerCount)"
RelaySvc-->>Manager : "joinCode"
Manager->>Manager : "UpdateLobby(data : start signal)"
Manager-->>Clients : "OnJoinedLobbyUpdate(lobby)"
Clients->>RelaySvc : "JoinRelay(joinCode)"
```

**Diagram sources**
- [LobbyManager.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyManager.cs)
- [Relay.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/Relay.cs)

**Section sources**
- [LobbyManager.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyManager.cs)
- [LobbyUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyUI.cs)
- [SlotManager.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/SlotManager.cs)
- [Relay.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/Relay.cs)

### Managing Join Failures and Private Lobbies
- Private/Inaccessible Lobbies:
  - When a lobby becomes private or otherwise inaccessible, polling catches a specific error and redirects the client back to the lobby list.
- Full Lobbies:
  - Discovery filters prevent listing full lobbies; attempting to join a full lobby would fail at the service level and surface as a failure to the caller.

```mermaid
flowchart TD
Start(["Poll Joined Lobby"]) --> Fetch["GetLobbyAsync"]
Fetch --> Accessible{"Accessible?"}
Accessible --> |No| Redirect["Redirect to Lobby Room UI"]
Redirect --> End(["Exit"])
Accessible --> |Yes| CheckPlayer{"Player still present?"}
CheckPlayer --> |No| Kicked["Invoke OnKickedFromLobby"]
Kicked --> End
CheckPlayer --> |Yes| Update["Invoke OnJoinedLobbyUpdate"]
Update --> End
```

**Diagram sources**
- [LobbyManager.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyManager.cs)

**Section sources**
- [LobbyManager.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyManager.cs)

### Practical Examples: Implementing a Lobby Browser
- Bind refresh button to trigger RefreshLobbyList.
- Render rows with LobbyListSingleUI; each row binds to JoinLobby(lobby).
- Support code-based join via an input field and JoinLobbyByCode.
- Update UI counters to reflect current players and bots.

```mermaid
classDiagram
class LobbyListUI {
+RefreshLobbyList()
+JoinLobbyByCode(code)
+Show()
+Hide()
}
class LobbyListSingleUI {
+UpdateLobby(lobby)
}
class LobbyCreateUI {
+Show()
+Hide()
+ReadLobbyName(s)
+ReadMaxPlayers(s)
+ReadIsPrivate(b)
}
class LobbyManager {
+RefreshLobbyList()
+JoinLobbyByCode(code)
+JoinLobby(lobby)
+CreateLobby(name,maxPlayers,isPrivate)
}
LobbyListUI --> LobbyManager : "calls"
LobbyListSingleUI --> LobbyManager : "calls JoinLobby"
LobbyCreateUI --> LobbyManager : "calls CreateLobby"
```

**Diagram sources**
- [LobbyListUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyListUI.cs)
- [LobbyListSingleUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyListSingleUI.cs)
- [LobbyCreateUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyCreateUI.cs)
- [LobbyManager.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyManager.cs)

**Section sources**
- [LobbyListUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyListUI.cs)
- [LobbyListSingleUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyListSingleUI.cs)
- [LobbyCreateUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyCreateUI.cs)
- [LobbyManager.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyManager.cs)

## Dependency Analysis
- UI depends on LobbyManager events and methods.
- LobbyManager depends on Unity Services (Lobbies, Authentication) and Relay.
- SlotManager and LobbyUI depend on LobbyManager’s joined lobby state and events.
- Relay integrates with NetworkManager transport and Unity Transport.
- PlayerNetwork reads lobby data to map player identities server-side.

```mermaid
graph LR
UI_List["LobbyListUI.cs"] --> LM["LobbyManager.cs"]
UI_Row["LobbyListSingleUI.cs"] --> LM
UI_Create["LobbyCreateUI.cs"] --> LM
LM --> Lobbies["Unity Services Lobbies"]
LM --> Relay["Relay.cs"]
LM --> PN["PlayerNetwork.cs"]
SlotMgr["SlotManager.cs"] --> LM
LUI["LobbyUI.cs"] --> LM
LRC["LobbyRelayChecker.cs"] --> LM
```

**Diagram sources**
- [LobbyListUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyListUI.cs)
- [LobbyListSingleUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyListSingleUI.cs)
- [LobbyCreateUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyCreateUI.cs)
- [LobbyManager.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyManager.cs)
- [SlotManager.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/SlotManager.cs)
- [LobbyUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyUI.cs)
- [Relay.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/Relay.cs)
- [LobbyRelayChecker.cs](file://Assets/FPS-Game/Scripts/System/LobbyRelayChecker.cs)
- [PlayerNetwork.cs](file://Assets/FPS-Game/Scripts/Player/PlayerNetwork.cs)

**Section sources**
- [LobbyManager.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyManager.cs)
- [LobbyListUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyListUI.cs)
- [LobbyListSingleUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyListSingleUI.cs)
- [LobbyCreateUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyCreateUI.cs)
- [LobbyUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyUI.cs)
- [SlotManager.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/SlotManager.cs)
- [Relay.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/Relay.cs)
- [LobbyRelayChecker.cs](file://Assets/FPS-Game/Scripts/System/LobbyRelayChecker.cs)
- [PlayerNetwork.cs](file://Assets/FPS-Game/Scripts/Player/PlayerNetwork.cs)

## Performance Considerations
- Discovery optimization:
  - Keep Count reasonable (currently 25) to balance freshness and bandwidth.
  - Debounce manual refresh actions in UI to avoid rapid repeated queries.
- Filtering and sorting:
  - Server-side filters and sorts reduce client-side work; keep filters minimal and precise.
- Polling and heartbeat:
  - Tune intervals (current heartbeat ~25s, polling ~1.5s) to balance responsiveness and cost.
- UI rendering:
  - Reuse row templates and destroy old entries efficiently to minimize GC pressure.
- Caching:
  - Cache recent lobby lists per session; invalidate on explicit refresh or join events.
  - Avoid redundant refreshes during short time windows after joins or creates.
- Network reliability:
  - Retry transient failures gracefully; surface user-friendly messages for persistent errors.

[No sources needed since this section provides general guidance]

## Troubleshooting Guide
Common scenarios and resolutions:
- Join fails due to private lobby:
  - The system detects a private/inaccessible state and returns the client to the lobby list UI.
- Full lobby:
  - Discovery filters prevent listing full lobbies; if a lobby appears full, it is intentionally excluded from the list.
- Kicked or removed:
  - Polling detects absence from the lobby and triggers a kick event; UI returns to the lobby list.
- Start game timing:
  - Only hosts can start the game; clients wait for the start signal and join the Relay accordingly.

Operational tips:
- Verify authentication state before refreshing lists.
- Ensure UI event handlers are attached and detached properly to avoid leaks.
- Monitor logs for lobby exceptions and relay errors.

**Section sources**
- [LobbyManager.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyManager.cs)
- [LobbyListUI.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/LobbyListUI.cs)
- [SlotManager.cs](file://Assets/FPS-Game/Scripts/Lobby%20Script/Lobby/Scripts/SlotManager.cs)

## Conclusion
The lobby discovery and joining system centers on a robust query pipeline that filters and sorts lobbies, a flexible join mechanism supporting code-based and direct selection, and a reliable polling model for updates and game start signaling. With host-only controls and clear failure handling, the system provides a solid foundation for building responsive and user-friendly lobby browsing experiences.

[No sources needed since this section summarizes without analyzing specific files]

## Appendices

### Best Practices for Lobby Browsing Interfaces
- Always pre-filter to show only open lobbies with available slots.
- Paginate and throttle refreshes; offer manual refresh with feedback.
- Provide clear join buttons per row and a secondary code-entry option.
- Show accurate player counts including bots and max capacity.
- Offer quick actions (create, refresh) and safe exits (leave, quit) with appropriate prompts.

[No sources needed since this section provides general guidance]