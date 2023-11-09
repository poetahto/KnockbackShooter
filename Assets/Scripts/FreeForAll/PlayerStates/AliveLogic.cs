using System;
using Knockouts;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FreeForAll.PlayerStates
{
    [Serializable]
    public class AliveLogic : FfaPlayerNetworkedState
    {
        private KnockoutManager _knockoutManager;

        public int Deaths { get; private set; }

        public override void InitializeServer()
        {
            _knockoutManager = Object.FindAnyObjectByType<KnockoutManager>();
        }

        public override void OnServerEnter()
        {
            _knockoutManager.AddObject(Parent.BodyInstance.Value.gameObject).Subscribe(ServerHandlePlayerKnockout);
        }

        private void ServerHandlePlayerKnockout(KnockoutManager.KnockoutData data)
        {
            Deaths++;

            if (Deaths > Settings.lives)
            {
                Parent.PlayerState.Value = FfaPlayer.State.Dead;
            }
            else
            {
                Parent.PlayerState.Value = FfaPlayer.State.Respawning;
            }
        }

        public override void OnGui()
        {
            GUILayout.Label($"Deaths: {Deaths}/{Settings.lives}");
        }
    }
}