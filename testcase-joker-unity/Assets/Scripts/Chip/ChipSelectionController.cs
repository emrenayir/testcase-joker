using UnityEngine;
using System;
using static ChipHelper;
using System.Collections.Generic;

/// <summary>
/// Handles the selection of chips for betting phase.
/// </summary>
public class ChipSelectionController : MonoBehaviour
{
    /// <summary>
    /// Event triggered when a chip is selected.
    /// </summary>
    public event Action<ChipValue> OnChipSelected;

    /// <summary>
    /// Currently selected chip value.
    /// </summary>
    public ChipValue SelectedChipValue 
    { 
        get { return selectedChipValue; } 
        private set 
        { 
            selectedChipValue = value;
            OnChipSelected?.Invoke(selectedChipValue);
        }
    }

    private ChipValue selectedChipValue;
    private ChipButtonFactory chipButtonFactory;

    private List<ChipSelectionButton> chipButtons;

    void Awake()
    {
        chipButtonFactory = GetComponent<ChipButtonFactory>();
        // Set default value
        SelectedChipValue = ChipValue.One;
    }

    void Start()
    {
        chipButtons = chipButtonFactory.CreateChipButtons(this);
        chipButtons[0].SetSelected(true);
    }

    /// <summary>
    /// Called when a chip button is clicked.
    /// </summary>
    /// <param name="button">The button that was clicked.</param>
    public void ChipButtonClick(ChipSelectionButton button)
    {
        foreach (var chipButton in chipButtons)
        {
            chipButton.SetSelected(false);
        }
        button.SetSelected(true);
        SelectedChipValue = button.Value;
    }
}

