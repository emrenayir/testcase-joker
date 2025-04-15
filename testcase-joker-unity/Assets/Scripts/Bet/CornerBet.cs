using UnityEngine;

public class CornerBet : BetButton
{
    [SerializeField] private int startNumber; // Bottom-left number in the corner
    
    private int[] numbers = new int[4];
    
    protected override void Start()
    {
        base.Start();
        // For a corner bet, we need to handle the 4 adjacent numbers
        for (int i = 0; i < 4; i++) {
            numbers[i] = startNumber + (i < 2 ? i : i + 1);
        }
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