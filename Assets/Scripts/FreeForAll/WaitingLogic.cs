using System;
using UnityEngine;

namespace FreeForAll
{
    [Serializable]
    public class WaitingLogic : StateLogic
    {
        public int requiredPlayers = 2;

        public override void OnServerLogic()
        {
            int currentPlayers = Game.Instance.FishNetManager.ServerManager.Clients.Count;

            if (currentPlayers >= requiredPlayers)
                Parent.GameState.Value = GameModeFreeForAll.State.Countdown;
        }

        public override void OnGui()
        {
            int currentPlayers = Game.Instance.FishNetManager.ClientManager.Clients.Count;
            GUILayout.Label($"{currentPlayers}/{requiredPlayers} required players.");
        }
    }
}