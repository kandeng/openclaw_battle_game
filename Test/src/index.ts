import { UnityWebSocketClient } from './UnityWebSocketClient';

/**
 * Basic test: Connect to Unity and receive game states
 */
async function main() {
  const client = new UnityWebSocketClient('ws://localhost:8080/agent');

  try {
    // Connect to Unity
    await client.connect();

    console.log('\n========================================');
    console.log('  Unity WebSocket Test Client');
    console.log('  Receiving game state updates...');
    console.log('  Press Ctrl+C to stop');
    console.log('========================================\n');

    // Keep running to receive game states
    await new Promise((resolve) => {
      process.on('SIGINT', () => {
        console.log('\n[Test] Disconnecting...');
        client.disconnect();
        resolve(true);
      });
    });

  } catch (error) {
    console.error('[Test] Failed to connect:', error);
    process.exit(1);
  }
}

main();
