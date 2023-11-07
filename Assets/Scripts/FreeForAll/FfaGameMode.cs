using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;

// wait until min players have joined
// start countdown
// spawn everyone at random positions
// for each player: on player eliminated, decrement remaining. 
// If remaining == 1, game over.

namespace FreeForAll
{
    public class FfaGameMode : NetworkBehaviour
    {
        public enum State
        {
            Waiting,
            Countdown,
            Playing,
            SuddenDeath,
            GameOver,
        }
    
        public WaitingLogic waitingLogic;
        public CountdownLogic countdownLogic;
        public PlayingLogic playingLogic;
        public SuddenDeathLogic suddenDeathLogic;
        public GameOverLogic gameOverLogic;
    
        public readonly SyncVar<State> GameState = new();
        public readonly SyncTimer CountdownTimer = new();
        public readonly SyncTimer GameTimer = new();
        private Dictionary<State, StateLogic> _logicTable;
        
        public FfaGameModeSettings Settings { get; set; }

        public override void OnStartNetwork()
        {
            _logicTable = new Dictionary<State, StateLogic>()
            {
                {State.Waiting, waitingLogic},
                {State.Countdown, countdownLogic},
                {State.Playing, playingLogic},
                {State.SuddenDeath, suddenDeathLogic},
                {State.GameOver, gameOverLogic},
            };

            Settings = Addressables.LoadAssetAsync<FfaGameModeSettings>("ffa_settings").WaitForCompletion();

            foreach (StateLogic logic in _logicTable.Values)
            {
                logic.Parent = this;
                logic.Initialize();
            }
        
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
                
                    logic[data.Previous].OnEnter();
                    logic[data.Next].OnExit();
                });
        
            ImGuiWhiteboard.Instance.Register(DrawGUI).AddTo(this);
        }

        public override void OnStartClient()
        {
            foreach (StateLogic logic in _logicTable.Values)
                logic.InitializeClient();
        }

        public override void OnStartServer()
        {
            foreach (StateLogic logic in _logicTable.Values)
                logic.InitializeServer();
        }

        private void DrawGUI()
        {
            GUILayout.Label("[GAME MODE: Free-For-All]");
            GUILayout.Label($"Current state: {GameState.Value.ToString()}");
        
            if (IsServerStarted) _logicTable[GameState.Value].OnServerGui();
            if (IsClientStarted) _logicTable[GameState.Value].OnClientGui();
            _logicTable[GameState.Value].OnGui();
        }

        private void Update()
        {
            if (IsClientStarted) _logicTable[GameState.Value].OnClientLogic();
            if (IsServerStarted) _logicTable[GameState.Value].OnServerLogic();
            _logicTable[GameState.Value].OnLogic();
        }
    }
}