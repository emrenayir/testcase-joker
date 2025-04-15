using UnityEngine;

namespace Bet
{
    public class DozenBet : BetButton
    {
        [SerializeField] private int dozenIndex; // 0, 1, or 2 for the three dozens
    
        private int minNumber;
        private int maxNumber;
    
        protected override void Start()
        {
            base.Start();
        
            // Set the range for this dozen
            minNumber = (dozenIndex * 12) + 1;
            maxNumber = (dozenIndex + 1) * 12;
        }
    
        public override BetType GetBetType()
        {
            return BetType.Dozen;
        }
    
        public override int GetCoveredNumbersCount()
        {
            return 12; // Dozen bet covers 12 numbers
        }
    
        public override bool IsWinner(int winningNumber)
        {
            if (winningNumber == 0)
            {
                return false; // Zero is not in any dozen
            }
        
            return winningNumber >= minNumber && winningNumber <= maxNumber;
        }
    }
} 