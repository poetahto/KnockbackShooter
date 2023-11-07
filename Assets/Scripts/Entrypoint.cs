using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Hook for starting up game logic before anything loads.
/// Responsible for initializing the global Game instance,
/// and providing it with Editor-only playmode settings.
/// </summary>
public static class Entrypoint
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] 
    // Timing is "BeforeSceneLoad" so scripts that depend on global state don't throw errors.
    private static async void Initialize()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "Entrypoint")
        {
            await Task.Yield(); // Wait until entrypoint loads, since this is BeforeSceneLoad
            Game.Instance.InitializeNormal();
        }
        else // Entrypoint needs to be loaded: only valid in the editor, error otherwise.
        {
#if UNITY_EDITOR
            Debug.Log("[ENTRYPOINT] Editor only: loading the entrypoint scene.");
            
            SceneManager.LoadScene("Entrypoint");
            await Task.Yield(); // still has the issue where you need to wait a frame for scripts to get Awake
            Game.Instance.Settings.editorContext.sceneName = currentSceneName;
            Game.Instance.InitializeEditor();
#else
            Debug.LogError("Entrypoint should be the first scene in build settings!");
#endif
        }
    }
}