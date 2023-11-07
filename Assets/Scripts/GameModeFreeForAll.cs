using FishNet.Object;

public class GameModeFreeForAll : NetworkBehaviour
{
    private void Start()
    {
        // wait until min players have joined
        // start countdown
        // spawn everyone at random positions
        // for each player: on player eliminated, decrement remaining. 
        // If remaining == 1, game over.
    }

    private void Update()
    {
        // count down from maximum time: if we hit sudden death, make things crazy.
    }

    private void HandleGameOver()
    {
        // show the game over details, award points ect.
    } 
}