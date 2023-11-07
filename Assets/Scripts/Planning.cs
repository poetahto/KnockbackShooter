using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // source-ish movement and air acceleration
    // movement parameters need to be easy to change at runtime (if you get hit, lower accel)
}

public class MovementHitStun : MonoBehaviour
{
    // lowers the player movement's acceleration when hit, gradually recovers
}

public class PlayerJumping : MonoBehaviour
{
    // typical feel-good jumping w/ coyote, buffer, should be very smooth
}

public class PlayerCollecting : MonoBehaviour
{
    // logic to make the player do stuff when they walk over a collectable (in this case, probably grab an item)
}

public class PlayerItems : MonoBehaviour
{
    // handles all actions that relate to held items
    // you can hold one item at a time?
    // you can control one item at a time
}

public class Collectable : MonoBehaviour
{
    // something that can execute logic when its walked over
    // generally is one-shot as well
    // should be synced so only one person can grab it
}

public class ItemTable
{
    // all references to items go through this table as index ids to the list
    // really should only be one item table for the whole game - its just a SO for easy extending
}

public class ItemDefinition
{
    // dropped item object (when its available to be picked up)
    // ui display info (ammo count UI, ect)
    // held item object (when someone is wielding it, firing it ect)
    // logic (what to do when its actually fired, grabbed, ect)
}

public class GameMode
{
    // changes drastically the gameplay
    // handles setting up the game and shutting down the game
    // - determines when the game should end, distributing rewards
    // doesn't really manage the player list, but is aware of it and many other details
    // can execute logic at important game callbacks (update, start, ect.)
}

public class GameState
{
    // one per game
    // always present in a game
    // has a game mode, item table
}

public class Application
{
    // truly global data
    // settings / config info
    // music player
}