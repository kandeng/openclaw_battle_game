# WebSocket-Sharp Installation Guide for Unity 6000.4 LTS

## Important: Unity 6000.4 Compatibility

This guide is specifically for Unity 6000.4 LTS. If you're using Unity 2022.3, some steps may differ.

## Option 1: Unity Package Manager (Recommended)

1. Open Unity Package Manager: `Window > Package Manager`
2. Click `+` button → `Add package from git URL...`
3. Enter: `https://github.com/sta/websocket-sharp.git`
4. Click `Add`
5. **Unity 6000.4 Note**: If you get compilation errors, try Option 2

## Option 2: Manual DLL Import (Most Reliable for Unity 6)

1. Download websocket-sharp DLL from: https://github.com/sta/websocket-sharp/releases
2. Download version 1.0.3-rc11 or later
3. Extract `websocket-sharp.dll` from the release
4. Place `websocket-sharp.dll` in `Assets/Plugins/` folder
5. Unity will automatically import it
6. **Verify**: Check Unity Console for any import errors

## Option 3: NuGet for Unity

1. Install NuGetForUnity package via Package Manager
2. Open NuGet: `Window > NuGet > Manage NuGet Packages`
3. Search for `WebSocketSharp`
4. Install version 1.0.3-rc11 or later
5. **Unity 6000.4 Note**: May require additional configuration

## Alternative: NativeWebSocket (If websocket-sharp Fails)

If websocket-sharp doesn't work with Unity 6000.4, use NativeWebSocket:

1. Download from: https://github.com/endel/NativeWebSocket
2. Import into `Assets/Plugins/`
3. Update WebSocketServerManager.cs to use NativeWebSocket API
4. See: `WebSocket/NATIVE_WEBSOCKET_MIGRATION.md` (to be created if needed)

## Verification

After installation, verify by checking:
- ✅ No compile errors in Unity Console
- ✅ `using WebSocketSharp;` works in scripts
- ✅ WebSocketServerManager compiles without errors
- ✅ Can start WebSocket server in Play mode

## Unity 6000.4 Specific Notes

- **API Level**: Unity 6000.4 uses .NET Standard 2.1
- **Threading**: WebSocket server runs on separate thread
- **Security**: Ensure port 8080 is not blocked by firewall
- **Performance**: Unity 6000.4 has improved threading support
