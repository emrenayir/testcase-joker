using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is the controller for the bet.
/// It handles the bet placement and the result processing.
/// </summary>
public class BetController : MonoBehaviour
{
    [SerializeField] private List<BetButton> betButtons;
    [SerializeField] private ChipSelectionController chipSelectionController;

    [SerializeField] private UserMoney userMoney;
    private List<BetButton> activeBets = new List<BetButton>();

     public event Action OnBetRemoved;

    /// <summary>
    /// Add bet to the necessary place
    /// </summary>
    /// <param name="betButton">The bet button that was clicked</param>
    /// 

    void Awake()
    {
        foreach (var betButton in betButtons)
        {
            betButton.OnBetPlaced += OnBetPlaced;
            betButton.SetBetButton(chipSelectionController, this);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ClearAllBets();
            OnBetRemoved?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            ProcessResult(1);
        }
    }
    void OnDisable()
    {
        if (betButtons == null) return;

        foreach (var betButton in betButtons)
        {
            if (betButton != null)
            {
                betButton.OnBetPlaced -= OnBetPlaced;
            }
        }
    }

    public void OnBetPlaced(BetButton betButton)
    {

        userMoney.PlaceBet(betButton.TotalChipValue);

        //Check if the bet is already in the list
        if (activeBets.Contains(betButton))
        {
            Debug.Log($"Bet {betButton.GetBetType()} $ {betButton.gameObject.name} is already in the list");
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

        foreach (var bet in activeBets)
        {
            Debug.Log($"Bet {bet.GetBetType()} $ {bet.gameObject.name}");
            bool isWinner = bet.IsWinner(winningNumber);
            bet.ShowWinningStatus(isWinner);

            if (isWinner)
            {
                Debug.Log($"Bet {bet.GetBetType()} $ {bet.gameObject.name} is a winner. Total chip value: {bet.TotalChipValue}");
                totalWinnings += bet.CalculatePayout(bet.TotalChipValue);
            }
        }

        userMoney.ProcessPayment(totalWinnings);

        Debug.Log($"Winning number: {winningNumber}. Total winnings: {totalWinnings}");
    }

    /// <summary>
    /// Clear all bets
    /// Chip vallue will be added later 
    /// </summary>
    public void ClearAllBets()
    {
        activeBets.Clear();
    }
}