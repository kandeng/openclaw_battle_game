import { UnityWebSocketClient } from './UnityWebSocketClient';

/**
 * Test: Look around and shoot
 */
async function main() {
  const client = new UnityWebSocketClient('ws://localhost:8080/agent');

  try {
    await client.connect();
    console.log('\n========================================');
    console.log('  Test: Shooting Commands');
    console.log('========================================\n');

    await client.sleep(1000);

    // Test 1: Look forward and shoot
    console.log('[Test 1] Looking forward (0°) and shooting...');
    await client.look(0, 0);
    await client.sleep(500);
    await client.shoot(0.5);
    await client.sleep(1000);

    // Test 2: Look left and shoot
    console.log('\n[Test 2] Looking left (-90°) and shooting...');
    await client.look(0, -90);
    await client.sleep(500);
    await client.shoot(0.5);
    await client.sleep(1000);

    // Test 3: Look right and shoot
    console.log('\n[Test 3] Looking right (90°) and shooting...');
    await client.look(0, 90);
    await client.sleep(500);
    await client.shoot(0.5);
    await client.sleep(1000);

    // Test 4: Look up and shoot
    console.log('\n[Test 4] Looking up (45°) and shooting...');
    await client.look(45, 0);
    await client.sleep(500);
    await client.shoot(0.5);
    await client.sleep(1000);

    // Test 5: Continuous fire
    console.log('\n[Test 5] Continuous fire for 2 seconds...');
    await client.look(0, 0);
    await client.sleep(300);
    await client.shoot(2.0);
    await client.sleep(2500);

    console.log('\n========================================');
    console.log('  Shooting tests complete!');
    console.log('========================================\n');

    client.disconnect();

  } catch (error) {
    console.error('[Test] Failed:', error);
    process.exit(1);
  }
}

main();
