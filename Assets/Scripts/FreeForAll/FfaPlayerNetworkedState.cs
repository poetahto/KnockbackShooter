namespace FreeForAll
{
    public abstract class FfaPlayerNetworkedState : NetworkedStateLogic
    {
        public FfaPlayer Parent { get; set; }
        public FfaGameModeSettings Settings => Parent.Settings;
    }
}