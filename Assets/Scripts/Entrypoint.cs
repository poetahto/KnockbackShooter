using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Entrypoint
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static async void Initialize()
    {
#if UNITY_EDITOR
        Scene activeScene = SceneManager.GetActiveScene();
        string desiredSceneName = activeScene.name;
#endif
        
        SceneManager.LoadScene("Entrypoint");
        await Task.Yield(); // still has the issue where you need to wait a frame for scripts to get Awake
        
#if UNITY_EDITOR
        Game.Instance.InitializeToScene(desiredSceneName);
#else
        Game.Instance.InitializeNormal();
#endif
    }
}