using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles saving and loading of player data using PlayerPrefs
/// </summary>
public class PlayerSave
{
    // Keys for PlayerPrefs
    private const string MONEY_KEY = "PlayerMoney";
    private const string CURRENT_BET_KEY = "CurrentBet";
    private const string LAST_PAYMENT_KEY = "LastPayment";
    private const string TOTAL_SPINS_KEY = "TotalSpins";
    private const string TOTAL_WINS_KEY = "TotalWins";
    private const string TOTAL_PROFIT_KEY = "TotalProfit";
    private const string ACTIVE_BETS_KEY = "ActiveBets";


    private EventBinding<OnTotalSpinsChangedEvent> onTotalSpinsChangedBinding;
    private EventBinding<OnTotalWinsChangedEvent> onTotalWinsChangedBinding;
    private EventBinding<OnTotalProfitChangedEvent> onTotalProfitChangedBinding;
    private EventBinding<OnCurrentRoundProfitChangedEvent> onCurrentRoundProfitChangedBinding;
    private EventBinding<ResetBetButtonEvent> resetBetBinding;
    private EventBinding<AddFreeChipsButtonClickedEvent> addFreeChipsBinding;   

    private int totalSpins = 0;
    private int totalWins = 0;
    private int totalProfit = 0;
    private int currentRoundProfit = 0;
    
    // Player money properties
    private int userMoney;
    private int currentBet;
    private int currentPayment;
    
