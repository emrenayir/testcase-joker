using UnityEngine;

public class SplitBet : BetButton
{
    [SerializeField] private int[] numbers = new int[2];
    
    public override BetType GetBetType()
    {
        return BetType.Split;
    }
    
    public override int GetCoveredNumbersCount()
    {
        return 2; // Split bet covers 2 numbers
    }
    
    public override bool IsWinner(int winningNumber)
    {
        return System.Array.IndexOf(numbers, winningNumber) != -1;
    }
} 