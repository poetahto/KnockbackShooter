using System;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FreeForAll.PlayerStates
{
    [Serializable]
    public class AliveLogic : FfaPlayerNetworkedState
    {
        private KnockoutManager _knockoutManager;
        private int _deaths;

        public override void InitializeServer()
        {
            _knockoutManager = Object.FindAnyObjectByType<KnockoutManager>();
        }

        public override void OnServerEnter()
        {
            _knockoutManager.AddObject(Parent.BodyInstance.gameObject).Subscribe(ServerHandlePlayerKnockout);
        }

        private void ServerHandlePlayerKnockout(KnockoutManager.KnockoutData data)
        {
            _deaths++;

            if (_deaths > Settings.lives)
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
            GUILayout.Label($"Deaths: {_deaths}/{Settings.lives}");
        }
    }
}