﻿using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameSettings : ScriptableObject
{
    public string mainMenuSceneName;
    public List<LevelSettings> networkedLevels;
    public EditorLaunchContext editorContext;
}

public enum NetworkLaunchType
{
    Host,
    Client,
}

[Serializable]
public class EditorLaunchContext
{
    public NetworkLaunchType networkType = NetworkLaunchType.Host;
    public int hostPort = 5674;
    public int clientPort = 5674;
    public string clientAddress = "localhost";
}