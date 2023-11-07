using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameSettings : ScriptableObject
{
    public string mainMenuSceneName;
    public List<LevelSettings> gameplayLevels;
    public EditorLaunchContext editorContext;
}

[Serializable]
public class LevelSettings
{
    public string sceneName;
    // todo: more level info (like gameMode, ect.)
}

public enum NetworkLaunchType
{
    Host,
    Client,
}

[Serializable]
public class EditorLaunchContext
{
    public string sceneName; // Set automatically by the Entrypoint
    
    public NetworkLaunchType networkType = NetworkLaunchType.Host;
    public int hostPort = 5674;
    public int clientPort = 5674;
    public string clientAddress = "localhost";
}