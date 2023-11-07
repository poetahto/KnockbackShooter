using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor
{
    public class SceneToolsWindow : EditorWindow
    {
        [MenuItem("Custom/Scene Tools")]
        private static void ShowWindow()
        {
            var window = GetWindow<SceneToolsWindow>();
            window.titleContent = new GUIContent("Scene Tools");
            window.Show();
        }

        private void Awake()
        {
            EditorSceneManager.sceneOpening += HandleSceneOpening;
        }

        private void OnDestroy()
        {
            EditorSceneManager.sceneOpening -= HandleSceneOpening;
        }

        private static void HandleSceneOpening(string path, OpenSceneMode mode)
        {
            string currentScenePath = SceneManager.GetActiveScene().path;
            
            // We want to ignore scene changes that are repeated - e.g. spamming Entrypoint doesn't clear history
            if (EditorPrefs.HasKey("previous_scene") && path != currentScenePath)
                EditorPrefs.SetString("previous_scene", currentScenePath);
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Entrypoint"))
                EditorSceneManager.OpenScene("Assets/Scenes/Entrypoint.unity", OpenSceneMode.Single);

            if (EditorPrefs.HasKey("previous_scene"))
            {
                string previousSceneName = EditorPrefs.GetString("previous_scene");
                
                if (GUILayout.Button($"Load Previous ({previousSceneName})"))
                    EditorSceneManager.OpenScene(previousSceneName, OpenSceneMode.Single);

            }
        }
    }
}