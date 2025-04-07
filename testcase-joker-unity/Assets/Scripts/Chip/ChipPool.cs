using System.Collections.Generic;
using UnityEngine;
using static ChipHelper;

/// <summary>
/// This is a singleton class that manages the pool of chips.
/// It is responsible for instantiating, pooling, and returning chips.
/// Handles multiple chip types.
/// </summary>
public class ChipPool : MonoBehaviour
{
    public static ChipPool Instance;

    [Tooltip("Scriptable Objects for different chip types")]
    public List<ChipSO> chipSOs;
    public int initialPoolSize = 20;

    private Dictionary<ChipValue, List<GameObject>> pooledChips;

    private void Awake()
    {
        Instance = this;
        pooledChips = new Dictionary<ChipValue, List<GameObject>>();
        InitializePool();
    }

    private void InitializePool()
    {
        foreach (ChipSO chipSO in chipSOs)
        {
            if (!pooledChips.ContainsKey(chipSO.value))
            {
                pooledChips[chipSO.value] = new List<GameObject>();
            }
            
            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject chip = Instantiate(chipSO.chipPrefab, transform);
                chip.SetActive(false);
                pooledChips[chipSO.value].Add(chip);
            }
        }
    }

    public GameObject GetChip(ChipValue chipValue)
    {
        if (!pooledChips.ContainsKey(chipValue))
        {
            Debug.LogError("No chip of type " + chipValue + " available in the pool");
            return null;
        }

        foreach (GameObject chip in pooledChips[chipValue])
        {
            if (!chip.activeInHierarchy)
            {
                chip.SetActive(true);
                return chip;
            }
        }

        // If we get here, we need to create a new chip
        ChipSO chipSO = chipSOs.Find(so => so.value == chipValue);
        if (chipSO == null)
        {
            Debug.LogError("No ChipSO found for value " + chipValue);
            return null;
        }

        GameObject newChip = Instantiate(chipSO.chipPrefab, transform);
        pooledChips[chipValue].Add(newChip);
        return newChip;
    }

    public void ReturnChip(GameObject chip)
    {
        // Reset position and rotation
        chip.transform.SetParent(transform);
        chip.transform.localPosition = Vector3.zero;
        chip.transform.localRotation = Quaternion.identity;
        chip.transform.localScale = Vector3.one;
        
        chip.SetActive(false);
    }   
}
