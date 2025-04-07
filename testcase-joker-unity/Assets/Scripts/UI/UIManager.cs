using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

/// <summary>
/// This is the main class that controls the UI of the game.
/// It is responsible for the UI of the game.
/// </summary>
public class UIManager : MonoBehaviour
{
    [SerializeField] private UserMoney userMoney;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button addFreeChipsButton;

    //[SerializeField] private Button undoBetButton; //TODO: implement this
    [SerializeField] private Button resetBetButton;

    [SerializeField] private TextMeshProUGUI betAmountText;
    [SerializeField] private TextMeshProUGUI lastRoundEarningsText;
    [SerializeField] private TextMeshProUGUI userMoneyText;


    [SerializeField] private TextMeshProUGUI totalSpinsText;
    [SerializeField] private TextMeshProUGUI totalWinsText;
    [SerializeField] private TextMeshProUGUI totalProfitText;

    // Stats UI elements
    
    // Define events to communicate with other systems
    public event Action OnConfirmButtonClicked;
    public event Action OnRemoveBetButtonClicked;
    public event Action OnResetBetButtonClicked;
    

    private void Awake()
    {
        // Set up button listeners
        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnBetPlacementConfirmed);
            
            
        if (resetBetButton != null)
            resetBetButton.onClick.AddListener(OnResetBetClicked);

        if (addFreeChipsButton != null)
            addFreeChipsButton.onClick.AddListener(OnAddFreeChipsClicked);

        if (userMoney != null)
        {
            userMoney.OnMoneyChanged += OnUserMoneyChanged;
            userMoney.OnBetChanged += OnBetAmountChanged;
            userMoney.OnPaymentChanged += OnLastRoundEarningsChanged;
        }
    }

    private void OnLastRoundEarningsChanged(int value)
    {
        lastRoundEarningsText.text = value.ToString();
    }

    private void OnAddFreeChipsClicked()
    {
        userMoney.AddFreeChips();
    }

    private void OnDestroy()
    {
        // Clean up listeners
        if (confirmButton != null)
            confirmButton.onClick.RemoveListener(OnBetPlacementConfirmed);
            
            
        if (resetBetButton != null)
            resetBetButton.onClick.RemoveListener(OnResetBetClicked);

        if (userMoney != null)
        {
            userMoney.OnMoneyChanged -= OnUserMoneyChanged;
            userMoney.OnBetChanged -= OnBetAmountChanged;
            userMoney.OnPaymentChanged -= OnLastRoundEarningsChanged;
        }
    }


    private void OnBetAmountChanged(int value)
    {
        betAmountText.text = value.ToString();
    }

    private void OnUserMoneyChanged(int value)
    {
        userMoneyText.text = value.ToString();
    }

    public void OnBetPlacementConfirmed()
    {
        // Invoke the event for anyone listening
        Debug.Log("OnBetPlacementConfirmed");
        OnConfirmButtonClicked?.Invoke();
    }
    
    private void OnResetBetClicked()
    {
        OnResetBetButtonClicked?.Invoke();
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
    public void UpdateStatsDisplay(int totalSpins, int totalWins, int totalProfit)
    {
        totalSpinsText.text = totalSpins.ToString();
        totalWinsText.text = totalWins.ToString();
        totalProfitText.text = totalProfit.ToString();
    }
}