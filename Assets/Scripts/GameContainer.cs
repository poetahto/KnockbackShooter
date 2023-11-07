using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameContainer : MonoBehaviour
{
    [SerializeField]
    private GameSettings gameSettings;

    private Game _game;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _game = new Game(gameSettings);
    }

    private void OnDestroy()
    {
        _game?.Dispose();
    }
}

public struct EditorInitializationSettings
{
    public string SceneName;
    public bool IsHost;
}

public class Game : IDisposable
{
    public static Game Instance { get; private set; }

    private readonly GameSettings _settings;

    public Game(GameSettings settings)
    {
        Instance = this;
        _settings = settings;
    }
    
    public void Dispose()
    {
        // Do nothing.
    }
    
    public void InitializeNormal()
    {
        SceneManager.LoadScene(_settings.mainMenuSceneName);
    }

    public void InitializeEditor(EditorInitializationSettings editorSettings)
    {
        if (_settings.gameplayLevels.Any(gameplayLevel => gameplayLevel.sceneName == editorSettings.SceneName))
        {
            // todo: load networking and other setup from settings, launch game
            SceneManager.LoadScene(editorSettings.SceneName);
        }
        else // Fallback - just load the scene normally
        {
            SceneManager.LoadScene(editorSettings.SceneName);
        }
    }
}
