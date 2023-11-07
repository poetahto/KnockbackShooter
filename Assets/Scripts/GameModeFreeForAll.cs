using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UniRx;
using UnityEngine;

// wait until min players have joined
// start countdown
// spawn everyone at random positions
// for each player: on player eliminated, decrement remaining. 
// If remaining == 1, game over.

public class GameModeFreeForAll : NetworkBehaviour
{
    public abstract class StateLogic
    {
        public GameModeFreeForAll Parent { get; set; }
        
        public virtual void OnClientEnter() {}
        public virtual void OnClientExit() {}
        public virtual void OnClientLogic() {}
        
        public virtual void OnServerEnter() {}
        public virtual void OnServerExit() {}
        public virtual void OnServerLogic() {}

        public virtual void OnClientGui()
        {
            GUILayout.Label("Test client");
        }

        public virtual void OnServerGui()
        {
            GUILayout.Label("Test server");
        }
    }

    public class WaitingLogic : StateLogic {}
    public class CountdownLogic : StateLogic {}
    public class PlayingLogic : StateLogic {}
    public class SuddenDeathLogic : StateLogic {}
    public class GameOverLogic : StateLogic {}
    
    public enum State
    {
        Waiting,
        Countdown,
        Playing,
        SuddenDeath,
        GameOver,
    }
    
    public readonly SyncVar<State> GameState = new();
    private Dictionary<State, StateLogic> _logicTable;

    private void Start()
    {
        _logicTable = new Dictionary<State, StateLogic>()
        {
            {State.Waiting, new WaitingLogic()},
            {State.Countdown, new CountdownLogic()},
            {State.Playing, new PlayingLogic()},
            {State.SuddenDeath, new SuddenDeathLogic()},
            {State.GameOver, new GameOverLogic()},
        };
        
        GameState
            .ObserveChanged()
            .SubscribeWithState(_logicTable, (data, logic) =>
            {
                if (data.AsServer)
                {
                    logic[data.Previous].OnServerExit();
                    logic[data.Next].OnServerEnter();
                }
                else
                {
                    logic[data.Previous].OnClientEnter();
                    logic[data.Next].OnClientExit();
                }
            });
        
        ImGuiWhiteboard.Instance.Register(DrawGUI).AddTo(this);
    }

    private void DrawGUI()
    {
        GUILayout.Label("[Free-For-All]");
        GUILayout.Label($"{GameState.Value.ToString()}");
        
        if (IsServerStarted)
        {
            GUILayout.Label("Server:");
            _logicTable[GameState.Value].OnServerGui();
        }

        if (IsClientStarted)
        {
            GUILayout.Label("Client:");
            _logicTable[GameState.Value].OnClientGui();
        }
    }

    private void Update()
    {
        if (IsClientStarted) _logicTable[GameState.Value].OnClientLogic();
        if (IsServerStarted) _logicTable[GameState.Value].OnServerLogic();
    }
}