using System;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FreeForAll.GameStates;
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
    
        [SerializeField]
        private WaitingLogic waitingLogic;
        
        [SerializeField]
        private CountdownLogic countdownLogic;
        
        [SerializeField]
        private PlayingLogic playingLogic;
        
        [SerializeField]
        private SuddenDeathLogic suddenDeathLogic;
        
        [SerializeField]
        private GameOverLogic gameOverLogic;
        
        public readonly SyncVar<State> GameState = new();
        public readonly SyncTimer CountdownTimer = new();
        public readonly SyncTimer GameTimer = new();
        
        private NetworkedStateMachine<State> _fsm;
        
        public FfaGameModeSettings Settings { get; set; }

        private void Awake()
        {
            waitingLogic.Parent = this;
            countdownLogic.Parent = this;
            playingLogic.Parent = this;
            suddenDeathLogic.Parent = this;
            gameOverLogic.Parent = this;
            
            _fsm = new NetworkedStateMachine<State>(new Dictionary<State, NetworkedStateLogic>()
            {
                { State.Waiting, waitingLogic },
                { State.Countdown, countdownLogic },
                { State.Playing, playingLogic },
                { State.SuddenDeath, suddenDeathLogic },
                { State.GameOver, gameOverLogic },
            });
            
            Settings = Addressables.LoadAssetAsync<FfaGameModeSettings>("ffa_settings").WaitForCompletion();
        }

        private void OnDestroy()
        {
            _fsm.Dispose();
        }

        public override void OnStartNetwork() => _fsm.OnStartNetwork(GameState);
        public override void OnStartClient() => _fsm.OnStartClient(GameState);
        public override void OnStartServer() => _fsm.OnStartServer(GameState);
    }
}