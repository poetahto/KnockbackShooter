using System;
using UnityEngine;

namespace FreeForAll.PlayerStates
{
    [Serializable]
    public class DeadLogic : FfaPlayerNetworkedState
    {
        public override void OnServerEnter()
        {
            Transform t = Parent.BodyInstance.Value.transform;
            Parent.ServerChangeBody(Parent.deadPrefab, t.position, t.rotation);
        }
    }
}