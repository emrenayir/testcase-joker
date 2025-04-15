using System;
using Game;
using UnityEngine;

/// <summary>
/// This script is responsible for controlling the roulette wheel and ball.
/// It also handles the outcome of the spin.
/// </summary>
public class RouletteController : MonoBehaviour
{
    [SerializeField] private RouletteBall ball;
    [SerializeField] private RouletteOutcomeManager outcomeManager;
    
    private bool isSpinning = false;

    private EventBinding<RouletteFinishedEvent> rouletteFinishedBinding;

    private EventBinding<GameStateChangeEvent> gameStateBinding;
    private void Awake() 
    {
        gameStateBinding = new EventBinding<GameStateChangeEvent>(OnGameStateChanged);
        EventBus<GameStateChangeEvent>.Register(gameStateBinding);

        rouletteFinishedBinding = new EventBinding<RouletteFinishedEvent>(OnRouletteFinished);
        EventBus<RouletteFinishedEvent>.Register(rouletteFinishedBinding);
    }

    private void OnDestroy()
    {
        EventBus<GameStateChangeEvent>.UnRegister(gameStateBinding);
        EventBus<RouletteFinishedEvent>.UnRegister(rouletteFinishedBinding);
    }

    private void OnRouletteFinished(RouletteFinishedEvent @event)
    {
        isSpinning = false;
    }

    private void OnGameStateChanged(GameStateChangeEvent @event)
    {
        if(@event.NewState == GameState.Running)
        {
            StartRoulette();
        }
    }

    /// <summary>
    /// Starts the spin of the roulette wheel.
    /// </summary>
    public void StartRoulette()
    {
        if (isSpinning) return;
        

        EventBus<RouletteStartedEvent>.Raise(new RouletteStartedEvent());
        isSpinning = true;
        int targetNumber = outcomeManager.GetTargetNumber();
        ball.StartRolling(targetNumber);
    }
}