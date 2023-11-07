using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Entrypoint
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static async void Initialize()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        
        if (currentSceneName != "Entrypoint")
        {
#if UNITY_EDITOR
            var editorSettings = new EditorInitializationSettings
            {
                SceneName = currentSceneName,
                IsHost = true,
            };
        
            SceneManager.LoadScene("Entrypoint");
            await Task.Yield(); // still has the issue where you need to wait a frame for scripts to get Awake
            Game.Instance.InitializeEditor(editorSettings);
#else
            Debug.LogError("Entrypoint should be the first scene in build settings!");
#endif
        }
        else
        {
            await Task.Yield(); // This time it's because we are BeforeSceneLoad
            Game.Instance.InitializeNormal();
        }
    }
}