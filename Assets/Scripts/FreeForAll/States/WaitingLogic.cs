using System;
using UnityEngine;

namespace FreeForAll
{
    [Serializable]
    public class WaitingLogic : StateLogic
    {
        public override void OnServerLogic()
        {
            int currentPlayers = Game.Instance.Network.ServerManager.Clients.Count;

            if (currentPlayers >= Settings.requiredPlayers)
                Parent.GameState.Value = FfaGameMode.State.Countdown;
        }

        public override void OnGui()
        {
            int currentPlayers = Game.Instance.Network.ClientManager.Clients.Count;
            GUILayout.Label($"{currentPlayers}/{Settings.requiredPlayers} required players.");
        }
    }
}