using System;
using UnityEngine;

/// <summary>
/// This class is responsible for managing the user's money.
/// It is responsible for adding and subtracting money from the user's money.
/// It is also responsible for updating the UI when the money changes.
/// </summary>
public class UserMoney : MonoBehaviour
{
    [SerializeField] private int userMoney = 1000;
    [SerializeField] private int currentBet = 0;
    [SerializeField] private int currentPayment = 0;

    // Events for UI updates
    public event Action<int> OnMoneyChanged;
    public event Action<int> OnBetChanged;
    public event Action<int> OnPaymentChanged;


    [SerializeField] private UIManager uiManager;

    private void Start()
    {
        // Initialize events
        OnMoneyChanged?.Invoke(userMoney);
        OnBetChanged?.Invoke(currentBet);
        OnPaymentChanged?.Invoke(currentPayment);

        uiManager.OnResetBetButtonClicked += OnResetBetButtonClicked;
    }

    private void OnResetBetButtonClicked()
    {
        //return bet to user
        AddMoney(currentBet);
        currentBet = 0;
        OnBetChanged?.Invoke(currentBet);
    }

    // Operations on player money
    private void AddMoney(int amount)
    {
        userMoney += amount;
        OnMoneyChanged?.Invoke(userMoney);
    }
    
    private void SubtractMoney(int amount)
    {
        if(amount < 0)
        {
            amount = Mathf.Abs(amount);
        }

        userMoney -= amount;
        
        OnMoneyChanged?.Invoke(userMoney);
    }

    // Operations on player bet
    private void AddBet(int amount)
    {
        currentBet += amount;
        OnBetChanged?.Invoke(currentBet);
    }
    
    private void SubtractBet(int amount)
    {
        currentBet -= amount;
        OnBetChanged?.Invoke(currentBet);
    }

    // Public methods
    public bool PlaceBet(int chipValue)
    {
        // Check if bet is possible
        if (chipValue <= userMoney && chipValue > 0)
        {
            SubtractMoney(chipValue);
            AddBet(chipValue);
            return true;
        }
        
        return false;
    }
    
    public void RemoveChip(int chipValue)
    {
        SubtractBet(chipValue);
        AddMoney(chipValue);
    }
    
    public void ProcessPayment(int payment, int lostBets)
    {
        currentPayment = payment + (currentBet - lostBets);
        OnPaymentChanged?.Invoke(currentPayment);
        
        // Reset current bet
        currentBet = 0;
        OnBetChanged?.Invoke(currentBet);

        // Add winnings if any
        if (payment > 0)
        {
            AddMoney(currentPayment);
        }
        
        Debug.Log($"Player money updated with payment: {payment}! Current payment set to: {currentPayment}");
    }
    
    public int GetCurrentMoney()
    {
        return userMoney;
    }
    
    public int GetCurrentBet()
    {
        return currentBet;
    }
    
    /// <summary>
    /// Gets the current payment amount
    /// </summary>
    /// <returns>The current payment amount</returns>
    public int GetCurrentPayment()
    {
        return currentPayment;
    }
    
    /// <summary>
    /// Sets the player's money amount directly (used for loading saved data)
    /// </summary>
    /// <param name="amount">The amount to set</param>
    public void SetMoney(int amount)
    {
        // Make sure we don't set a negative amount
        userMoney = Mathf.Max(0, amount);
        OnMoneyChanged?.Invoke(userMoney);
        Debug.Log($"Player money set to: {userMoney}");
    }
    
    /// <summary>
    /// Sets the current bet amount directly (used for loading saved data)
    /// </summary>
    /// <param name="amount">The bet amount to set</param>
    public void SetCurrentBet(int amount)
    {
        // Make sure we don't set a negative amount
        currentBet = Mathf.Max(0, amount);
        OnBetChanged?.Invoke(currentBet);
        Debug.Log($"Current bet set to: {currentBet}");
    }
    
    /// <summary>
    /// Sets the last round earnings amount directly (used for loading saved data)
    /// </summary>
    /// <param name="amount">The payment amount to set</param>
    public void SetCurrentPayment(int amount)
    {
        currentPayment = amount;
        OnPaymentChanged?.Invoke(currentPayment);
        Debug.Log($"Last round earnings set to: {currentPayment}");
    }

    public void AddFreeChips()
    {
        AddMoney(1000);
    }
}
