using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is the controller for the bet.
/// It handles the bet placement and the result processing.
/// </summary>
public class BetController : MonoBehaviour
{
    [SerializeField] private float chipValue = 1f;
    
    private List<BetButton> activeBets = new List<BetButton>();
    private float totalBetAmount = 0f;
    
    /// <summary>
    /// Add bet to the necessary place
    /// </summary>
    /// <param name="betButton">The bet button that was clicked</param>
    public void OnBetPlaced(BetButton betButton)
    {
        if (activeBets.Contains(betButton))
        {
            // Remove bet
            activeBets.Remove(betButton);
            totalBetAmount -= chipValue;
        }
        else
        {
            // Add bet
            activeBets.Add(betButton);
            totalBetAmount += chipValue;
        }
        
        Debug.Log($"Total bet: {totalBetAmount}");
    }
    
    /// <summary>
    /// Process the result of the bet
    /// </summary>
    /// <param name="winningNumber">The winning number</param>
    public void ProcessResult(int winningNumber)
    {
        float totalWinnings = 0f;
        
        foreach (var bet in activeBets)
        {
            bool isWinner = bet.IsWinner(winningNumber);
            bet.ShowWinningStatus(isWinner);
            
            if (isWinner)
            {
                float winnings = bet.CalculatePayout(chipValue);
                totalWinnings += winnings;
            }
        }
        
        Debug.Log($"Winning number: {winningNumber}. Total winnings: {totalWinnings}");
    }
    
    /// <summary>
    /// Clear all bets
    /// Chip vallue will be added later 
    /// </summary>
    public void ClearAllBets()
    {
        activeBets.Clear();
        totalBetAmount = 0f;
    }
} 