using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FreeForAll.PlayerStates;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
        
        public FfaGameModeSettings Settings { get; private set; }
        public NetworkObject BodyInstance { get; set; }
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

        private void OnDestroy()
        {
            _fsm.Dispose();
        }

        public void ServerInitializeAt(FfaSpawn spawn)
        {
            Transform t = spawn.transform;
            ServerChangeBody(alivePrefab, t.position, t.rotation);
        }

        public void ServerChangeBody(NetworkObject newBody, Vector3 position, Quaternion rotation)
        {
            if (BodyInstance != null && BodyInstance.IsSpawned)
                Game.Instance.Network.ServerManager.Despawn(BodyInstance);

            BodyInstance = Instantiate(newBody, position, rotation);
            Game.Instance.Network.ServerManager.Spawn(BodyInstance, Owner);
        }

        public override void OnStartNetwork() => _fsm.OnStartNetwork(PlayerState);
        public override void OnStartClient() => _fsm.OnStartClient(PlayerState);
        public override void OnStartServer() => _fsm.OnStartServer(PlayerState);
    }
}