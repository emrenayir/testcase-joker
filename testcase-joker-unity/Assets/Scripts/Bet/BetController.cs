using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

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

    private EventBinding<ResetBetButtonEvent> resetBetBinding;
    private EventBinding<GameStateChangeEvent> gameStateBinding;
    private EventBinding<RouletteFinishedEvent> rouletteFinishedBinding;

    void Awake()
    {
        foreach (var betButton in betButtons)
        {
            betButton.SetBetButton(chipSelectionController, this);
        }

        resetBetBinding = new EventBinding<ResetBetButtonEvent>(ClearAllBets);
        EventBus<ResetBetButtonEvent>.Register(resetBetBinding);

        gameStateBinding = new EventBinding<GameStateChangeEvent>(OnGameStateChanged);
        EventBus<GameStateChangeEvent>.Register(gameStateBinding);

        rouletteFinishedBinding = new EventBinding<RouletteFinishedEvent>(OnRouletteFinished);
        EventBus<RouletteFinishedEvent>.Register(rouletteFinishedBinding);
    }

    private void OnRouletteFinished(RouletteFinishedEvent @event)
    {
        winningNumber = @event.WinningNumber;
        StartCoroutine(ProcessResult());
    }

    private void OnGameStateChanged(GameStateChangeEvent @event)
    {
        IsBettingEnabled = @event.NewState == GameState.InBet ? true : false;
    }

    void Start()
    {
        PlayerSave.Instance.LoadBets(this);
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
            PlayerSave.Instance.SaveBets(activeBets);
        }
    }

    public void InvokePlaceBet(BetButton betButton)
    {
        HandleBet(betButton);
    }

    private void HandleBet(BetButton betButton)
    {
        PlayerSave.Instance.PlaceBet(ChipHelper.GetChipValue(chipSelectionController.SelectedChipValue));

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

        PlayerSave.Instance.ProcessPayment(totalWinnings, lostBets);

        // Wait for a moment before starting a new round
        yield return new WaitForSeconds(3f);

        EventBus<BetProcessingFinishedEvent>.Raise(new BetProcessingFinishedEvent { IsWinner = totalWinnings - lostBets > 0 });
        
        
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
        PlayerSave.Instance.ClearSavedBets();
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