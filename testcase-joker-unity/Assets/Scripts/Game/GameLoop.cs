using UnityEngine;
using System.Collections;
using Game;
using System;

/// <summary>
/// This is the main class that controls the game loop.
/// It is responsible for the game phases and the logic of the game.
/// </summary>
public class GameLoop : MonoBehaviour
{
    [SerializeField] private ParticleSystem winParticles;

    private GameState currentPhase;

    private EventBinding<BetPlacementConfirmedButtonEvent> betPlacementConfirmedBinding;
    private EventBinding<BetProcessingFinishedEvent> betProcessingFinishedBinding;

    void Awake()
    {
        betPlacementConfirmedBinding = new EventBinding<BetPlacementConfirmedButtonEvent>(OnBetPlacementConfirmed);
        EventBus<BetPlacementConfirmedButtonEvent>.Register(betPlacementConfirmedBinding);


        betProcessingFinishedBinding = new EventBinding<BetProcessingFinishedEvent>(OnBetProcessingFinished);
        EventBus<BetProcessingFinishedEvent>.Register(betProcessingFinishedBinding);
    }
    void OnDestroy()
    {
        EventBus<BetPlacementConfirmedButtonEvent>.UnRegister(betPlacementConfirmedBinding);
        EventBus<BetProcessingFinishedEvent>.UnRegister(betProcessingFinishedBinding);
    }

    void Start()
    {
        var playerSave = PlayerSave.Instance; //TODO: this is probably not a good way to do this

        SetPhase(GameState.InBet);
    }

    private void SetPhase(GameState newPhase, bool isWinner = false)
    {
        currentPhase = newPhase;

        // Raise event for state change
        EventBus<GameStateChangeEvent>.Raise(new GameStateChangeEvent { NewState = newPhase });

        switch (currentPhase)
        {
            case GameState.Running:
                EnterRunningPhase();
                break;
            case GameState.Finish:
                EnterFinishPhase(isWinner);
                break;
        }
    }
    private void EnterRunningPhase()
    {
        EventBus<OnTotalSpinsChangedEvent>.Raise(new OnTotalSpinsChangedEvent { });
    }

    private void EnterFinishPhase(bool isWinner)
    {
        if (isWinner)
        {
            EventBus<OnTotalWinsChangedEvent>.Raise(new OnTotalWinsChangedEvent { TotalWinsChangeAmount = 1 });
            winParticles.Play();
            SoundManager.Instance.PlaySound("Win");
        }
        else
        {
            SoundManager.Instance.PlaySound("Error");
        }
        
        SetPhase(GameState.InBet);
    }

    private void OnBetProcessingFinished(BetProcessingFinishedEvent @event)
    {
        SetPhase(GameState.Finish, @event.IsWinner);
    }
    private void OnBetPlacementConfirmed()
    {
        // Player has confirmed their chip selection, move to next phase
        SetPhase(GameState.Running);
    }
}