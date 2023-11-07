using FishNet.Connection;
using TMPro;
using UnityEngine;

public class ConnectionUIView : MonoBehaviour
{
    [SerializeField] 
    private TMP_Text idText;
    
    public void BindTo(NetworkConnection connection)
    {
        idText.text = $"Player {connection.ClientId}";
    }
}