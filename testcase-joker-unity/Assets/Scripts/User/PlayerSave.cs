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
    private const string ACTIVE_BETS_KEY = "ActiveBets";

    private int totalSpins = 0;
    private int totalWins = 0;

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
        LoadPlayerData();

        // Register to application quit event
        Application.quitting += SaveAllData;
    }

    // Load player data from PlayerPrefs
    private void LoadPlayerData()
    {
        userMoney = PlayerPrefs.GetInt(MONEY_KEY, 1000); // Default starting money is 1000
        currentBet = PlayerPrefs.GetInt(CURRENT_BET_KEY, 0);
        currentPayment = PlayerPrefs.GetInt(LAST_PAYMENT_KEY, 0);

        totalSpins = PlayerPrefs.GetInt(TOTAL_SPINS_KEY, 0);
        totalWins = PlayerPrefs.GetInt(TOTAL_WINS_KEY, 0);

        // Notify for initial values
        EventManager.Instance.Raise(new OnMoneyChangedEvent { Money = userMoney });
        EventManager.Instance.Raise(new OnBetChangedEvent { Bet = currentBet });
        EventManager.Instance.Raise(new OnPaymentChangedEvent { Payment = currentPayment });

        // Register event handlers using EventManager
        EventManager.Instance.RegisterEvent<OnTotalSpinsChangedEvent>(OnTotalSpinsChanged);
        EventManager.Instance.RegisterEvent<OnTotalWinsChangedEvent>(OnTotalWinsChanged);
        EventManager.Instance.RegisterEvent<OnCurrentRoundProfitChangedEvent>(OnCurrentRoundProfitChanged);
        EventManager.Instance.RegisterEvent<ResetBetButtonEvent>(ResetBet);
        EventManager.Instance.RegisterEvent<AddFreeChipsButtonClickedEvent>(AddFreeChips);
        EventManager.Instance.RegisterEvent<SaveBetsEvent>(OnSaveBets);
        EventManager.Instance.RegisterEvent<LoadBetsRequestEvent>(OnLoadBetsRequest);
        EventManager.Instance.RegisterEvent<PlaceBetEvent>(OnPlaceBet);
        EventManager.Instance.RegisterEvent<ProcessPaymentEvent>(OnProcessPayment);
        EventManager.Instance.RegisterEvent<ClearSavedBetsEvent>(OnClearSavedBets);

        // Load and notify stats
        PlayerStatsData stats = LoadPlayerStats();
        if (stats != null)
        {
            EventManager.Instance.Raise(new UpdateStatsEvent { PlayerStatsData = stats });
        }

        // Notify system that we're ready to load saved bets
        EventManager.Instance.Raise(new LoadBetsRequestEvent());
    }

    private void OnCurrentRoundProfitChanged(OnCurrentRoundProfitChangedEvent @event)
    {
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
        EventManager.Instance.Raise(new UpdateStatsEvent { PlayerStatsData = new PlayerStatsData { TotalSpins = totalSpins, TotalWins = totalWins } });
    }

    // Money Management Methods

    private void OnPlaceBet(PlaceBetEvent @event)
    {
        HandleBet(@event.ChipValue);
    }

    private void OnProcessPayment(ProcessPaymentEvent @event)
    {
        ProcessPayment(@event.Payment, @event.LostBets);
    }

    private void OnClearSavedBets(ClearSavedBetsEvent @event)
    {
        ClearSavedBets();
    }

    public bool HandleBet(int chipValue)
    {
        if (chipValue <= userMoney && chipValue > 0)
        {
            userMoney -= chipValue;
            currentBet += chipValue;

            EventManager.Instance.Raise(new OnMoneyChangedEvent { Money = userMoney });
            EventManager.Instance.Raise(new OnBetChangedEvent { Bet = currentBet });
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

        EventManager.Instance.Raise(new OnPaymentChangedEvent { Payment = payment - lostBets });
        EventManager.Instance.Raise(new OnBetChangedEvent { Bet = currentBet });
        EventManager.Instance.Raise(new OnMoneyChangedEvent { Money = userMoney });
    }

    public void AddFreeChips()
    {
        userMoney += 1000;
        EventManager.Instance.Raise(new OnMoneyChangedEvent { Money = userMoney });
    }

    public void ResetBet()
    {
        userMoney += currentBet;
        currentBet = 0;

        EventManager.Instance.Raise(new OnMoneyChangedEvent { Money = userMoney });
        EventManager.Instance.Raise(new OnBetChangedEvent { Bet = currentBet });
    }

    // Getters
    public int GetCurrentMoney() => userMoney;

    // Setters (for loading saved data)
    private void SetMoney(int amount)
    {
        userMoney = Mathf.Max(0, amount);
        PlayerPrefs.SetInt(MONEY_KEY, userMoney);
        PlayerPrefs.Save();

        EventManager.Instance.Raise(new OnMoneyChangedEvent { Money = userMoney });
        Debug.Log($"Player money set to: {userMoney}");
    }

    private void SetCurrentBet(int amount)
    {
        currentBet = Mathf.Max(0, amount);
        PlayerPrefs.SetInt(CURRENT_BET_KEY, currentBet);
        PlayerPrefs.Save();

        EventManager.Instance.Raise(new OnBetChangedEvent { Bet = currentBet });
        Debug.Log($"Current bet set to: {currentBet}");
    }

    private void SetCurrentPayment(int amount)
    {
        currentPayment = amount;
        PlayerPrefs.SetInt(LAST_PAYMENT_KEY, currentPayment);
        PlayerPrefs.Save();

        EventManager.Instance.Raise(new OnPaymentChangedEvent { Payment = currentPayment });
        Debug.Log($"Last round earnings set to: {currentPayment}");
    }

    // Bet Data Management
    private void OnSaveBets(SaveBetsEvent @event)
    {
        SaveBets(@event.ActiveBets);
    }

    private void SaveBets(List<BetButton> activeBets)
    {
        if (activeBets == null || activeBets.Count == 0)
        {
            PlayerPrefs.DeleteKey(ACTIVE_BETS_KEY);
            return;
        }

        PlayerSaveData saveData = new PlayerSaveData
        {
            PlayerMoney = userMoney,
            CurrentBet = currentBet,
            LastRoundEarnings = currentPayment,
            TotalSpins = totalSpins,
            TotalWins = totalWins
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
    }

    private void OnLoadBetsRequest(LoadBetsRequestEvent @event)
    {
        if (!PlayerPrefs.HasKey(ACTIVE_BETS_KEY))
        {
            Debug.Log("No saved bet data found");
            return;
        }

        string jsonData = PlayerPrefs.GetString(ACTIVE_BETS_KEY);
        PlayerSaveData saveData = JsonUtility.FromJson<PlayerSaveData>(jsonData);

        if (saveData == null)
        {
            Debug.Log("No valid save data found");
            return;
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

        // Notify BetController of saved bets to load
        if (saveData.Bets != null && saveData.Bets.Count > 0)
        {
            EventManager.Instance.Raise(new LoadSavedBetsEvent { SavedBets = saveData.Bets });
        }

        // Update stats
        PlayerStatsData statsData = new PlayerStatsData
        {
            TotalSpins = saveData.TotalSpins,
            TotalWins = saveData.TotalWins,
        };

        EventManager.Instance.Raise(new UpdateStatsEvent { PlayerStatsData = statsData });
    }

    public void ClearSavedBets()
    {
        PlayerPrefs.DeleteKey(ACTIVE_BETS_KEY);
        PlayerPrefs.Save();
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
        };
    }

    // Save all player data at once
    public void SaveAllData()
    {
        Debug.Log("Saving all player data");

        // Save money, bet and payment data
        PlayerPrefs.SetInt(MONEY_KEY, userMoney);
        PlayerPrefs.SetInt(CURRENT_BET_KEY, currentBet);
        PlayerPrefs.SetInt(LAST_PAYMENT_KEY, currentPayment);

        // Save stats
        PlayerPrefs.SetInt(TOTAL_SPINS_KEY, totalSpins);
        PlayerPrefs.SetInt(TOTAL_WINS_KEY, totalWins);

        // Save active bets if any
        if (PlayerPrefs.HasKey(ACTIVE_BETS_KEY))
        {
            string jsonData = PlayerPrefs.GetString(ACTIVE_BETS_KEY);
            PlayerSaveData saveData = JsonUtility.FromJson<PlayerSaveData>(jsonData);

            if (saveData != null)
            {
                saveData.PlayerMoney = userMoney;
                saveData.CurrentBet = currentBet;
                saveData.LastRoundEarnings = currentPayment;
                saveData.TotalSpins = totalSpins;
                saveData.TotalWins = totalWins;

                PlayerPrefs.SetString(ACTIVE_BETS_KEY, JsonUtility.ToJson(saveData));
            }
        }

        // Actually save to disk
        PlayerPrefs.Save();
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
}