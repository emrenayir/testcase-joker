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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartRoulette();
        }
    }

    void OnDisable()
    {
        onSpinCompleted = null;
    }

    public void InvokeSpinCompleted()
    {
        onSpinCompleted?.Invoke();
    }


    /// <summary>
    /// Starts the spin of the roulette wheel.
    /// </summary>
    public void StartRoulette()
    {
        Debug.Log("Starting roulette");
        if (isSpinning) return;
        
        isSpinning = true;
        int targetNumber = outcomeManager.GetTargetNumber();
        ball.StartRolling(targetNumber);
    }

    public int GetResult()
    {
        return outcomeManager.GetResult();
    }
}