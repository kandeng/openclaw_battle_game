import WebSocket from 'ws';

// ==========================================
// Type Definitions (matching Unity structures)
// ==========================================

export interface CommandData {
  x?: number;
  y?: number;
  z?: number;
  pitch?: number;
  yaw?: number;
  roll?: number;
  duration?: number;
  weaponIndex?: number;
}

export interface AgentCommand {
  commandType: string;
  data: CommandData;
  agentId: string;
  timestamp: number;
}

export interface Vector3 {
  x: number;
  y: number;
  z: number;
}

export interface PlayerState {
  position: Vector3;
  rotation: Vector3;
  velocity: Vector3;
  health: number;
  maxHealth: number;
  currentAmmo: number;
  maxAmmo: number;
  isReloading: boolean;
  kills: number;
  deaths: number;
  currentWeapon: string;
  isGrounded: boolean;
  movementState: string;
}

export interface EnemyState {
  id: string;
  position: Vector3;
  rotation: Vector3;
  health: number;
  distance: number;
  isVisible: boolean;
  isAlive: boolean;
  lastSeenPosition: Vector3;
}

export interface ZoneInfo {
  currentZone: string;
  nearbyZones: string[];
  zoneFullyScanned: boolean;
}

export interface GameInfo {
  gameMode: string;
  matchTime: number;
  maxMatchTime: number;
  isGameActive: boolean;
  killLimit: number;
  currentMap: string;
  zoneInfo: ZoneInfo;
}

export interface GameStateSnapshot {
  timestamp: number;
  frameCount: number;
  player: PlayerState;
  enemies: EnemyState[];
  gameInfo: GameInfo;
}

// ==========================================
// Unity WebSocket Client
// ==========================================

export class UnityWebSocketClient {
  private ws: WebSocket | null = null;
  private isConnected: boolean = false;
  private gameState: GameStateSnapshot | null = null;
  private readonly uri: string;
  private reconnectAttempts: number = 0;
  private maxReconnectAttempts: number = 5;
  private reconnectDelay: number = 2000;

  constructor(uri: string = 'ws://localhost:8080/agent') {
    this.uri = uri;
  }

  /**
   * Connect to Unity WebSocket server
   */
  public connect(): Promise<void> {
    return new Promise((resolve, reject) => {
      try {
        console.log(`[UnityClient] Connecting to ${this.uri}...`);
        
        this.ws = new WebSocket(this.uri);

        this.ws.on('open', () => {
          this.isConnected = true;
          this.reconnectAttempts = 0;
          console.log('[UnityClient] ✓ Connected to Unity game');
          resolve();
        });

        this.ws.on('message', (data: WebSocket.Data) => {
          try {
            const message = JSON.parse(data.toString());
            
            // Handle welcome message
            if (message.type === 'welcome') {
              console.log(`[UnityClient] ${message.message}`);
              console.log(`[UnityClient] Session ID: ${message.sessionId}`);
            }
            // Handle game state updates
            else if (message.player || message.gameInfo) {
              this.gameState = message as GameStateSnapshot;
              this.onGameStateUpdate(this.gameState);
            }
            // Handle error messages
            else if (message.type === 'error') {
              console.error('[UnityClient] Error from server:', message.message);
            }
          } catch (error) {
            console.error('[UnityClient] Failed to parse message:', error);
          }
        });

        this.ws.on('close', (code: number, reason: string) => {
          this.isConnected = false;
          console.log(`[UnityClient] ✗ Connection closed: ${code} - ${reason}`);
          this.handleReconnect();
        });

        this.ws.on('error', (error: Error) => {
          console.error('[UnityClient] WebSocket error:', error.message);
          reject(error);
        });

      } catch (error) {
        console.error('[UnityClient] Connection failed:', error);
        reject(error);
      }
    });
  }

