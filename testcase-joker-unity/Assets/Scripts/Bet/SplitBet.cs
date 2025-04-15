using UnityEngine;

namespace Bet
{
    public class SplitBet : BetButton
    {
        [SerializeField] private int firstNumber;
        private int[] numbers = new int[2];
    
        protected override void Start()
        {
            base.Start();
            numbers[0] = firstNumber;
            numbers[1] = firstNumber + 3;
        }
    
        public override BetType GetBetType()
        {
            return BetType.Split;
        }
    
        public override int GetCoveredNumbersCount()
        {
            return 2; // Split bet covers 2 numbers
        }
    
        public override bool IsWinner(int winningNumber)
        {
            return System.Array.IndexOf(numbers, winningNumber) != -1;
        }
    }
} 