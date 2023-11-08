using System;
using System.Collections.Generic;
using FishNet.Object.Synchronizing;
using UniRx;
using UnityEngine;
using Util;

public class NetworkedStateMachine<T> : IDisposable
{
    private readonly IDictionary<T, NetworkedStateLogic> _logicTable;
    private IDisposable _networkCallbacks;
    private IDisposable _clientCallbacks;
    private IDisposable _serverCallbacks;
    
    public NetworkedStateMachine(IDictionary<T, NetworkedStateLogic> logicTable)
    {
        _logicTable = logicTable;
    }
    
    public void OnStartNetwork(SyncVar<T> _syncedState)
    {
        var disposables = new CompositeDisposable();
        
        foreach (NetworkedStateLogic state in _logicTable.Values)
            state.Initialize();

        _syncedState.ObserveChanged().SubscribeWithState(_logicTable, (data, logic) =>
        {
            if (data.AsServer)
            {
                logic[data.Previous].OnServerExit();
                logic[data.Next].OnServerEnter();
            }
            else // Run as the client
            {
                logic[data.Previous].OnClientExit();
                logic[data.Next].OnClientEnter();
            }

            logic[data.Previous].OnExit();
            logic[data.Next].OnEnter();
        });
        
        _logicTable[_syncedState.Value].OnEnter();
        ImGuiWhiteboard.Instance.Register(() => DrawGUI(_syncedState)).AddTo(disposables);
        
        Observable
            .EveryUpdate()
            .SubscribeWithState2(_logicTable, _syncedState, (_, logic, state) =>
            {
                logic[state.Value].OnLogic();
            })
            .AddTo(disposables);

        _networkCallbacks = disposables;
    }

    public void OnStartClient(SyncVar<T> _syncedState)
    {
        var disposables = new CompositeDisposable();
        
        foreach (NetworkedStateLogic state in _logicTable.Values)
            state.InitializeClient();

        _logicTable[_syncedState.Value].OnClientEnter();
        ImGuiWhiteboard.Instance.Register(() => DrawClientGUI(_syncedState)).AddTo(disposables);
        
        Observable
            .EveryUpdate()
            .SubscribeWithState2(_logicTable, _syncedState, (_, logic, state) =>
            {
                logic[state.Value].OnClientLogic();
            })
            .AddTo(disposables);

        _clientCallbacks = disposables;
    }

    public void OnStartServer(SyncVar<T> _syncedState)
    {
        var disposables = new CompositeDisposable();
        
        foreach (NetworkedStateLogic state in _logicTable.Values)
            state.InitializeServer();

        _logicTable[_syncedState.Value].OnServerEnter();
        ImGuiWhiteboard.Instance.Register(() => DrawServerGUI(_syncedState)).AddTo(disposables);
        
        Observable
            .EveryUpdate()
            .SubscribeWithState2(_logicTable, _syncedState, (_, logic, state) =>
            {
                logic[state.Value].OnServerLogic();
            })
            .AddTo(disposables);

        _serverCallbacks = disposables;
    }

    private void DrawGUI(SyncVar<T> _syncedState)
    {
        GUILayout.Label($"Current state: {_syncedState.Value.ToString()}");
        _logicTable[_syncedState.Value].OnGui();
    }

    private void DrawServerGUI(SyncVar<T> _syncedState) => _logicTable[_syncedState.Value].OnServerGui();
    private void DrawClientGUI(SyncVar<T> _syncedState) => _logicTable[_syncedState.Value].OnClientGui();

    public void Dispose()
    {
        _networkCallbacks?.Dispose();
        _clientCallbacks?.Dispose();
        _serverCallbacks?.Dispose();
    }
}