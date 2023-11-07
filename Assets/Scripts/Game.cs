using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using FishNet;
using FishNet.Managing;
using FishNet.Managing.Scened;
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
    public async UniTask InitializeNormal()
    {
        Debug.Log("[GAME] Launching the game.");
        await UniTaskSceneTools.LoadAndSetActive(Settings.mainMenuSceneName, LoadSceneMode.Additive);
    }
    
    /// <summary>
    /// Run the game from the Editor's "Enter PlayMode", launching directly into a scene.
    /// </summary>
    public async UniTask InitializeEditor(string sceneName)
    {
        EditorLaunchContext ctx = Settings.editorContext;
        Debug.Log($"[GAME] Launching in an editor environment: {sceneName}");
        
        LevelSettings initialLevel = Settings.networkedLevels
            .FirstOrDefault(gameplayLevel => gameplayLevel.sceneName == sceneName);
        
        if (initialLevel != null)
        {
            Debug.Log($"Launching with networking: {ctx.networkType.ToString()}");
            
            switch (ctx.networkType)
            {
                case NetworkLaunchType.Host:
                    await HostGame((ushort) ctx.hostPort, initialLevel);
                    break;
                case NetworkLaunchType.Client:
                    await JoinGame((ushort) ctx.clientPort, ctx.clientAddress);
                    break;
                default: 
                    throw new ArgumentOutOfRangeException();
            }
        }
        else // Fallback - just load the scene normally
        {
            await UniTaskSceneTools.ChangeActiveScene(sceneName);
        }
    }
    
    public async UniTask HostGame(ushort port, LevelSettings initialLevel)
    {
        await UniTaskSceneTools.UnloadActiveScene();
        FishNetManager.ServerManager.StartConnection(port);
        FishNetManager.ClientManager.StartConnection();
        await UniTask.WaitUntil(() => FishNetManager.ServerManager.Started);
        FishNetManager.SceneManager.LoadGlobalScenes(new SceneLoadData(initialLevel.sceneName));
    }
     
    public async UniTask JoinGame(ushort port, string address)
    {
        await UniTaskSceneTools.UnloadActiveScene();
        FishNetManager.ClientManager.StartConnection(address, port);
        await UniTask.WaitUntil(() => FishNetManager.ClientManager.Started);
    }
    
    public void Dispose()
    {
        if (FishNetManager.ServerManager.Started)
            FishNetManager.ServerManager.StopConnection(true);

        if (FishNetManager.ClientManager.Started)
            FishNetManager.ClientManager.StopConnection();
    }
}