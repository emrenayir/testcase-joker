using System;
using EventBus;
using Game;
using Roulette;
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

    private void Awake() 
    {
        EventManager.Instance.RegisterEvent<GameStateChangeEvent>(OnGameStateChanged);
        EventManager.Instance.RegisterEvent<RouletteFinishedEvent>(OnRouletteFinished);
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
        isSpinning = true;
        int targetNumber = outcomeManager.GetTargetNumber();
        ball.StartRolling(targetNumber);
    }
}