import { UnityWebSocketClient } from './UnityWebSocketClient';

/**
 * Test: Move player in different directions
 */
async function main() {
  const client = new UnityWebSocketClient('ws://localhost:8080/agent');

  try {
    await client.connect();
    console.log('\n========================================');
    console.log('  Test: Movement Commands');
    console.log('========================================\n');

    // Wait a moment for connection to stabilize
    await client.sleep(1000);

    // Test 1: Move forward for 2 seconds
    console.log('[Test 1] Moving forward for 2 seconds...');
    await client.move([0, 0, 1], 2.0);
    await client.sleep(1000);

    // Test 2: Move backward for 1 second
    console.log('\n[Test 2] Moving backward for 1 second...');
    await client.move([0, 0, -1], 1.0);
    await client.sleep(1000);

    // Test 3: Move left for 1 second
    console.log('\n[Test 3] Moving left for 1 second...');
    await client.move([-1, 0, 0], 1.0);
    await client.sleep(1000);

    // Test 4: Move right for 1 second
    console.log('\n[Test 4] Moving right for 1 second...');
    await client.move([1, 0, 0], 1.0);
    await client.sleep(1000);

    // Test 5: Jump
    console.log('\n[Test 5] Jumping...');
    await client.jump();
    await client.sleep(1500);

    console.log('\n========================================');
    console.log('  Movement tests complete!');
    console.log('========================================\n');

    client.disconnect();

  } catch (error) {
    console.error('[Test] Failed:', error);
    process.exit(1);
  }
}

main();
