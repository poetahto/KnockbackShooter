using System;
using UnityEngine;

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

[Serializable]
public class GameSettings
{
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
        Debug.Log("Normal init!");
    }

    public void InitializeToScene(string sceneName)
    {
        Debug.Log($"Scene init! {sceneName}");
    }
}
