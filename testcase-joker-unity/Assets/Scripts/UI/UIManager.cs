using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Game;

/// <summary>
/// This is the main class that controls the UI of the game.
/// It is responsible for the UI of the game.
/// </summary>
public class UIManager : MonoBehaviour
{
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button addFreeChipsButton;
    [SerializeField] private Button resetBetButton;

    [SerializeField] private TextMeshProUGUI betAmountText;
    [SerializeField] private TextMeshProUGUI lastRoundEarningsText;
    [SerializeField] private TextMeshProUGUI userMoneyText;


    [SerializeField] private TextMeshProUGUI totalSpinsText;
    [SerializeField] private TextMeshProUGUI totalWinsText;
    [SerializeField] private TextMeshProUGUI totalProfitText;


    //Event binding for game state changes
    private EventBinding<GameStateChangeEvent> gameStateBinding;
    private EventBinding<UpdateStatsEvent> updateStatsBinding;
    private EventBinding<OnMoneyChangedEvent> onMoneyChangedBinding;
    private EventBinding<OnBetChangedEvent> onBetChangedBinding;
    private EventBinding<OnPaymentChangedEvent> onPaymentChangedBinding;


    private void Awake()
    {
        //GameState 
        gameStateBinding = new EventBinding<GameStateChangeEvent>(OnGameStateChanged);
        EventBus<GameStateChangeEvent>.Register(gameStateBinding);

        //UpdateStats
        updateStatsBinding = new EventBinding<UpdateStatsEvent>(UpdateStatsDisplay);
        EventBus<UpdateStatsEvent>.Register(updateStatsBinding);

        Debug.Log("UIManager Awake");
        //Event binding for user money changes
        onMoneyChangedBinding = new EventBinding<OnMoneyChangedEvent>(OnUserMoneyChanged);
        EventBus<OnMoneyChangedEvent>.Register(onMoneyChangedBinding);

        onBetChangedBinding = new EventBinding<OnBetChangedEvent>(OnBetAmountChanged);
        EventBus<OnBetChangedEvent>.Register(onBetChangedBinding);

        onPaymentChangedBinding = new EventBinding<OnPaymentChangedEvent>(OnLastRoundEarningsChanged);
        EventBus<OnPaymentChangedEvent>.Register(onPaymentChangedBinding);


        

    }

    private void OnDestroy()
    {
        EventBus<GameStateChangeEvent>.UnRegister(gameStateBinding);
        EventBus<UpdateStatsEvent>.UnRegister(updateStatsBinding);
        EventBus<OnMoneyChangedEvent>.UnRegister(onMoneyChangedBinding);
        EventBus<OnBetChangedEvent>.UnRegister(onBetChangedBinding);
        EventBus<OnPaymentChangedEvent>.UnRegister(onPaymentChangedBinding);

    }

    private void OnGameStateChanged(GameStateChangeEvent @event)
    {
        switch (@event.NewState)
        {
            case GameState.InBet:
                SetButtonActive(true);
                break;
                
            case GameState.Running:
                SetButtonActive(false);
                break;
                
            case GameState.Finish:
                break;
        }
    }

    private void OnLastRoundEarningsChanged(OnPaymentChangedEvent @event)
    {
        lastRoundEarningsText.text = @event.Payment.ToString();
    }

    public void OnAddFreeChipsClicked()
    {
        EventBus<AddFreeChipsButtonClickedEvent>.Raise(new AddFreeChipsButtonClickedEvent());
    }


    private void OnBetAmountChanged(OnBetChangedEvent @event)
    {
        betAmountText.text = @event.Bet.ToString();
    }

    private void OnUserMoneyChanged(OnMoneyChangedEvent @event)
    {
        userMoneyText.text = @event.Money.ToString();
    }

    public void OnBetPlacementConfirmed()
    {
        EventBus<BetPlacementConfirmedButtonEvent>.Raise(new BetPlacementConfirmedButtonEvent());
    }

    public void OnResetBetClicked()
    {
        EventBus<ResetBetButtonEvent>.Raise(new ResetBetButtonEvent());
    }

    // Methods to control button visibility
    public void SetButtonActive(bool isActive)
    {
        if (confirmButton != null)
            confirmButton.gameObject.SetActive(isActive);

        if (resetBetButton != null)
            resetBetButton.gameObject.SetActive(isActive);

        if (addFreeChipsButton != null)
            addFreeChipsButton.gameObject.SetActive(isActive);
    }


    /// <summary>
    /// Update the stats display with the current win/loss tracking information
    /// </summary>
    public void UpdateStatsDisplay(UpdateStatsEvent @event)
    {
        totalSpinsText.text = @event.PlayerStatsData.TotalSpins.ToString();
        totalWinsText.text = @event.PlayerStatsData.TotalWins.ToString();
        totalProfitText.text = @event.PlayerStatsData.TotalProfit.ToString();
    }
}