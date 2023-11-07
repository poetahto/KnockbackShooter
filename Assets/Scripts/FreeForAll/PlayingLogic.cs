using System;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Managing;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace FreeForAll
{
    [Serializable]
    public class PlayingLogic : StateLogic
    {
        private FfaSpawn[] _spawns;
        private Dictionary<NetworkConnection, int> _knockoutTable = new();
        private KnockoutManager _knockoutManager;
        
        public override void InitializeServer()
        {
            _spawns = Object.FindObjectsByType<FfaSpawn>(FindObjectsSortMode.None);
            _knockoutManager = Object.FindAnyObjectByType<KnockoutManager>();

            if (_spawns.Length <= Settings.requiredPlayers)
                Debug.LogError("There are more players than spawns! This could lead to stacked spawning, so please add more spawns!");
        }

        private IDisposable _timerCallback;

        public override void OnServerEnter()
        {
            NetworkManager network = Game.Instance.Network;
            var remainingSpawns = new List<FfaSpawn>(_spawns);
            _knockoutTable.Clear();
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

                Transform randomSpawn = TakeRandomSpawn(remainingSpawns).transform;
                FfaPlayer instance = Object.Instantiate(Settings.playerPrefab, randomSpawn.position, randomSpawn.rotation);
                network.ServerManager.Spawn(instance.gameObject, connection);
                _knockoutManager.AddObject(instance.gameObject).Subscribe(HandlePlayerKnockout);
                _knockoutTable.Add(connection, 0);
            }
        }

        public override void OnServerLogic()
        {
            Parent.GameTimer.Update(Time.deltaTime);
        }

        public override void OnServerExit()
        {
            _timerCallback?.Dispose();
        }

        private void HandlePlayerKnockout(KnockoutManager.KnockoutData data)
        {
            var player = data.Object.GetComponent<FfaPlayer>();
            _knockoutTable[player.Owner]++;
            
            if (_knockoutTable[player.Owner] < Settings.lives)
            {
                // respawn player if they have lives left
                Transform randomSpawn = _spawns[Random.Range(0, _spawns.Length)].transform;
                player.transform.SetPositionAndRotation(randomSpawn.position, randomSpawn.rotation);
                _knockoutManager.AddObject(player.gameObject).Subscribe(HandlePlayerKnockout);
            }
            else
            {
                // remove and spectate otherwise
                // if removed, check for total elim and victory
                Debug.Log($"Player {player.name} was eliminated!");
                _knockoutTable.Remove(player.Owner);
                if (_knockoutTable.Count <= 1)
                {
                    // victory
                    Parent.GameState.Value = FfaGameMode.State.GameOver;
                } 
            }
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
            
            foreach (KeyValuePair<NetworkConnection, int> playersToDeaths in _knockoutTable)
                GUILayout.Label($"Player {playersToDeaths.Key.ClientId}: {playersToDeaths.Value} Deaths");
        }

        // todo: if playing for too long, go to sudden death
    }
}