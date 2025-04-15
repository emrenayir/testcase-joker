using System.Collections.Generic;
using UnityEngine;

namespace Chips
{
    /// <summary>
    /// Handles the selection of chips for betting phase.
    /// </summary>
    public class ChipSelectionController : MonoBehaviour
    {

        /// <summary>
        /// Currently selected chip value.
        /// </summary>
        public ChipValue SelectedChipValue { get; private set; }

        private ChipButtonFactory chipButtonFactory;

        private List<ChipSelectionButton> chipButtons;

        void Awake()
        {
            chipButtonFactory = GetComponent<ChipButtonFactory>();
        }

        void Start()
        {
            chipButtons = chipButtonFactory.CreateChipButtons(this);

            // Set default value
            SelectedChipValue = ChipValue.One;
            SelectChipButtonByValue(SelectedChipValue);
        }

        /// <summary>
        /// Selects the chip button matching the specified value.
        /// </summary>
        /// <param name="value">The chip value to select.</param>
        private void SelectChipButtonByValue(ChipValue value)
        {
            // Deselect all buttons first
            foreach (var chipButton in chipButtons)
            {
                chipButton.SetSelected(false);
            }

            // Find and select the button with matching value
            ChipSelectionButton buttonToSelect = chipButtons.Find(button => button.Value == value);

            buttonToSelect.SetSelected(true);
            SelectedChipValue = buttonToSelect.Value;

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
}

