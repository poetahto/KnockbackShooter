namespace FreeForAll
{
    public abstract class StateLogic
    {
        public FfaGameMode Parent { get; set; }
        protected FfaGameModeSettings Settings => Parent.Settings;
        
        
        public virtual void Initialize() {}
        public virtual void OnEnter() {}
        public virtual void OnExit() {}
        public virtual void OnLogic() {}
        public virtual void OnGui() {}
        
        public virtual void InitializeClient() {}
        public virtual void OnClientEnter() {}
        public virtual void OnClientExit() {}
        public virtual void OnClientLogic() {}
        public virtual void OnClientGui() {}
        
        public virtual void InitializeServer() {}
        public virtual void OnServerEnter() {}
        public virtual void OnServerExit() {}
        public virtual void OnServerLogic() {}
        public virtual void OnServerGui() {}
    }
}