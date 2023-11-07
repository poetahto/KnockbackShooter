using System;
using System.Linq;
using FishNet.Object;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
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

        private GameSettings _settings;

        private void OnEnable()
        {
            _settings = Addressables.LoadAssetAsync<GameSettings>("game_settings").WaitForCompletion();
            EditorSceneManager.sceneOpening += HandleSceneOpening;
        }

        private void OnDisable()
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
            if (GUILayout.Button("Load Persistent"))
                EditorSceneManager.OpenScene("Assets/Scenes/Persistent.unity", OpenSceneMode.Single);

            if (EditorPrefs.HasKey("previous_scene"))
            {
                string previousSceneName = EditorPrefs.GetString("previous_scene");
                
                if (GUILayout.Button($"Load Previous ({previousSceneName})"))
                    EditorSceneManager.OpenScene(previousSceneName, OpenSceneMode.Single);
            }

            Scene currentScene = SceneManager.GetActiveScene();

            if (FindAnyObjectByType<NetworkBehaviour>(FindObjectsInactive.Include) && _settings.networkedLevels.All(settings => settings.sceneName != currentScene.name))
            {
                if (ColoredButton("Register Level", Color.red))
                {
                    // Register this scene with the gameplay levels
                    _settings.networkedLevels.Add(new LevelSettings
                    {
                        sceneName = currentScene.name,
                    });

                }
            }
            
            // Add this scene to the build settings if it hasn't yet been added
            if (!IsInBuildSettings(currentScene.path) && ColoredButton("Add to Build Settings", Color.red))
                AddToBuildSettings(currentScene.path);

            // Dropdown for setting the editor context when launching scenes
            EditorLaunchContext ctx = _settings.editorContext;
            
            if (EditorGUILayout.DropdownButton(new GUIContent(ctx.networkType.ToString()), FocusType.Keyboard))
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Host"), ctx.networkType == NetworkLaunchType.Host, () => ctx.networkType = NetworkLaunchType.Host);
                menu.AddItem(new GUIContent("Client"), ctx.networkType == NetworkLaunchType.Client, () => ctx.networkType = NetworkLaunchType.Client);
                menu.ShowAsContext();
            }

            switch (ctx.networkType)
            {
                case NetworkLaunchType.Host:
                    ctx.hostPort = EditorGUILayout.IntField("Host Port", ctx.hostPort);
                    break;
                case NetworkLaunchType.Client:
                    ctx.clientPort = EditorGUILayout.IntField("Client Port", ctx.clientPort);
                    ctx.clientAddress = EditorGUILayout.TextField("Client Address", ctx.clientAddress);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private static bool ColoredButton(string label, Color color)
        {
            GUI.color = color;
            bool result = GUILayout.Button(label);
            GUI.color = Color.white;
            return result;
        }

        private static bool IsInBuildSettings(string scenePath)
        {
            return EditorBuildSettings.scenes.Any(scene => scene.path == scenePath);
        }

        private static void AddToBuildSettings(string scenePath)
        {
            EditorBuildSettings.scenes = EditorBuildSettings.scenes
                .Append(new EditorBuildSettingsScene(scenePath, true))
                .ToArray();
        }
    }
}