using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameSettings : ScriptableObject
{
    public string mainMenuSceneName;
    public List<LevelSettings> gameplayLevels;
}

[Serializable]
public class LevelSettings
{
    public string sceneName;
    // todo: more level info (like gameMode, ect.)
}
