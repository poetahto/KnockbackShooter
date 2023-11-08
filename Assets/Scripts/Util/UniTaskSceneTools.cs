using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

// Utilities for making life with scene management nice.
// Note; these assume the existence of a persistent scene - otherwise we violate the "at least 1" rule
namespace Util
{
    public static class UniTaskSceneTools
    {
        public static async UniTask UnloadActiveScene()
        {
            await SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        }

        public static async UniTask LoadAndSetActive(string sceneName, LoadSceneMode mode = LoadSceneMode.Additive)
        {
            await SceneManager.LoadSceneAsync(sceneName, mode);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        }
    
        public static async UniTask ChangeActiveScene(string sceneName)
        {
            await UnloadActiveScene();
            await LoadAndSetActive(sceneName, LoadSceneMode.Additive);
        }
    }
}