    // Singleton instance
    private static PlayerSave _instance;
    public static PlayerSave Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PlayerSave();
            }
            return _instance;
        }
    }
    
    // Constructor
    private PlayerSave()
    {
        Debug.Log("PlayerSave constructor");
        LoadPlayerData();
    }
    
    // Load player data from PlayerPrefs
    private void LoadPlayerData()
    {
        userMoney = PlayerPrefs.GetInt(MONEY_KEY, 1000); // Default starting money is 1000
        currentBet = PlayerPrefs.GetInt(CURRENT_BET_KEY, 0);
        currentPayment = PlayerPrefs.GetInt(LAST_PAYMENT_KEY, 0);

        totalSpins = PlayerPrefs.GetInt(TOTAL_SPINS_KEY, 0);
        totalWins = PlayerPrefs.GetInt(TOTAL_WINS_KEY, 0);
        totalProfit = PlayerPrefs.GetInt(TOTAL_PROFIT_KEY, 0);
        
        // Notify UI of initial values
        EventBus<OnMoneyChangedEvent>.Raise(new OnMoneyChangedEvent { Money = userMoney });
        EventBus<OnBetChangedEvent>.Raise(new OnBetChangedEvent { Bet = currentBet });
        EventBus<OnPaymentChangedEvent>.Raise(new OnPaymentChangedEvent { Payment = currentPayment });

        onTotalSpinsChangedBinding = new EventBinding<OnTotalSpinsChangedEvent>(OnTotalSpinsChanged);
        EventBus<OnTotalSpinsChangedEvent>.Register(onTotalSpinsChangedBinding);

        onTotalWinsChangedBinding = new EventBinding<OnTotalWinsChangedEvent>(OnTotalWinsChanged);
        EventBus<OnTotalWinsChangedEvent>.Register(onTotalWinsChangedBinding);

        onTotalProfitChangedBinding = new EventBinding<OnTotalProfitChangedEvent>(OnTotalProfitChanged);
        EventBus<OnTotalProfitChangedEvent>.Register(onTotalProfitChangedBinding);

        onCurrentRoundProfitChangedBinding = new EventBinding<OnCurrentRoundProfitChangedEvent>(OnCurrentRoundProfitChanged);
        EventBus<OnCurrentRoundProfitChangedEvent>.Register(onCurrentRoundProfitChangedBinding);

        resetBetBinding = new EventBinding<ResetBetButtonEvent>(ResetBet);
        EventBus<ResetBetButtonEvent>.Register(resetBetBinding);

        addFreeChipsBinding = new EventBinding<AddFreeChipsButtonClickedEvent>(AddFreeChips);
        EventBus<AddFreeChipsButtonClickedEvent>.Register(addFreeChipsBinding);

        
        // Load and notify stats
        PlayerStatsData stats = LoadPlayerStats();
        if (stats != null)
        {
            EventBus<UpdateStatsEvent>.Raise(new UpdateStatsEvent { PlayerStatsData = stats });
        }
        
    }

    private void OnCurrentRoundProfitChanged(OnCurrentRoundProfitChangedEvent @event)
    {
        currentRoundProfit = @event.CurrentRoundProfitChangeAmount;
        OnUpdateStats();
    }

    private void OnTotalProfitChanged(OnTotalProfitChangedEvent @event)
    {
        totalProfit += @event.ProfitChangeAmount;
        OnUpdateStats();
    }

    private void OnTotalWinsChanged(OnTotalWinsChangedEvent @event)
    {
        totalWins += @event.TotalWinsChangeAmount;
        OnUpdateStats();
    }

    private void OnTotalSpinsChanged(OnTotalSpinsChangedEvent @event)
    {
        totalSpins++;
        OnUpdateStats();
    }

    private void OnUpdateStats()
    {
        EventBus<UpdateStatsEvent>.Raise(new UpdateStatsEvent { PlayerStatsData = new PlayerStatsData { TotalSpins = totalSpins, TotalWins = totalWins, TotalProfit = totalProfit } });
        SavePlayerStats(totalSpins, totalWins, totalProfit);
    }

    // Money Management Methods

    public bool PlaceBet(int chipValue)
    {
        if (chipValue <= userMoney && chipValue > 0)
        {
            userMoney -= chipValue;
            currentBet += chipValue;
            
            PlayerPrefs.SetInt(MONEY_KEY, userMoney);
            PlayerPrefs.SetInt(CURRENT_BET_KEY, currentBet);
            PlayerPrefs.Save();
            
            EventBus<OnMoneyChangedEvent>.Raise(new OnMoneyChangedEvent { Money = userMoney });
            EventBus<OnBetChangedEvent>.Raise(new OnBetChangedEvent { Bet = currentBet });
            return true;
        }
        return false;
    }
    
    public void ProcessPayment(int payment, int lostBets)
    {
        currentPayment = payment + (currentBet - lostBets);
        currentBet = 0;
        
        if (payment > 0)
        {
            userMoney += currentPayment;
        }
        
        PlayerPrefs.SetInt(MONEY_KEY, userMoney);
        PlayerPrefs.SetInt(CURRENT_BET_KEY, currentBet);
        PlayerPrefs.SetInt(LAST_PAYMENT_KEY, payment - lostBets);
        PlayerPrefs.Save();
        
        EventBus<OnPaymentChangedEvent>.Raise(new OnPaymentChangedEvent { Payment = payment - lostBets });
        EventBus<OnBetChangedEvent>.Raise(new OnBetChangedEvent { Bet = currentBet });
        EventBus<OnMoneyChangedEvent>.Raise(new OnMoneyChangedEvent { Money = userMoney });

        EventBus<OnTotalProfitChangedEvent>.Raise(new OnTotalProfitChangedEvent { ProfitChangeAmount = payment - lostBets  });
    }
    
    public void AddFreeChips()
    {
        userMoney += 1000;
        PlayerPrefs.SetInt(MONEY_KEY, userMoney);
        PlayerPrefs.Save();
        
        EventBus<OnMoneyChangedEvent>.Raise(new OnMoneyChangedEvent { Money = userMoney });
    }
    
    public void ResetBet()
    {
        userMoney += currentBet;
        currentBet = 0;
        
        PlayerPrefs.SetInt(MONEY_KEY, userMoney);
        PlayerPrefs.SetInt(CURRENT_BET_KEY, currentBet);
        PlayerPrefs.Save();
        
        EventBus<OnMoneyChangedEvent>.Raise(new OnMoneyChangedEvent { Money = userMoney });
        EventBus<OnBetChangedEvent>.Raise(new OnBetChangedEvent { Bet = currentBet });
    }
    
    // Getters
    public int GetCurrentMoney() => userMoney;
    public int GetCurrentBet() => currentBet;
    public int GetCurrentPayment() => currentPayment;
    
    // Setters (for loading saved data)
    private void SetMoney(int amount)
    {
        userMoney = Mathf.Max(0, amount);
        PlayerPrefs.SetInt(MONEY_KEY, userMoney);
        PlayerPrefs.Save();
        
        EventBus<OnMoneyChangedEvent>.Raise(new OnMoneyChangedEvent { Money = userMoney });
        Debug.Log($"Player money set to: {userMoney}");
    }
    
    private void SetCurrentBet(int amount)
    {
        currentBet = Mathf.Max(0, amount);
        PlayerPrefs.SetInt(CURRENT_BET_KEY, currentBet);
        PlayerPrefs.Save();
        
        EventBus<OnBetChangedEvent>.Raise(new OnBetChangedEvent { Bet = currentBet });
        Debug.Log($"Current bet set to: {currentBet}");
    }
    
    private void SetCurrentPayment(int amount)
    {
        currentPayment = amount;
        PlayerPrefs.SetInt(LAST_PAYMENT_KEY, currentPayment);
        PlayerPrefs.Save();
        
        EventBus<OnPaymentChangedEvent>.Raise(new OnPaymentChangedEvent { Payment = currentPayment });
        Debug.Log($"Last round earnings set to: {currentPayment}");
    }

    
    
    // Bet Data Management
    
    public void SaveBets(List<BetButton> activeBets)
    {
        if (activeBets == null || activeBets.Count == 0)
        {
            PlayerPrefs.DeleteKey(ACTIVE_BETS_KEY);
            PlayerPrefs.Save();
            return;
        }
        
        PlayerSaveData saveData = new PlayerSaveData
        {
            PlayerMoney = userMoney,
            CurrentBet = currentBet,
            LastRoundEarnings = currentPayment,
            TotalSpins = PlayerPrefs.GetInt(TOTAL_SPINS_KEY, 0),
            TotalWins = PlayerPrefs.GetInt(TOTAL_WINS_KEY, 0),
            TotalProfit = PlayerPrefs.GetInt(TOTAL_PROFIT_KEY, 0)
        };
        
        foreach (var bet in activeBets)
        {
            BetData betData = new BetData
            {
                BetButtonName = bet.gameObject.name,
                BetType = bet.GetBetType().ToString(),
                TotalChipValue = bet.TotalChipValue,
                PlacedChipsData = bet.GetPlacedChipsData()
            };
            
            saveData.Bets.Add(betData);
        }
        
        string jsonData = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(ACTIVE_BETS_KEY, jsonData);
        PlayerPrefs.Save();
    }
    
    public PlayerStatsData LoadBets(BetController betController)
    {
        if (!PlayerPrefs.HasKey(ACTIVE_BETS_KEY))
        {
            Debug.Log("No saved bet data found");
            return null;
        }
        
        string jsonData = PlayerPrefs.GetString(ACTIVE_BETS_KEY);
        PlayerSaveData saveData = JsonUtility.FromJson<PlayerSaveData>(jsonData);
        
        if (saveData == null)
        {
            Debug.Log("No valid save data found");
            return null;
        }
        
        // Load player money data
        if (saveData.PlayerMoney > 0)
        {
            SetMoney(saveData.PlayerMoney);
        }
        
        if (saveData.CurrentBet > 0)
        {
            SetCurrentBet(saveData.CurrentBet);
        }
        
        SetCurrentPayment(saveData.LastRoundEarnings);
        
        // Load bets data
        if (saveData.Bets != null && saveData.Bets.Count > 0)
        {
            List<BetButton> betButtons = betController.GetBetButtons();
            
            foreach (var betData in saveData.Bets)
            {
                BetButton betButton = betButtons.Find(b => b.gameObject.name == betData.BetButtonName);
                if (betButton != null)
                {
                    betButton.LoadBetData(betData);
                }
            }
        }
        
        // Return stats data
        return new PlayerStatsData
        {
            TotalSpins = saveData.TotalSpins,
            TotalWins = saveData.TotalWins,
            TotalProfit = saveData.TotalProfit
        };
    }
    
    public void ClearSavedBets()
    {
        PlayerPrefs.DeleteKey(ACTIVE_BETS_KEY);
        PlayerPrefs.Save();
    }
    
    // Stats Management
    
    private void SavePlayerStats(int totalSpins, int totalWins, int totalProfit)
    {
        PlayerPrefs.SetInt(TOTAL_SPINS_KEY, totalSpins);
        PlayerPrefs.SetInt(TOTAL_WINS_KEY, totalWins);
        PlayerPrefs.SetInt(TOTAL_PROFIT_KEY, totalProfit);
        PlayerPrefs.Save();
        
        // Update active bets data if exists
        if (PlayerPrefs.HasKey(ACTIVE_BETS_KEY))
        {
            string jsonData = PlayerPrefs.GetString(ACTIVE_BETS_KEY);
            PlayerSaveData saveData = JsonUtility.FromJson<PlayerSaveData>(jsonData);
            
            if (saveData != null)
            {
                saveData.TotalSpins = totalSpins;
                saveData.TotalWins = totalWins;
                saveData.TotalProfit = totalProfit;
                
                PlayerPrefs.SetString(ACTIVE_BETS_KEY, JsonUtility.ToJson(saveData));
                PlayerPrefs.Save();
            }
        }
    }
    
    public PlayerStatsData LoadPlayerStats()
    {
        if (!PlayerPrefs.HasKey(TOTAL_SPINS_KEY))
        {
            return null;
        }
        
        return new PlayerStatsData
        {
            TotalSpins = PlayerPrefs.GetInt(TOTAL_SPINS_KEY, 0),
            TotalWins = PlayerPrefs.GetInt(TOTAL_WINS_KEY, 0),
            TotalProfit = PlayerPrefs.GetInt(TOTAL_PROFIT_KEY, 0)
        };
    }
    
}

[Serializable]
public class PlayerSaveData
{
    public List<BetData> Bets = new List<BetData>();
    public int PlayerMoney = 1000; // Default starting money
    public int CurrentBet = 0;
    public int LastRoundEarnings = 0;
    
    // Stats data
    public int TotalSpins = 0;
    public int TotalWins = 0;
    public int TotalProfit = 0;
}

[Serializable]
public class BetData
{
    public string BetButtonName;
    public string BetType;
    public int TotalChipValue;
    public List<ChipData> PlacedChipsData = new List<ChipData>();
}

[Serializable]
public class ChipData
{
    public int ChipValue;
    public float PositionY;
} 

[Serializable]
public class PlayerStatsData
{
    public int TotalSpins = 0;
    public int TotalWins = 0;
    public int TotalProfit = 0;
}