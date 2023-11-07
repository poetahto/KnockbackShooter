using System;
using UniRx;
using UnityEngine;

namespace FreeForAll
{
    [Serializable]
    public class CountdownLogic : StateLogic
    {
        public float countdownDuration = 10;

        private IDisposable _disposable;
        
        public override void OnServerEnter()
        {
            Parent.CountdownTimer.StartTimer(countdownDuration);
            
            _disposable = Parent.CountdownTimer
                .ObserveComplete()
                .SubscribeWithState(Parent, (_, all) => all.GameState.Value = GameModeFreeForAll.State.Playing);
        }

        public override void OnServerLogic()
        {
            Parent.CountdownTimer.Update(Time.deltaTime);
            
            int currentPlayers = Game.Instance.FishNetManager.ServerManager.Clients.Count;
            
            if (currentPlayers < Parent.waitingLogic.requiredPlayers)
                Parent.GameState.Value = GameModeFreeForAll.State.Waiting;
        }

        public override void OnServerExit()
        {
            _disposable?.Dispose();
        }

        public override void OnGui()
        {
            GUILayout.Label($"{Parent.CountdownTimer.Remaining:F1} seconds");
        }
    }
}