using UnityEngine;

public class RouletteOutcomeManager : MonoBehaviour
{
    [SerializeField] private int selectedNumber = -1; // -1 means random
    
    public void SetSelectedNumber(int number)
    {
        if (number >= 0 && number <= 36) 
        {
            selectedNumber = number;
        }
    }
    
    public int GetTargetNumber()
    {
        if (selectedNumber >= 0)
        {
            int result = selectedNumber;
            selectedNumber = -1; // Reset after use
            return result;
        }
        
        var randomNumber = UnityEngine.Random.Range(0, 37);
        Debug.Log("Random number: " + randomNumber);
        return randomNumber;
    }
}