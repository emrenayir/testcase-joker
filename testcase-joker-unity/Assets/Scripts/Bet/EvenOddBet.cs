using UnityEngine;

public class EvenOddBet : BetButton
{
    [SerializeField] private bool isEven;
    
    public override BetType GetBetType()
    {
        return BetType.EvenOdd;
    }
    
    public override int GetCoveredNumbersCount()
    {
        return 18; // Even/Odd bet covers 18 numbers
    }
    
    public override bool IsWinner(int winningNumber)
    {
        if (winningNumber == 0)
        {
            return false; // Zero is neither even nor odd in roulette
        }
        
        bool numberIsEven = winningNumber % 2 == 0;
        return isEven == numberIsEven;
    }
    
    public override float CalculatePayout(float betAmount)
    {
        return betAmount * GetPayout();
    }
} 