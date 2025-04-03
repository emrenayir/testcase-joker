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

    public Action OnSpinCompleted;

    private void Awake() 
    {
        OnSpinCompleted += () => 
        {
            Debug.Log("Spin completed");
            isSpinning = false;
        };
    }

    public void InvokeSpinCompleted()
    {
        OnSpinCompleted?.Invoke();
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