  /**
   * Send command to Unity game
   */
  public async sendCommand(commandType: string, data: CommandData): Promise<void> {
    if (!this.isConnected || !this.ws) {
      console.error('[UnityClient] Not connected to Unity');
      return;
    }

    const command: AgentCommand = {
      commandType,
      data,
      agentId: 'test_client_01',
      timestamp: Date.now() / 1000
    };

    try {
      this.ws.send(JSON.stringify(command));
    } catch (error) {
      console.error('[UnityClient] Failed to send command:', error);
    }
  }

  // ==========================================
  // High-Level Command Methods
  // ==========================================

  /**
   * Move in specified direction
   */
  public async move(direction: [number, number, number], duration: number = 0): Promise<void> {
    console.log(`[UnityClient] MOVE: direction=(${direction.join(',')}), duration=${duration}s`);
    
    await this.sendCommand('MOVE', {
      x: direction[0],
      y: direction[1],
      z: direction[2],
      duration
    });

    if (duration > 0) {
      await this.sleep(duration * 1000);
      await this.stop();
    }
  }

  /**
   * Look in specified direction (Euler angles)
   */
  public async look(pitch: number, yaw: number, roll: number = 0): Promise<void> {
    console.log(`[UnityClient] LOOK: pitch=${pitch}, yaw=${yaw}, roll=${roll}`);
    
    await this.sendCommand('LOOK', {
      pitch,
      yaw,
      roll
    });
  }

  /**
   * Shoot weapon
   */
  public async shoot(duration: number = 0.1): Promise<void> {
    console.log(`[UnityClient] SHOOT: duration=${duration}s`);
    
    await this.sendCommand('SHOOT', {
      duration
    });
  }

  /**
   * Jump
   */
  public async jump(): Promise<void> {
    console.log('[UnityClient] JUMP');
    await this.sendCommand('JUMP', {});
  }

  /**
   * Reload weapon
   */
  public async reload(): Promise<void> {
    console.log('[UnityClient] RELOAD');
    await this.sendCommand('RELOAD', {});
  }

  /**
   * Stop all movement and shooting
   */
  public async stop(): Promise<void> {
    console.log('[UnityClient] STOP');
    await this.sendCommand('STOP', {});
  }

  /**
   * Switch weapon
   */
  public async switchWeapon(weaponIndex: number): Promise<void> {
    console.log(`[UnityClient] SWITCH_WEAPON: index=${weaponIndex}`);
    await this.sendCommand('SWITCH_WEAPON', { weaponIndex });
  }

  // ==========================================
  // Game State Access
  // ==========================================

  /**
   * Get current game state
   */
  public getGameState(): GameStateSnapshot | null {
    return this.gameState;
  }

  /**
   * Check if connected
   */
  public getConnected(): boolean {
    return this.isConnected;
  }

  /**
   * Disconnect from Unity
   */
  public disconnect(): void {
    if (this.ws) {
      this.ws.close();
      this.isConnected = false;
      console.log('[UnityClient] Disconnected');
    }
  }

  // ==========================================
  // Event Handlers (Override these)
  // ==========================================

  /**
   * Called when game state is updated
   */
  protected onGameStateUpdate(state: GameStateSnapshot): void {
    // Override this method to handle game state updates
    // Default: print summary
    if (state.player) {
      const p = state.player;
      console.log(
        `[Game State] Health: ${p.health}/${p.maxHealth} | ` +
        `Ammo: ${p.currentAmmo}/${p.maxAmmo} | ` +
        `State: ${p.movementState} | ` +
        `Enemies: ${state.enemies?.length || 0}`
      );
    }
  }

  // ==========================================
  // Private Methods
  // ==========================================

  private handleReconnect(): void {
    if (this.reconnectAttempts < this.maxReconnectAttempts) {
      this.reconnectAttempts++;
      console.log(`[UnityClient] Reconnecting attempt ${this.reconnectAttempts}/${this.maxReconnectAttempts}...`);
      
      setTimeout(() => {
        this.connect().catch(console.error);
      }, this.reconnectDelay);
    } else {
      console.error('[UnityClient] Max reconnect attempts reached');
    }
  }

  /**
   * Sleep helper
   */
  public sleep(ms: number): Promise<void> {
    return new Promise(resolve => setTimeout(resolve, ms));
  }
}
