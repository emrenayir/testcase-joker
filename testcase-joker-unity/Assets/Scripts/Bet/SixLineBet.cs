using UnityEngine;

namespace Bet
{
    public class SixLineBet : BetButton
    {
        [SerializeField] private int startNumber; // The first number in the six line
    
        private int[] numbers = new int[6];
    
        protected override void Start()
        {
            base.Start();
            // Six line bet covers two adjacent rows of numbers
            for (int i = 0; i < 6; i++)
            {
                numbers[i] = startNumber + i;
            }
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
} 