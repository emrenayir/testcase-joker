using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is the controller for the bet.
/// It handles the bet placement and the result processing.
/// </summary>
public class BetController : MonoBehaviour
{
    public event Action OnBetRemoved;
    public event Action<BetButton> OnBetPlaced;
    public bool IsBettingEnabled = true;

    [SerializeField] private ChipSelectionController chipSelectionController;
    [SerializeField] private UserMoney userMoney;
    [SerializeField] private List<BetButton> betButtons;

    private List<BetButton> activeBets = new List<BetButton>();


    void Awake()
    {
        OnBetPlaced += HandleBet;

        foreach (var betButton in betButtons)
        {
            betButton.SetBetButton(chipSelectionController, this, userMoney);
        }
    }


    public void InvokePlaceBet(BetButton betButton)
    {
        OnBetPlaced?.Invoke(betButton);
    }

    private void HandleBet(BetButton betButton)
    {
        userMoney.PlaceBet(ChipHelper.GetChipValue(chipSelectionController.SelectedChipValue));


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
    /// <param name="winningNumber">The winning number</param>
    public void ProcessResult(int winningNumber)
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
                Debug.Log($"Bet {bet.GetBetType()} $ {bet.gameObject.name} is a winner. Total chip value: {bet.TotalChipValue}");
                totalWinnings += bet.CalculatePayout(bet.TotalChipValue);
            }
            else
            {
                lostBets += bet.TotalChipValue;
            }
        }

        userMoney.ProcessPayment(totalWinnings, lostBets);

        Debug.Log($"Winning number: {winningNumber}. Total winnings: {totalWinnings}");
    }

    /// <summary>
    /// Clear all bets
    /// Chip vallue will be added later 
    /// </summary>
    public void ClearAllBets()
    {
        activeBets.Clear();
        OnBetRemoved?.Invoke();
    }
}