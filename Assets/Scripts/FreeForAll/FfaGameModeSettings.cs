using UnityEngine;

namespace FreeForAll
{
    [CreateAssetMenu]
    public class FfaGameModeSettings : ScriptableObject
    {
        public FfaPlayer playerPrefab;
        
        public int requiredPlayers = 2;
        public float countdownDuration = 10;
        public float gameDuration = 60;
        public int lives = 3;
    }
}