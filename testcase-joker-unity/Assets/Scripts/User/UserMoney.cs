using System;
using UnityEngine;

public class UserMoney : MonoBehaviour
{
    [SerializeField] private int userMoney = 1000;
    [SerializeField] private int currentBet = 0;
    [SerializeField] private int currentPayment = 0;

    // Events for UI updates
    public event Action<int> OnMoneyChanged;
    public event Action<int> OnBetChanged;
    public event Action<int> OnPaymentChanged;

    private void Start()
    {
        // Initialize events
        OnMoneyChanged?.Invoke(userMoney);
        OnBetChanged?.Invoke(currentBet);
        OnPaymentChanged?.Invoke(currentPayment);
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

        if (userMoney < 0)
        {
            userMoney = 0;
        }
        
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
        // Remove chip from table
        SubtractBet(chipValue);
        AddMoney(chipValue);
    }
    
    public void ProcessPayment(int payment)
    {
        currentPayment = payment;
        OnPaymentChanged?.Invoke(payment);
        
        // Reset current bet
        currentBet = 0;
        OnBetChanged?.Invoke(currentBet);

        // Add winnings if any
        if (payment > 0)
        {
            AddMoney(payment);
        }
        
        Debug.Log($"Player money updated with payment: {payment}!");
    }
    
    public int GetCurrentMoney()
    {
        return userMoney;
    }
    
    public int GetCurrentBet()
    {
        return currentBet;
    }
}
