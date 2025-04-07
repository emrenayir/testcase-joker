using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script is used to handle the outcome button click event.
/// It is attached to each outcome button in the roulette game.
/// </summary>
public class OutcomeButton : MonoBehaviour
{
    private OutcomeSelectionController outcomeSelectionController;
    private Button button;
    private int outcomeIndex;

    /// <summary>
    /// This method is used to set the outcome index and the outcome selection controller.
    /// </summary>
    /// <param name="index">The index of the outcome.</param>
    /// <param name="outcomeSelectionController">The outcome selection controller.</param>
    public void SetOutcomeIndex(int index, OutcomeSelectionController outcomeSelectionController)
    {
        this.outcomeSelectionController = outcomeSelectionController;
        outcomeIndex = index;
        button = GetComponent<Button>();
        button.onClick.AddListener(() => OutcomeSelected(outcomeIndex));
    }

    void OnDisable()
    {
        button.onClick.RemoveListener(() => OutcomeSelected(outcomeIndex));
    }

    private void OutcomeSelected(int outcomeIndex)
    {
        outcomeSelectionController.OnOutcomeSelected?.Invoke(outcomeIndex);
    }

}
