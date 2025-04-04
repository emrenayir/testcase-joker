using UnityEngine;

public class SixLineBet : BetButton
{
    [SerializeField] private int startNumber; // The first number in the six line
    
    private int[] numbers = new int[6];
    
    protected override void Awake()
    {
        base.Awake();
        // Six line bet covers two adjacent rows of numbers
        numbers[0] = startNumber;
        numbers[1] = startNumber + 1;
        numbers[2] = startNumber + 2;
        numbers[3] = startNumber + 3;
        numbers[4] = startNumber + 4;
        numbers[5] = startNumber + 5;
    }
    
    public override BetType GetBetType()
    {
        return BetType.SixLine;
    }
    
    public override int GetCoveredNumbersCount()
    {
        return 6; // Six line bet covers 6 numbers
    }
    
    public override bool IsWinner(int winningNumber)
    {
        return System.Array.IndexOf(numbers, winningNumber) != -1;
    }
} 