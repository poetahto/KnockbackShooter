using FreeForAll;
using TMPro;
using UniRx;
using UnityEngine;

public class FreeForAllUIView : MonoBehaviour
{
    [SerializeField] 
    private TMP_Text stateDisplay;
    
    private void Start()
    {
        var gameMode = FindAnyObjectByType<FfaGameMode>();
        
        if (!gameMode) 
            return;
        
        gameMode.GameState.ObserveChanged()
            .Select(data => data.Next.ToString())
            .SubscribeToText(stateDisplay)
            .AddTo(this);
    }
}