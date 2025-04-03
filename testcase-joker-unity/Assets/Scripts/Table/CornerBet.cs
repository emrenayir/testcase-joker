using UnityEngine;

public class CornerBet : BetButton
{
    [SerializeField] private int startNumber; // Bottom-left number in the corner
    
    private int[] numbers = new int[4];
    
    protected override void Awake()
    {
        base.Awake();
        // For a corner bet, we need to handle the 4 adjacent numbers
        numbers[0] = startNumber;
        numbers[1] = startNumber + 1;
        numbers[2] = startNumber + 3;
        numbers[3] = startNumber + 4;
    }
    
    public override BetType GetBetType()
    {
        return BetType.Corner;
    }
    
    public override int GetCoveredNumbersCount()
    {
        return 4; // Corner bet covers 4 numbers
    }
    
    public override bool IsWinner(int winningNumber)
    {
        return System.Array.IndexOf(numbers, winningNumber) != -1;
    }
} 