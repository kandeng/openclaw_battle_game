import { UnityWebSocketClient } from './UnityWebSocketClient';

/**
 * Full scenario test: Navigate, aim, shoot, reload
 */
async function main() {
  const client = new UnityWebSocketClient('ws://localhost:8080/agent');

  try {
    await client.connect();
    
    console.log('\n========================================');
    console.log('  Full Scenario Test');
    console.log('  Navigation + Combat Sequence');
    console.log('========================================\n');

    // Phase 1: Move forward while scanning
    console.log('[Phase 1] Moving forward and scanning area...');
    await client.move([0, 0, 1], 3.0);
    
    // Look around while moving
    await client.look(0, -45);
    await client.sleep(500);
    await client.look(0, 0);
    await client.sleep(500);
    await client.look(0, 45);
    await client.sleep(500);
    await client.look(0, 0);

    await client.sleep(1000);

    // Phase 2: Detect and engage enemy (simulated)
    console.log('\n[Phase 2] Enemy detected! Engaging...');
    
    // Aim at enemy
    await client.look(-10, 30);
    await client.sleep(200);
    
    // Fire burst
    await client.shoot(0.8);
    await client.sleep(1000);
    
    // Re-aim and fire again
    await client.look(-5, 35);
    await client.sleep(200);
    await client.shoot(0.6);
    
    await client.sleep(1000);

    // Phase 3: Reload weapon
    console.log('\n[Phase 3] Reloading weapon...');
    await client.reload();
    await client.sleep(2500); // Typical reload time

    // Phase 4: Take cover (move to side)
    console.log('\n[Phase 4] Moving to cover...');
    await client.stop();
    await client.sleep(200);
    await client.move([-1, 0, 0.5], 1.5);
    
    await client.sleep(1000);

    // Phase 5: Final scan
    console.log('\n[Phase 5] Final area scan...');
    await client.look(0, -90);
    await client.sleep(300);
    await client.look(0, -45);
    await client.sleep(300);
    await client.look(0, 0);
    await client.sleep(300);
    await client.look(0, 45);
    await client.sleep(300);
    await client.look(0, 90);
    await client.sleep(500);

    // Print final game state
    const finalState = client.getGameState();
    if (finalState && finalState.player) {
      console.log('\n========================================');
      console.log('  Final Game State:');
      console.log(`  Health: ${finalState.player.health}/${finalState.player.maxHealth}`);
      console.log(`  Ammo: ${finalState.player.currentAmmo}/${finalState.player.maxAmmo}`);
      console.log(`  Position: (${finalState.player.position.x.toFixed(1)}, ${finalState.player.position.y.toFixed(1)}, ${finalState.player.position.z.toFixed(1)})`);
      console.log(`  Enemies: ${finalState.enemies?.filter(e => e.isAlive).length || 0} alive`);
      console.log('========================================\n');
    }

    console.log('[Test] Full scenario complete!');
    client.disconnect();

  } catch (error) {
    console.error('[Test] Failed:', error);
    process.exit(1);
  }
}

// Helper sleep function
function sleep(ms: number): Promise<void> {
  return new Promise(resolve => setTimeout(resolve, ms));
}

// Extend UnityWebSocketClient to expose sleep
declare module './UnityWebSocketClient' {
  interface UnityWebSocketClient {
    sleep(ms: number): Promise<void>;
  }
}

// Monkey-patch for testing (only works in this file)
const originalClient = new UnityWebSocketClient('');
(originalClient as any).sleep = sleep;

main();
