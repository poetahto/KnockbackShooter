using Core;
using FishNet.Object;
using UnityEngine;

public class CollectableItem : NetworkBehaviour
{
    [SerializeField] 
    private int itemId;
    
    private void OnTriggerEnter(Collider other)
    {
        if (IsServerStarted && other.TryGetComponentWithRigidbody(out ItemSystem itemSystem))
        {
            itemSystem.CollectedItems.Add(itemId);

            if (itemSystem.SelectedItem.Value == -1)
                itemSystem.SelectedItem.Value = itemId;
            
            ServerManager.Despawn(gameObject);
        }
    }
}