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
        string initialScene = SceneManager.GetActiveScene().name;

        if (initialScene != "Persistent")
            SceneManager.LoadScene("Persistent");

        await Task.Yield(); // Wait until entrypoint loads
        await Game.Instance.InitializeNormal();
        
#if UNITY_EDITOR
        if (initialScene != "Persistent")
            await Game.Instance.InitializeEditor(initialScene);
#endif
    }
}