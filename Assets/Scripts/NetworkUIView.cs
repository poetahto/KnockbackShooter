using System;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Transporting;
using UnityEngine;

public class NetworkUIView : MonoBehaviour
{
    [SerializeField] 
    private Transform connectionViewParent;

    [SerializeField] 
    private ConnectionUIView connectionViewPrefab;

    private Dictionary<NetworkConnection, ConnectionUIView> _viewTable;

    private void Start()
    {
        _viewTable = new Dictionary<NetworkConnection, ConnectionUIView>();
        Game.Instance.Network.ServerManager.OnServerConnectionState += HandleServerChange;
        gameObject.SetActive(false);
    }

    private void HandleServerChange(ServerConnectionStateArgs state)
    {
        if (state.ConnectionState != LocalConnectionState.Started) 
            return;
        
        gameObject.SetActive(true);
        NetworkManager network = Game.Instance.Network;
        network.ServerManager.OnRemoteConnectionState += HandleRemoteClientChange;
        network.ServerManager.OnAuthenticationResult += HandleAuthenticationFinish;

        foreach (NetworkConnection connection in network.ServerManager.Clients.Values)
            CreateNewView(connection);
    }

    private void HandleAuthenticationFinish(NetworkConnection connection, bool successfullyAuthenticated)
    {
        if (successfullyAuthenticated)
            CreateNewView(connection);
    }

    private void HandleRemoteClientChange(NetworkConnection connection, RemoteConnectionStateArgs state)
    {
        if (state.ConnectionState == RemoteConnectionState.Stopped)
            RemoveView(connection);
    }
    
    private void CreateNewView(NetworkConnection connection)
    {
        ConnectionUIView instance = Instantiate(connectionViewPrefab, connectionViewParent);
        instance.BindTo(connection);
        _viewTable.Add(connection, instance);
    }

    private void RemoveView(NetworkConnection connection)
    {
        Destroy(_viewTable[connection].gameObject);
        _viewTable.Remove(connection);
    }
}