using System;
using UniRx;
using UnityEngine;

namespace FreeForAll
{
    [Serializable]
    public class CountdownLogic : StateLogic
    {
        private IDisposable _countdownCallback;
        
        public override void OnServerEnter()
        {
            Parent.CountdownTimer.StartTimer(Settings.countdownDuration);
            
            _countdownCallback = Parent.CountdownTimer
                .ObserveComplete()
                .SubscribeWithState(Parent, (_, all) => all.GameState.Value = FfaGameMode.State.Playing);
        }

        public override void OnServerLogic()
        {
            Parent.CountdownTimer.Update(Time.deltaTime);
            
            int currentPlayers = Game.Instance.Network.ServerManager.Clients.Count;
            
            if (currentPlayers < Settings.requiredPlayers)
                Parent.GameState.Value = FfaGameMode.State.Waiting;
        }

        public override void OnServerExit()
        {
            _countdownCallback?.Dispose();
        }

        public override void OnGui()
        {
            GUILayout.Label($"{Parent.CountdownTimer.Remaining:F1} seconds");
        }
    }
}