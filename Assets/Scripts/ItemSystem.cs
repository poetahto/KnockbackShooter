using FishNet.Object;
using FishNet.Object.Synchronizing;

public class ItemSystem : NetworkBehaviour
{
    public readonly SyncHashSet<int> CollectedItems = new();
    public readonly SyncVar<int> SelectedItem = new(-1);

    [ServerRpc]
    public void RpcApplySelectItemInput(int itemId)
    {
        if (CollectedItems.Contains(itemId))
            SelectedItem.Value = itemId;
    }
    
    [ServerRpc]
    public void RpcApplyFireInput()
    {
        if (SelectedItem.Value != -1)
            Game.Instance.Settings.items[SelectedItem.Value].logic.HandleFire();
    }
}