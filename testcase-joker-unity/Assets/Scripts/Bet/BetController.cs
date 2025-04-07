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
    [SerializeField] private PlayerSave playerSave;

    private List<BetButton> activeBets = new List<BetButton>();

    void Awake()
    {
        OnBetPlaced += HandleBet;

        foreach (var betButton in betButtons)
        {
            betButton.SetBetButton(chipSelectionController, this, userMoney);
        }
    }
    
    void Start()
    {
        // Load saved bets when the game starts
        if (playerSave != null)
        {
            playerSave.LoadBets(this);
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
        if (playerSave != null)
        {
            if (activeBets.Count > 0)
            {
                playerSave.SaveBets(activeBets);
            }
            else
            {
                // Even if there are no active bets, save the player money
                playerSave.SavePlayerMoneyOnly();
            }
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
        
        // Save the player's money after processing results
        if (playerSave != null)
        {
            playerSave.SavePlayerMoneyOnly();
        }
        
        // After processing the round, clear the active bets and saved data
        // This should be called by a "New Round" function in your game
        // If you want automatic clearing after each round, uncomment the next line:
        // ClearAllBets();
    }

    /// <summary>
    /// Clear all bets
    /// </summary>
    public void ClearAllBets()
    {
        activeBets.Clear();
        OnBetRemoved?.Invoke();
        
        // Clear saved bets data as well
        if (playerSave != null)
        {
            playerSave.ClearSavedBets();
        }
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
    
    /// <summary>
    /// Get the list of active bets
    /// </summary>
    public List<BetButton> GetActiveBets()
    {
        return activeBets;
    }

    /// <summary>
    /// Reset the entire game state
    /// </summary>
    public void ResetGame()
    {
        // Clear all bets (which also clears saved data)
        ClearAllBets();
        
        // Reset player money to default value if needed
        if (userMoney != null)
        {
            userMoney.SetMoney(1000); // Reset to default starting money
        }
        
        // Save the reset state
        if (playerSave != null)
        {
            playerSave.SavePlayerMoneyOnly();
        }
        
        Debug.Log("Game has been reset. All bets cleared and money reset.");
    }
}