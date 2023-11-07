using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Transporting;
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
        Network = InstanceFinder.NetworkManager;
    }
    
    public GameSettings Settings { get; }
    public NetworkManager Network { get; }
    
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
        Network.ServerManager.StartConnection(port);
        Network.ClientManager.StartConnection();
        await UniTask.WaitUntil(() => Network.ServerManager.Started);
        Network.SceneManager.LoadGlobalScenes(new SceneLoadData(initialLevel.sceneName));
        Network.ServerManager.OnServerConnectionState += WaitForServerToStop;

        ImGuiWhiteboard.Instance.Register(DrawHostGUI);
    }

    private void WaitForServerToStop(ServerConnectionStateArgs state)
    {
        if (state.ConnectionState != LocalConnectionState.Stopped)
            return;

        Network.ServerManager.OnServerConnectionState -= WaitForServerToStop;
        ImGuiWhiteboard.Instance.Unregister(DrawHostGUI);
        UniTaskSceneTools.ChangeActiveScene(Settings.mainMenuSceneName).Forget();
    }

    public async UniTask JoinGame(ushort port, string address)
    {
        await UniTaskSceneTools.UnloadActiveScene();
        Network.ClientManager.StartConnection(address, port);
        await UniTask.WaitUntil(() => Network.ClientManager.Started);
        Network.ClientManager.OnClientConnectionState += WaitForClientToStop;
        
        ImGuiWhiteboard.Instance.Register(DrawClientGUI);
    }

    private void WaitForClientToStop(ClientConnectionStateArgs state)
    {
        if (state.ConnectionState != LocalConnectionState.Stopped)
            return;

        Network.ClientManager.OnClientConnectionState -= WaitForClientToStop;
        ImGuiWhiteboard.Instance.Unregister(DrawClientGUI);
        UniTaskSceneTools.ChangeActiveScene(Settings.mainMenuSceneName).Forget();
    }

    public void Dispose()
    {
        if (Network.ServerManager.Started)
            Network.ServerManager.StopConnection(true);

        if (Network.ClientManager.Started)
            Network.ClientManager.StopConnection();
    }

    private void DrawHostGUI()
    {
        GUILayout.Label("[GAME:HOST]");
        GUILayout.Label($"{Network.ServerManager.Clients.Count} Total Players");

        foreach (NetworkConnection networkConnection in Network.ServerManager.Clients.Values)
            GUILayout.Label($"Player: ID={networkConnection.ClientId}");

        if (GUILayout.Button("Stop Server"))
            Network.ServerManager.StopConnection(true);
    }

    private void DrawClientGUI()
    {
        GUILayout.Label("[GAME:CLIENT]");
        GUILayout.Label($"Local client ID: {Network.ClientManager.Connection.ClientId}");

        if (GUILayout.Button("Disconnect"))
            Network.ClientManager.StopConnection();
    }
}