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
    [SerializeField] private Button undoBetButton; //TODO: implement this
    [SerializeField] private Button resetBetButton;

    [SerializeField] private TextMeshProUGUI betAmountText;
    [SerializeField] private TextMeshProUGUI userMoneyText;
    
    // Define events to communicate with other systems
    public event Action OnConfirmButtonClicked;
    public event Action OnRemoveBetButtonClicked;
    public event Action OnResetBetButtonClicked;
    

    private void Awake()
    {
        // Set up button listeners
        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnBetPlacementConfirmed);
            
        if (undoBetButton != null)
            undoBetButton.onClick.AddListener(OnRemoveBetClicked);
            
        if (resetBetButton != null)
            resetBetButton.onClick.AddListener(OnResetBetClicked);

        if (userMoney != null)
        {
            userMoney.OnMoneyChanged += OnUserMoneyChanged;
            userMoney.OnBetChanged += OnBetAmountChanged;
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

    private void OnDestroy()
    {
        // Clean up listeners
        if (confirmButton != null)
            confirmButton.onClick.RemoveListener(OnBetPlacementConfirmed);
            
        if (undoBetButton != null)
            undoBetButton.onClick.RemoveListener(OnRemoveBetClicked);
            
        if (resetBetButton != null)
            resetBetButton.onClick.RemoveListener(OnResetBetClicked);
    }

    public void OnBetPlacementConfirmed()
    {
        // Invoke the event for anyone listening
        OnConfirmButtonClicked?.Invoke();
    }
    
    private void OnRemoveBetClicked()
    {
        OnRemoveBetButtonClicked?.Invoke();
    }
    
    private void OnResetBetClicked()
    {
        OnResetBetButtonClicked?.Invoke();
    }
    
    // Methods to control button visibility
    public void SetConfirmButtonActive(bool isActive)
    {
        if (confirmButton != null)
            confirmButton.gameObject.SetActive(isActive);
    }
    
    public void SetRemoveBetButtonActive(bool isActive)
    {
        if (undoBetButton != null)
            undoBetButton.gameObject.SetActive(isActive);
    }
    
    public void SetResetBetButtonActive(bool isActive)
    {
        if (resetBetButton != null)
            resetBetButton.gameObject.SetActive(isActive);
    }
}