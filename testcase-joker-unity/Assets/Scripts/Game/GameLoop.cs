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
    [SerializeField] private PlayerSave playerSave;

    [SerializeField] private ParticleSystem winParticles;

    // Win/Loss tracking
    private int totalSpins = 0;
    private int totalWins = 0;
    private int totalProfit = 0;
    private int currentRoundProfit = 0;

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
        
        // Load stats
        if (playerSave != null)
        {
            var stats = playerSave.LoadPlayerStats();
            
            if (stats == null && betController != null)
            {
                stats = playerSave.LoadBets(betController);
            }
            
            // Apply the loaded stats if available
            if (stats != null)
            {
                totalSpins = stats.TotalSpins;
                totalWins = stats.TotalWins;
                totalProfit = stats.TotalProfit;
            }
        }
        
        // Update UI with initial stats
        UpdateStatsUI();
        
        StartGame();
    }

    // Helper method to update stats UI
    private void UpdateStatsUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateStatsDisplay(totalSpins, totalWins, totalProfit);
        }
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
        // Enable chip selection and betting
        // Enable confirm button
        if (uiManager != null)
        {
            uiManager.SetButtonActive(true);
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
            uiManager.SetButtonActive(false);
        }
        
        // Track the current bet amount for profit/loss calculation
        currentRoundProfit = -userMoney.GetCurrentBet();
        
        // Increment total spins
        totalSpins++;
        
        // Start the roulette spinning and listen for completion
        rouletteController.StartRoulette();
        
        // Start a coroutine to check when the roulette stops spinning
        StartCoroutine(WaitForRouletteToComplete());
    }

    private IEnumerator WaitForRouletteToComplete()
    {
        // Wait until roulette is no longer spinning
        // Use a reasonable delay to check periodically
        yield return new WaitForSeconds(8f); // Adjust as needed
        
        
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
        // Store initial money for win/loss calculation
        int initialMoney = userMoney.GetCurrentMoney();
        
        // Process the bet based on the winning number
        betController.ProcessResult(rouletteController.GetResult());
        
        // Calculate profit/loss for this round
        int finalMoney = userMoney.GetCurrentMoney();
        int winnings = finalMoney - initialMoney;
        currentRoundProfit += winnings;
        
        // Update stats
        totalProfit += currentRoundProfit;
        
        // Check if player won this round
        if (currentRoundProfit > 0)
        {
            totalWins++;
        }
        
        // Update UI
        UpdateStatsUI();
        
        // Save stats
        if (playerSave != null)
        {
            playerSave.SavePlayerStats(totalSpins, totalWins, totalProfit);
        }
        
        if (winnings > 0)
        {
            SoundManager.Instance.PlaySFX("Win");
            winParticles.Play();
        }
        else
        {
            SoundManager.Instance.PlaySFX("Error");
        }
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