using System.Collections.Generic;
using Chips.ScriptableObjects;
using UnityEngine;

namespace Chips
{
    /// <summary>
    /// Creates chip buttons at the locations specified in the inspector.
    /// </summary>
    public class ChipButtonFactory : MonoBehaviour
    {
        [Tooltip("Locations where chip buttons will be placed")]
        [SerializeField] private List<Transform> chipButtonLocations;
    
        [Tooltip("Prefab for chip buttons")]
        [SerializeField] private GameObject chipButtonPrefab;

        [Tooltip("ScriptableObjects for different chip values")]
        [SerializeField] private List<ChipSO> chipSOList;

        /// <summary>
        /// Creates chip buttons for the provided chip selection controller
        /// and places them at the specified locations.
        /// </summary>
        /// <param name="chipSelectionController">The controller that manages chip selection.</param>
        public List<ChipSelectionButton> CreateChipButtons(ChipSelectionController chipSelectionController)
        {
            ChipValue[] chipValues = (ChipValue[])System.Enum.GetValues(typeof(ChipValue));
            List<ChipSelectionButton> chipButtons = new List<ChipSelectionButton>();
            for (int i = 0; i < chipButtonLocations.Count && i < chipValues.Length; i++)
            {
                GameObject chipButton = CreateChipButton(chipValues[i], chipSelectionController);
                PlaceChipButton(chipButton, chipButtonLocations[i]);
                chipButtons.Add(chipButton.GetComponent<ChipSelectionButton>());
            }

            return chipButtons;
        }

        /// <summary>
        /// Creates a chip button with the specified value.
        /// </summary>
        /// <param name="value">The value of the chip button.</param>
        /// <param name="chipSelectionController">The controller that manages chip selection.</param>
        /// <returns>The created chip button GameObject.</returns>
        private GameObject CreateChipButton(ChipValue value, ChipSelectionController chipSelectionController)
        {
            // Instantiate the chip button prefab
            GameObject chipButton = Instantiate(chipButtonPrefab);

            // Find the chip SO for the specified value
            ChipSO chipSO = FindChipSO(value);
        
            // Instantiate the chip prefab
            GameObject chip = Instantiate(chipSO.chipPrefab, chipButton.transform, true);
            chip.transform.localPosition = Vector3.zero;

            // Initialize the chip button component
            ChipSelectionButton chipButtonComponent = chipButton.GetComponent<ChipSelectionButton>();
            chipButtonComponent.Initialize(value, chipSelectionController, chip);
        
            return chipButton;
        }

        /// <summary>
        /// Places a chip button at the specified location.
        /// </summary>
        /// <param name="chipButton">The chip button to place.</param>
        /// <param name="location">The location to place the chip button at.</param>
        private void PlaceChipButton(GameObject chipButton, Transform location)
        {
            chipButton.transform.SetParent(location);
            chipButton.transform.localPosition = Vector3.zero;
        }

        /// <summary>
        /// Finds the ChipSO for the specified value.
        /// </summary>
        /// <param name="value">The value to find the ChipSO for.</param>
        /// <returns>The ChipSO for the specified value.</returns>
        private ChipSO FindChipSO(ChipValue value)
        {
            return chipSOList.Find(chipSO => chipSO.value == value);
        }
    }
}