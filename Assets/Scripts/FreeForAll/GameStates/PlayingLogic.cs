using System;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Managing;
using UniRx;
using UnityEngine;
using Util;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace FreeForAll.GameStates
{
    [Serializable]
    public class PlayingLogic : FfaGameModeNetworkedState
    {
        private FfaSpawn[] _spawns;
        private IDisposable _timerCallback;
        private int _remainingPlayers;
        
        public override void InitializeServer()
        {
            _spawns = Object.FindObjectsByType<FfaSpawn>(FindObjectsSortMode.None);
        }
        
        public override void OnServerEnter()
        {
            NetworkManager network = Game.Instance.Network;
            var remainingSpawns = new List<FfaSpawn>(_spawns);
            Parent.GameTimer.StartTimer(Settings.gameDuration);
            
            _timerCallback = Parent.GameTimer
                .ObserveComplete()
                .SubscribeWithState(Parent, (_, mode) => mode.GameState.Value = FfaGameMode.State.SuddenDeath);
            
            foreach (NetworkConnection connection in network.ServerManager.Clients.Values)
            {
                // Refill the spawn pool if we run out
                if (remainingSpawns.Count == 0)
                {
                    remainingSpawns.AddRange(_spawns);
                    Debug.LogWarning("There are more players than spawns! This leads to stacked spawning, so add more spawns!");
                }
                
                FfaPlayer instance = Object.Instantiate(Settings.playerPrefab);
                instance.InitialSpawn = TakeRandomSpawn(remainingSpawns);
                network.ServerManager.Spawn(instance.gameObject, connection);
                
                instance.PlayerState
                    .ObserveChanged()
                    .Where(data => data.Next == FfaPlayer.State.Dead)
                    .Subscribe(_ => ServerHandlePlayerDeath());
            }
        }

        private void ServerHandlePlayerDeath()
        {
            _remainingPlayers--;

            if (_remainingPlayers <= 0)
            {
                Parent.GameState.Value = FfaGameMode.State.GameOver;
            }
        }

        public override void OnLogic()
        {
            Parent.GameTimer.Update(Time.deltaTime);
        }

        public override void OnServerExit()
        {
            _timerCallback?.Dispose();
        }

        private FfaSpawn TakeRandomSpawn(IList<FfaSpawn> pool)
        {
            int randomIndex = Random.Range(0, pool.Count);
            FfaSpawn result = pool[randomIndex];
            pool.RemoveAt(randomIndex);
            return result;
        }

        public override void OnGui()
        {
            GUILayout.Label($"Time until sudden death: {Parent.GameTimer.Remaining}");
        }
    }
}