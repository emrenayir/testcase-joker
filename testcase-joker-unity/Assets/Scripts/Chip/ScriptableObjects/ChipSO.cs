using UnityEngine;
using static ChipHelper;

/// <summary>
/// Scriptable Object for chip data including its value and prefab.
/// </summary>
[CreateAssetMenu(fileName = "New Chip", menuName = "Chip")]
public class ChipSO : ScriptableObject
{
    /// <summary>   
    /// The value of the chip.
    /// </summary>
    public ChipValue value;

    /// <summary>
    /// The prefab of the chip.
    /// </summary>
    public GameObject chipPrefab;
}
