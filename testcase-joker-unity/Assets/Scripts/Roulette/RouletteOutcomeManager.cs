using UnityEngine;

/// <summary>
/// This script is responsible for managing the outcome of the roulette wheel.
/// It is responsible for selecting the target number and handling the random number generation.
/// -1 is used to indicate that the number is not yet selected.
/// </summary>
public class RouletteOutcomeManager : MonoBehaviour, IRouletteOutcomeProvider
{
    [SerializeField] private int selectedNumber = -1; // -1 means random
    
    /// <summary>
    /// Sets the target number.
    /// It will be used by the roulette ball to determine the target slot.
    /// </summary>
    /// <param name="number">The number to set the target to.</param>
    public void SetSelectedNumber(int number)
    {
        if (number >= 0 && number <= 36) 
        {
            selectedNumber = number;
        }
    }
    
    /// <summary>
    /// Gets the target number.
    /// If the target number is not yet selected, a random number is generated.
    /// </summary>
    /// <returns>The target number.</returns>
    public int GetTargetNumber()
    {
        if (selectedNumber >= 0)
        {
            int result = selectedNumber;
            selectedNumber = -1; // Reset after use
            return result;
        }
        
        var randomNumber = Random.Range(0, 37);
        Debug.Log("Random number: " + randomNumber);
        return randomNumber;
    }
}