using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// This class handles saving and loading of bet data using JSON
/// </summary>
public class PlayerSave : MonoBehaviour
{
    private string saveFilePath;
    private string statsFilePath;
    [SerializeField] private UserMoney userMoney;

    private void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "player_data.json");
        statsFilePath = Path.Combine(Application.persistentDataPath, "player_stats.json");
        Debug.Log($"Save file path: {saveFilePath}");
    }

    /// <summary>
    /// Save the active bets to JSON file
    /// </summary>
    /// <param name="activeBets">List of active bet buttons</param>
    public void SaveBets(List<BetButton> activeBets)
    {
        PlayerSaveData saveData = new PlayerSaveData();
        
        // If we have an existing save file, try to load stats from it first
        if (File.Exists(saveFilePath))
        {
            string existingJson = File.ReadAllText(saveFilePath);
            PlayerSaveData existingData = JsonUtility.FromJson<PlayerSaveData>(existingJson);
            if (existingData != null)
            {
                // Preserve stats data
                saveData.TotalSpins = existingData.TotalSpins;
                saveData.TotalWins = existingData.TotalWins;
                saveData.TotalProfit = existingData.TotalProfit;
            }
        }
        
        // Load latest stats data if available
        PlayerStatsData statsData = LoadPlayerStats();
        if (statsData != null)
        {
            saveData.TotalSpins = statsData.TotalSpins;
            saveData.TotalWins = statsData.TotalWins;
            saveData.TotalProfit = statsData.TotalProfit;
        }
        
        // Save bets data
        if (activeBets != null && activeBets.Count > 0)
        {
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
        }
        
        // Save player money data
        if (userMoney != null)
        {
            saveData.PlayerMoney = userMoney.GetCurrentMoney();
            saveData.CurrentBet = userMoney.GetCurrentBet();
            saveData.LastRoundEarnings = userMoney.GetCurrentPayment();
        }

        string jsonData = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveFilePath, jsonData);
    }
    

    [ContextMenu("ClearSavedBets")]
    /// <summary>
    /// Clear all saved data by deleting the save file
    /// </summary>
    public void ClearSavedBets()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }
    }

    /// <summary>
    /// Load saved bets from JSON and restore them to the game
    /// </summary>
    /// <param name="betController">Reference to the bet controller</param>
    /// <returns>PlayerStatsData object containing stored stats, or null if no data</returns>
    public PlayerStatsData LoadBets(BetController betController)
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.Log("No saved data found");
            return null;
        }

        string jsonData = File.ReadAllText(saveFilePath);
        PlayerSaveData saveData = JsonUtility.FromJson<PlayerSaveData>(jsonData);

        if (saveData == null)
        {
            Debug.Log("No valid save data found");
            return null;
        }
        
        // Load player money data
        if (userMoney != null)
        {
            if (saveData.PlayerMoney > 0)
            {
                userMoney.SetMoney(saveData.PlayerMoney);
            }
            
            if (saveData.CurrentBet > 0)
            {
                userMoney.SetCurrentBet(saveData.CurrentBet);
            }
            
            // Load last round earnings
            userMoney.SetCurrentPayment(saveData.LastRoundEarnings);
        }

        // Load bets data
        if (saveData.Bets == null || saveData.Bets.Count == 0)
        {
            Debug.Log("No bet data found in save file");
        }
        else
        {
            List<BetButton> betButtons = betController.GetBetButtons();

            int loadedBetsCount = 0;
            foreach (var betData in saveData.Bets)
            {
                // Find the bet button by name
                BetButton betButton = betButtons.Find(b => b.gameObject.name == betData.BetButtonName);
                if (betButton != null)
                {
                    betButton.LoadBetData(betData);
                    loadedBetsCount++;
                }
            }
        }
        
        // Return stats data
        PlayerStatsData statsData = new PlayerStatsData
        {
            TotalSpins = saveData.TotalSpins,
            TotalWins = saveData.TotalWins,
            TotalProfit = saveData.TotalProfit
        };
        
        
        
        return statsData;
    }
    
    /// <summary>
    /// Save only the player money data
    /// </summary>
    public void SavePlayerMoneyOnly()
    {
        if (userMoney == null) return;
        
        // If save file exists, load it first to preserve bet data
        PlayerSaveData saveData = new PlayerSaveData();
        if (File.Exists(saveFilePath))
        {
            string jsonData = File.ReadAllText(saveFilePath);
            saveData = JsonUtility.FromJson<PlayerSaveData>(jsonData);
            
            if (saveData == null)
            {
                saveData = new PlayerSaveData();
            }
        }
        
        // Update money data
        saveData.PlayerMoney = userMoney.GetCurrentMoney();
        saveData.CurrentBet = userMoney.GetCurrentBet();
        saveData.LastRoundEarnings = userMoney.GetCurrentPayment();
        
        // Save back to file
        string updatedJsonData = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveFilePath, updatedJsonData);
    }
    
    /// <summary>
    /// Save player stats (wins, losses, etc)
    /// </summary>
    public void SavePlayerStats(int totalSpins, int totalWins, int totalProfit)
    {
        // Save to dedicated stats file
        PlayerStatsData statsData = new PlayerStatsData
        {
            TotalSpins = totalSpins,
            TotalWins = totalWins,
            TotalProfit = totalProfit
        };
        
        string jsonData = JsonUtility.ToJson(statsData, true);
        File.WriteAllText(statsFilePath, jsonData);
        
        // Also update the main save file if it exists
        if (File.Exists(saveFilePath))
        {
            string existingJson = File.ReadAllText(saveFilePath);
            PlayerSaveData saveData = JsonUtility.FromJson<PlayerSaveData>(existingJson);
            
            if (saveData != null)
            {
                saveData.TotalSpins = totalSpins;
                saveData.TotalWins = totalWins;
                saveData.TotalProfit = totalProfit;
                
                string updatedJson = JsonUtility.ToJson(saveData, true);
                File.WriteAllText(saveFilePath, updatedJson);
            }
        }
    }
    
    /// <summary>
    /// Load player stats
    /// </summary>
    /// <returns>PlayerStatsData object or null if no data exists</returns>
    public PlayerStatsData LoadPlayerStats()
    {
        if (!File.Exists(statsFilePath))
        {
            return null;
        }
        
        string jsonData = File.ReadAllText(statsFilePath);
        PlayerStatsData statsData = JsonUtility.FromJson<PlayerStatsData>(jsonData);
        
        return statsData;
    }
    
    [ContextMenu("ClearPlayerStats")]
    /// <summary>
    /// Clear player stats by deleting the stats file
    /// </summary>
    public void ClearPlayerStats()
    {
        if (File.Exists(statsFilePath))
        {
            File.Delete(statsFilePath);
        }
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