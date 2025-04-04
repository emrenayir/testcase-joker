using UnityEngine;

public class StraightBet : BetButton
{
    [SerializeField] private int number;
    
    public override BetType GetBetType()
    {
        return BetType.Straight;
    }
    
    public override int GetCoveredNumbersCount()
    {
        return 1; // Straight bet covers 1 number
    }
    
    public override bool IsWinner(int winningNumber)
    {
        return number == winningNumber;
    }
} 