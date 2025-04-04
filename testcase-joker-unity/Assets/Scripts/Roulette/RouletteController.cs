using System;
using UnityEngine;

/// <summary>
/// This script is responsible for controlling the roulette wheel and ball.
/// It also handles the outcome of the spin.
/// </summary>
public class RouletteController : MonoBehaviour
{
    [SerializeField] private RouletteWheel wheel;
    [SerializeField] private RouletteBall ball;
    [SerializeField] private RouletteOutcomeManager outcomeManager;
    
    private bool isSpinning = false;

    private Action onSpinCompleted;

    private void Awake() 
    {
        onSpinCompleted += () => 
        {
            Debug.Log("Spin completed");
            isSpinning = false;
        };
    }

    void OnDisable()
    {
        onSpinCompleted = null;
    }

    public void InvokeSpinCompleted()
    {
        onSpinCompleted?.Invoke();
    }


    //TODO: this is for testing and will be removed later
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
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