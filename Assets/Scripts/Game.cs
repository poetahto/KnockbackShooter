using System;
using System.Linq;
using FishNet;
using FishNet.Managing;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Global state for the game.
/// Loaded once at startup, persists throughout the entire session.
/// </summary>
public class Game : IDisposable
{
    public static Game Instance { get; private set; }
    
    public Game(GameSettings settings)
    {
        Instance = this;
        Settings = settings;
        FishNetManager = InstanceFinder.NetworkManager;
    }
    
    public GameSettings Settings { get; }
    public NetworkManager FishNetManager { get; }
    
    /// <summary>
    /// Run the game, as if the player has launched the game normally.
    /// </summary>
    public void InitializeNormal()
    {
        Debug.Log("[GAME] Launching the game.");
        SceneManager.LoadScene(Settings.mainMenuSceneName);
    }

    /// <summary>
    /// Run the game from the Editor's "Enter PlayMode", launching directly into a scene.
    /// </summary>
    public void InitializeEditor()
    {
        EditorLaunchContext ctx = Settings.editorContext;
        Debug.Log($"[GAME] Launching in an editor environment: {ctx.sceneName}");
        
        if (Settings.gameplayLevels.Any(gameplayLevel => gameplayLevel.sceneName == ctx.sceneName))
        {
            Debug.Log($"Launching with networking: {ctx.networkType.ToString()}");
            
            switch (ctx.networkType)
            {
                case NetworkLaunchType.Host:
                    FishNetManager.ServerManager.StartConnection((ushort) ctx.hostPort);
                    FishNetManager.ClientManager.StartConnection();
                    break;
                
                case NetworkLaunchType.Client:
                    FishNetManager.ClientManager.StartConnection(ctx.clientAddress, (ushort) ctx.clientPort);
                    break;
                
                default: throw new ArgumentOutOfRangeException();
            }
            
            SceneManager.LoadScene(ctx.sceneName);
        }
        else // Fallback - just load the scene normally
        {
            SceneManager.LoadScene(ctx.sceneName);
        }
    }
    
    public void Dispose()
    {
        if (FishNetManager.ServerManager.Started)
            FishNetManager.ServerManager.StopConnection(true);

        if (FishNetManager.ClientManager.Started)
            FishNetManager.ClientManager.StopConnection();
    }
}