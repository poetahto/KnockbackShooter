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
            Parent.Deaths.Value++;
            
            Parent.PlayerState.Value = Parent.Deaths.Value > Settings.lives 
                ? FfaPlayer.State.Dead 
                : FfaPlayer.State.Respawning;
        }

        public override void OnGui()
        {
            GUILayout.Label($"Deaths: {Parent.Deaths.Value}/{Settings.lives}");
        }
    }
}