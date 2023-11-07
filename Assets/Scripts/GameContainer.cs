using UnityEngine;

/// <summary>
/// MonoBehaviour hook for the Game instance.
/// Should be loaded once in the "Entrypoint" scene.
/// </summary>
public class GameContainer : MonoBehaviour
{
    [SerializeField]
    private GameSettings gameSettings;

    private Game _game;

    private void Awake()
    {
        _game = new Game(gameSettings);
    }

    private void OnDestroy()
    {
        _game?.Dispose();
    }
}