using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public static class SceneUtil
{
    public static async UniTask UnloadActiveScene()
    {
        await SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
    }

    public static async UniTask AdditiveLoadAndSetActive(string sceneName)
    {
        await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
    }
    
    public static async UniTask ChangeActiveScene(string sceneName)
    {
        await UnloadActiveScene();
        await AdditiveLoadAndSetActive(sceneName);
    }
}