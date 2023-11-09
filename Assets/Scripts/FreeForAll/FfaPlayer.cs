using System;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FreeForAll.PlayerStates;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Util;

namespace FreeForAll
{
    public class FfaPlayer : NetworkBehaviour
    {
        public enum State
        {
            Alive,
            Respawning,
            Dead,
        }

        [SerializeField] 
        private AliveLogic aliveLogic;
        
        [SerializeField] 
        private RespawningLogic respawningLogic;

        [SerializeField] 
        private DeadLogic deadLogic;

        public NetworkObject alivePrefab;
        public NetworkObject respawningPrefab;
        public NetworkObject deadPrefab;
        
        public FfaSpawn InitialSpawn { get; set; }
        public FfaGameModeSettings Settings { get; private set; }
        public readonly SyncVar<NetworkObject> BodyInstance = new();
        public readonly SyncVar<State> PlayerState = new();
        public readonly SyncTimer RespawnTimer = new();
        private NetworkedStateMachine<State> _fsm;

        private void Awake()
        {
            aliveLogic.Parent = this;
            respawningLogic.Parent = this;
            deadLogic.Parent = this;

            _fsm = new NetworkedStateMachine<State>(new Dictionary<State, NetworkedStateLogic>
            {
                { State.Alive, aliveLogic },
                { State.Respawning, respawningLogic },
                { State.Dead , deadLogic},
            });

            Settings = Addressables.LoadAssetAsync<FfaGameModeSettings>("ffa_settings").WaitForCompletion();
        }

        private void Start()
        {
            ImGuiWhiteboard.Instance.Register(DrawGUI).AddTo(this);
        }

        private void DrawGUI()
        {
            string stateLabel = $"Player {OwnerId} ";

            stateLabel += PlayerState.Value switch
            {
                State.Alive => $"[Alive] {aliveLogic.Deaths}/{Settings.lives} Lives",
                State.Respawning => $"[Respawning] {RespawnTimer.Remaining}",
                State.Dead => "[Dead]",
                _ => throw new ArgumentOutOfRangeException()
            };
            
            GUILayout.Label(stateLabel);

            if (BodyInstance != null && BodyInstance.Value.TryGetComponent(out ItemSystem itemSystem))
            {
                string itemsLabel = "    Items: ";
                
                foreach (int collectedItem in itemSystem.CollectedItems.Collection)
                {
                    if (itemSystem.SelectedItem.Value == collectedItem)
                        itemsLabel += $"[{Game.Instance.Settings.items[collectedItem].name}] ";
                    
                    else itemsLabel += $"{Game.Instance.Settings.items[collectedItem].name} ";
                }
                
                GUILayout.Label(itemsLabel);
            }
        }

        private void OnDestroy()
        {
            _fsm.Dispose();
        }

        public void ServerChangeBody(NetworkObject newBody, Vector3 position, Quaternion rotation)
        {
            if (BodyInstance.Value != null && BodyInstance.Value.IsSpawned)
                Game.Instance.Network.ServerManager.Despawn(BodyInstance.Value);

            BodyInstance.Value = Instantiate(newBody, position, rotation);
            Game.Instance.Network.ServerManager.Spawn(BodyInstance.Value, Owner);
        }

        public override void OnStartNetwork() => _fsm.OnStartNetwork(PlayerState);
        public override void OnStartClient() => _fsm.OnStartClient(PlayerState);
        
        public override void OnStartServer()
        {
            ServerChangeBody(alivePrefab, InitialSpawn.transform.position, InitialSpawn.transform.rotation);
            _fsm.OnStartServer(PlayerState);
        }
    }
}