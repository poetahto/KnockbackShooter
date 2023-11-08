namespace FreeForAll
{
    public abstract class FfaGameModeNetworkedState : NetworkedStateLogic
    {
        public FfaGameMode Parent { get; set; }
        protected FfaGameModeSettings Settings => Parent.Settings;
    }
}