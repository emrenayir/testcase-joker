using UnityEngine;
using System.Collections.Generic;

public class ColumnBet : BetButton
{
    [SerializeField] private int columnIndex; // 0, 1, or 2 for the three columns
    
    private HashSet<int> columnNumbers = new HashSet<int>();
    
    protected override void Awake()
    {
        base.Awake();
        
        // Populate the column numbers
        for (int i = 0; i < 12; i++)
        {
            int number = columnIndex + 1 + (i * 3);
            columnNumbers.Add(number);
        }
    }
    
    public override BetType GetBetType()
    {
        return BetType.Column;
    }
    
    public override int GetCoveredNumbersCount()
    {
        return 12; // Column bet covers 12 numbers
    }
    
    public override bool IsWinner(int winningNumber)
    {
        if (winningNumber == 0)
        {
            return false; // Zero is not in any column
        }
        
        return columnNumbers.Contains(winningNumber);
    }
} 