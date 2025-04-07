using UnityEngine;

public class StreetBet : BetButton
{
    [SerializeField] private int startNumber; // The first number in the street
    
    private int[] numbers = new int[3];
    
    protected override void Start()
    {
        base.Start();
        numbers[0] = startNumber;
        numbers[1] = startNumber + 1;
        numbers[2] = startNumber + 2;
    }
    
    public override BetType GetBetType()
    {
        return BetType.Street;
    }
    
    public override int GetCoveredNumbersCount()
    {
        return 3; // Street bet covers 3 numbers
    }
    
    public override bool IsWinner(int winningNumber)
    {
        return System.Array.IndexOf(numbers, winningNumber) != -1;
    }
} 