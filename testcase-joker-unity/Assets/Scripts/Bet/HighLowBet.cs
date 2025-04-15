using UnityEngine;

namespace Bet
{
    public class HighLowBet : BetButton
    {
        [SerializeField] private bool isHigh;
    
        public override BetType GetBetType()
        {
            return BetType.HighLow;
        }
    
        public override int GetCoveredNumbersCount()
        {
            return 18; // High/Low bet covers 18 numbers
        }
    
        public override bool IsWinner(int winningNumber)
        {
            if (winningNumber == 0)
            {
                return false; // Zero is neither high nor low
            }
        
            bool numberIsHigh = winningNumber >= 19 && winningNumber <= 36;
            return isHigh == numberIsHigh;
        }
    
        public override int CalculatePayout(int betAmount)
        {
            return (int)(betAmount * GetPayout());
        }
    }
} 