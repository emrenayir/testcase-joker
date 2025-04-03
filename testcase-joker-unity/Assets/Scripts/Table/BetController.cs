using System.Collections.Generic;
using UnityEngine;

public class BetController : MonoBehaviour
{
    [SerializeField] private float chipValue = 1f;
    
    private List<BetButton> activeBets = new List<BetButton>();
    private float totalBetAmount = 0f;
    
    
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
    
    public void ClearAllBets()
    {
        activeBets.Clear();
        totalBetAmount = 0f;
    }
} 