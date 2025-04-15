using System.Collections;
using System.Collections.Generic;
using Chips;
using EventBus;
using Game;
using UnityEngine;
using User;

namespace Bet
{
    /// <summary>
    /// This class is the controller for the bet.
    /// It handles the bet placement and the result processing.
    /// </summary>
    public class BetController : MonoBehaviour
    {
        public bool IsBettingEnabled = true;

        [SerializeField] private ChipSelectionController chipSelectionController;
        [SerializeField] private List<BetButton> betButtons;

        private List<BetButton> activeBets = new List<BetButton>();
        private int winningNumber;

        void Awake()
        {
            foreach (var betButton in betButtons)
            {
                betButton.SetBetButton(chipSelectionController, this);
            }

            EventManager.Instance.RegisterEvent<ResetBetButtonEvent>(ClearAllBets);
            EventManager.Instance.RegisterEvent<GameStateChangeEvent>(OnGameStateChanged);
            EventManager.Instance.RegisterEvent<RouletteFinishedEvent>(OnRouletteFinished);
            EventManager.Instance.RegisterEvent<LoadSavedBetsEvent>(OnLoadSavedBets);
        }

        private void OnRouletteFinished(RouletteFinishedEvent @event)
        {
            winningNumber = @event.WinningNumber;
            StartCoroutine(ProcessResult());
        }

        private void OnGameStateChanged(GameStateChangeEvent @event)
        {
            IsBettingEnabled = @event.NewState == GameState.InBet;
        }
    
        private void OnLoadSavedBets(LoadSavedBetsEvent @event)
        {
            List<BetData> savedBets = @event.SavedBets;
        
            if (savedBets != null && savedBets.Count > 0)
            {
                foreach (var betData in savedBets)
                {
                    BetButton betButton = betButtons.Find(b => b.gameObject.name == betData.BetButtonName);
                    if (betButton != null)
                    {
                        betButton.LoadBetData(betData);
                        if (!activeBets.Contains(betButton))
                        {
                            activeBets.Add(betButton);
                        }
                    }
                }
            }
        }

        void OnApplicationQuit()
        {
            // Save bets when the game is closed
            SaveBets();
        }

        void OnApplicationPause(bool pauseStatus)
        {
            // Save bets when the game is paused (mobile or tab switching)
            if (pauseStatus)
            {
                SaveBets();
            }
        }

        private void SaveBets()
        {
            if (activeBets.Count > 0)
            {
                EventManager.Instance.Raise(new SaveBetsEvent { ActiveBets = activeBets });
            }
        }

        public void InvokePlaceBet(BetButton betButton)
        {
            HandleBet(betButton);
        }

        private void HandleBet(BetButton betButton)
        {
            int chipValue = ChipHelper.GetChipValue(chipSelectionController.SelectedChipValue);
            EventManager.Instance.Raise(new PlaceBetEvent { ChipValue = chipValue });

            //Check if the bet is already in the list
            if (activeBets.Contains(betButton))
            {
                return;
            }
            activeBets.Add(betButton);
        }

        /// <summary>
        /// Process the result of the bet
        /// </summary>
        public IEnumerator ProcessResult()
        {
            int totalWinnings = 0;
            int lostBets = 0;

            foreach (var bet in activeBets)
            {
                //Check if the bet is a winner
                bool isWinner = bet.IsWinner(winningNumber);

                //Indicate the winning status of the bet
                bet.ShowWinningStatus(isWinner);

                //Calculate the payout
                if (isWinner)
                {
                    totalWinnings += bet.CalculatePayout(bet.TotalChipValue);
                }
                else
                {
                    lostBets += bet.TotalChipValue;
                }
            }

            EventManager.Instance.Raise(new ProcessPaymentEvent { Payment = totalWinnings, LostBets = lostBets });

            // Wait for a moment before starting a new round
            yield return new WaitForSeconds(3f);

            EventManager.Instance.Raise(new BetProcessingFinishedEvent { IsWinner = totalWinnings - lostBets > 0 });
        
        
            ClearAllBets();
        }

        /// <summary>
        /// Clear all bets
        /// </summary>
        public void ClearAllBets()
        {
            foreach (var bet in activeBets)
            {
                bet.ResetChips();
            }
            activeBets.Clear();
        
            EventManager.Instance.Raise(new ClearSavedBetsEvent());
        }


        /// <summary>
        /// Add a bet to the active bets list if it's not already there
        /// </summary>
        public void AddActiveBet(BetButton betButton)
        {
            if (!activeBets.Contains(betButton))
            {
                activeBets.Add(betButton);
            }
        }

        /// <summary>
        /// Get the list of bet buttons
        /// </summary>
        public List<BetButton> GetBetButtons()
        {
            return betButtons;
        }
    }
}