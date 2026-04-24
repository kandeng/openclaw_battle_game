using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Editor script to automatically setup Play.unity scene with InGameManager
/// Run this once to fix the "No cameras rendering" issue
/// </summary>
public class SceneSetupTool : EditorWindow
{
    [MenuItem("Tools/Setup Play Scene")]
    public static void SetupPlayScene()
    {
        // Open the Play scene
        string scenePath = "Assets/FPS-Game/Scenes/MainScenes/Play.unity";
        
        if (!System.IO.File.Exists(scenePath))
        {
            EditorUtility.DisplayDialog("Error", 
                $"Play.unity not found at: {scenePath}\nPlease verify the scene exists.", 
                "OK");
            return;
        }
        
        // Open scene
        Scene scene = EditorSceneManager.OpenScene(scenePath);
        Debug.Log($"[SceneSetup] Opened scene: {scenePath}");
        
        // Check if InGameManager already exists
        GameObject existingManager = GameObject.Find("InGameManager");
        if (existingManager != null)
        {
            Debug.Log("[SceneSetup] InGameManager already exists in scene");
            EditorUtility.DisplayDialog("Info", 
                "InGameManager is already in the scene!\nNo setup needed.", 
                "OK");
            return;
        }
        
        // Load InGameManager prefab
        string prefabPath = "Assets/FPS-Game/Prefabs/System/InGameManager.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (prefab == null)
        {
            EditorUtility.DisplayDialog("Error", 
                $"InGameManager prefab not found at: {prefabPath}", 
                "OK");
            return;
        }
        
        // Instantiate prefab in scene
        GameObject inGameManager = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        inGameManager.name = "InGameManager";
        
        Debug.Log("[SceneSetup] ✓ Added InGameManager to scene");
        
        // Mark scene as dirty
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        
        Debug.Log("[SceneSetup] ✓ Scene saved");
        
        // Success message
        EditorUtility.DisplayDialog("Success!", 
            "✓ InGameManager added to Play.unity scene\n\n" +
            "Next steps:\n" +
            "1. Select InGameManager in Hierarchy\n" +
            "2. In Inspector, set Game Mode to 'SinglePlayer' (for testing)\n" +
            "3. Press Play (▶️) to test", 
            "OK");
    }
    
    [MenuItem("Tools/Verify Scene Setup")]
    public static void VerifySceneSetup()
    {
        GameObject manager = GameObject.Find("InGameManager");
        
        if (manager == null)
        {
            EditorUtility.DisplayDialog("Verification Failed", 
                "❌ InGameManager NOT found in current scene!\n\n" +
                "Please run: Tools → Setup Play Scene", 
                "OK");
            return;
        }
        
        // Check if script is attached
        var script = manager.GetComponent<MonoBehaviour>();
        if (script == null)
        {
            EditorUtility.DisplayDialog("Warning", 
                "⚠️ InGameManager exists but script may be missing!\n\n" +
                "Check the Inspector for 'Missing Script' errors.", 
                "OK");
            return;
        }
        
        EditorUtility.DisplayDialog("Verification Passed", 
            "✓ InGameManager found in scene\n" +
            "✓ Script attached: " + script.GetType().Name + "\n\n" +
            "Ready to play!", 
            "OK");
    }
}
