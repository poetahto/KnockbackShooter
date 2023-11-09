using System;
using UniRx;
using UnityEngine;
using Util;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace FreeForAll.PlayerStates
{
    [Serializable]
    public class RespawningLogic : FfaPlayerNetworkedState
    {
        private FfaSpawn[] _spawns;
            
        public override void InitializeServer()
        {
            _spawns = Object.FindObjectsByType<FfaSpawn>(FindObjectsSortMode.None);
        }

        public override void OnServerEnter()
        {
            Transform t = Parent.BodyInstance.Value.transform;
            Parent.ServerChangeBody(Parent.respawningPrefab, t.position, t.rotation);
            Parent.RespawnTimer.StartTimer(Settings.respawnTime);
            Parent.RespawnTimer.ObserveComplete().Subscribe(_ => Parent.PlayerState.Value = FfaPlayer.State.Alive);
        }

        public override void OnLogic()
        {
            Parent.RespawnTimer.Update(Time.deltaTime);
        }

        public override void OnServerExit()
        {
            Transform randomSpawn = _spawns[Random.Range(0, _spawns.Length)].transform;
            Parent.ServerChangeBody(Parent.alivePrefab, randomSpawn.position, randomSpawn.rotation);
        }

        public override void OnGui()
        {
            GUILayout.Label($"Respawning in: {Parent.RespawnTimer.Remaining}");
        }
    }
}