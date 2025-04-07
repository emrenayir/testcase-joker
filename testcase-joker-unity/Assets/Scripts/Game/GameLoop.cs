using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

/// <summary>
/// This is the main class that controls the game loop.
/// It is responsible for the game phases and the logic of the game.
/// </summary>
public class GameLoop : MonoBehaviour
{
    [SerializeField] private RouletteController rouletteController;
    [SerializeField] private BetController betController;
    [SerializeField] private UserMoney userMoney;
    [SerializeField] private UIManager uiManager;

    private enum GamePhase
    {
        BetPlacement,
        RouletteSpinning,
        Results
    }

    private GamePhase currentPhase;

    void Start()
    {
        if (uiManager != null)
        {
            uiManager.OnConfirmButtonClicked += OnBetPlacementConfirmed;
            uiManager.OnResetBetButtonClicked += OnResetBetButtonClicked;
        }
        
        StartGame();
    }


    void OnDestroy()
    {
        if (uiManager != null)
            uiManager.OnConfirmButtonClicked -= OnBetPlacementConfirmed;
    }

    public void StartGame()
    {
        // Start with chip selection phase
        SetPhase(GamePhase.BetPlacement);
    }

    private void SetPhase(GamePhase newPhase)
    {
        currentPhase = newPhase;
        
        switch (currentPhase)
        {
            case GamePhase.BetPlacement:
                EnterChipSelectionPhase();
                break;
            case GamePhase.RouletteSpinning:
                EnterRouletteSpinningPhase();
                break;
            case GamePhase.Results:
                EnterResultsPhase();
                break;
        }
    }

    private void EnterChipSelectionPhase()
    {
        Debug.Log("Entering Chip Selection Phase");
        // Enable chip selection and betting
        // Enable confirm button
        if (uiManager != null)
        {
            uiManager.SetConfirmButtonActive(true);
        }
        
        betController.IsBettingEnabled = true;
    }

    private void EnterRouletteSpinningPhase()
    {
        Debug.Log("Entering Roulette Spinning Phase");
        // Disable chip selection and betting
        betController.IsBettingEnabled = false;
        if (uiManager != null)
        {
            uiManager.SetConfirmButtonActive(false);
        }
        
        // Start the roulette spinning and listen for completion
        rouletteController.StartRoulette();
        
        // Start a coroutine to check when the roulette stops spinning
        StartCoroutine(WaitForRouletteToComplete());
    }

    private IEnumerator WaitForRouletteToComplete()
    {
        // Wait until roulette is no longer spinning
        // Use a reasonable delay to check periodically
        yield return new WaitForSeconds(5f); // Adjust as needed
        
        // Move to results phase
        SetPhase(GamePhase.Results);
    }

    private void EnterResultsPhase()
    {
        Debug.Log("Entering Results Phase");
        
        // Process the bet results
        StartCoroutine(ProcessResults());
    }

    private IEnumerator ProcessResults()
    {
        
        // Process the bet based on the winning number
        betController.ProcessResult(rouletteController.GetResult());
        
        // Wait for a moment before starting a new round
        yield return new WaitForSeconds(3f);
        
        // Clear previous bets
        betController.ClearAllBets();
        
        // Return to chip selection phase
        SetPhase(GamePhase.BetPlacement);
    }

    // Button event handlers
    private void OnBetPlacementConfirmed()
    {
        // Player has confirmed their chip selection, move to next phase
        SetPhase(GamePhase.RouletteSpinning);
    }

    private void OnResetBetButtonClicked()
    {
        betController.ClearAllBets();
    }
}