using UnityEngine;

[CreateAssetMenu]
public class TestingItemLogic : ItemLogic
{
    public string message;
    
    public override void HandleFire()
    {
        Debug.Log(message);
    }
}