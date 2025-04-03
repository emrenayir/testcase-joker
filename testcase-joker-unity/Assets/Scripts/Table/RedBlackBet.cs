using UnityEngine;
using System.Collections.Generic;

public class RedBlackBet : BetButton
{
    [SerializeField] private bool isRed;
    
    // Red numbers in roulette
    private static readonly HashSet<int> redNumbers = new HashSet<int> 
    { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 };
    
    public override BetType GetBetType()
    {
        return BetType.RedBlack;
    }
    
    public override int GetCoveredNumbersCount()
    {
        return 18; // Red/Black bet covers 18 numbers
    }
    
    public override bool IsWinner(int winningNumber)
    {
        if (winningNumber == 0)
        {
            return false; // Zero is neither red nor black
        }
        
        bool isNumberRed = redNumbers.Contains(winningNumber);
        return isRed ? isNumberRed : !isNumberRed;
    }
} 