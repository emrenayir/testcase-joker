using UnityEngine;
using System.Collections;
using Game;

/// <summary>
/// This is the main class that controls the game loop.
/// It is responsible for the game phases and the logic of the game.
/// </summary>
public class GameLoop : MonoBehaviour
{
    [SerializeField] private RouletteController rouletteController; //TODO: remove this dependency
    [SerializeField] private BetController betController; //TODO: remove this dependency
    [SerializeField] private ParticleSystem winParticles; //TODO: remove this dependency

    private GameState currentPhase;

    private EventBinding<BetPlacementConfirmedButtonEvent> betPlacementConfirmedBinding;


    void Awake()
    {
        betPlacementConfirmedBinding = new EventBinding<BetPlacementConfirmedButtonEvent>(OnBetPlacementConfirmed);
        EventBus<BetPlacementConfirmedButtonEvent>.Register(betPlacementConfirmedBinding);
    }

    void OnDestroy()
    {
        EventBus<BetPlacementConfirmedButtonEvent>.UnRegister(betPlacementConfirmedBinding);
    }

    void Start()
    {
        //Init player save TODO: Move to some other class idk where
        var playerSave = PlayerSave.Instance;
        SetPhase(GameState.InBet);
    }

    private void SetPhase(GameState newPhase)
    {
        currentPhase = newPhase;
        
        // Raise event for state change
        EventBus<GameStateChangeEvent>.Raise(new GameStateChangeEvent { NewState = newPhase });
        
        switch (currentPhase)
        {
            case GameState.InBet:
                EnterChipSelectionPhase();
                break;
            case GameState.Running:
                EnterRouletteSpinningPhase();
                break;
            case GameState.Finish:
                EnterResultsPhase();
                break;
        }
    }

    private void EnterChipSelectionPhase()
    {
        betController.IsBettingEnabled = true;
    }

    private void EnterRouletteSpinningPhase()
    {
         //TODO: Move to some other class its not the game loop responsibility
        EventBus<OnCurrentRoundProfitChangedEvent>.Raise(new OnCurrentRoundProfitChangedEvent { CurrentRoundProfitChangeAmount = -PlayerSave.Instance.GetCurrentBet() });
        EventBus<OnTotalSpinsChangedEvent>.Raise(new OnTotalSpinsChangedEvent {});


        // Start the roulette spinning and listen for completion
        rouletteController.StartRoulette(); //Roullette will listen for state change and call StartRoulette()
        
        // Start a coroutine to check when the roulette stops spinning //instead of waiting for the coroutine we should listen for the event from roulette controller
        StartCoroutine(WaitForRouletteToComplete());
    }

    private IEnumerator WaitForRouletteToComplete()
    {
        // Wait until roulette is no longer spinning
        // Use a reasonable delay to check periodically
        yield return new WaitForSeconds(8f); // Adjust as needed
        
        
        // Move to results phase
        SetPhase(GameState.Finish);
    }

    private void EnterResultsPhase()
    {
        // Process the bet results is not the game loop responsibility
        StartCoroutine(ProcessResults());
    }

    //TODO: Move to some other class its not the game loop responsibility
    private IEnumerator ProcessResults()
    {
        // Store initial money for win/loss calculation
        int initialMoney = PlayerSave.Instance.GetCurrentMoney();
        
        // Process the bet based on the winning number
        betController.ProcessResult(rouletteController.GetResult());
        
        // Calculate profit/loss for this round
        int finalMoney = PlayerSave.Instance.GetCurrentMoney();
        int winnings = finalMoney - initialMoney;



        EventBus<OnCurrentRoundProfitChangedEvent>.Raise(new OnCurrentRoundProfitChangedEvent { CurrentRoundProfitChangeAmount = winnings });
        
        // Update stats
        EventBus<OnTotalProfitChangedEvent>.Raise(new OnTotalProfitChangedEvent { ProfitChangeAmount = winnings });
        
        // Check if player won this round
        if (winnings > 0)
        {
            EventBus<OnTotalWinsChangedEvent>.Raise(new OnTotalWinsChangedEvent { TotalWinsChangeAmount = 1 });

            SoundManager.Instance.PlaySFX("Win");
            winParticles.Play();
        }else
        {
            SoundManager.Instance.PlaySFX("Error");
        }

        
        // Wait for a moment before starting a new round
        yield return new WaitForSeconds(3f);
        
        
        // Clear previous bets
        betController.ClearAllBets();
        
        // Return to chip selection phase
        SetPhase(GameState.InBet);
    }

    // Button event handlers
    private void OnBetPlacementConfirmed()
    {
        // Player has confirmed their chip selection, move to next phase
        SetPhase(GameState.Running);
    }

    
}