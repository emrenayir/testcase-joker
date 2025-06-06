using System.Collections;
using System.Collections.Generic;
using Audio;
using Bet.ScriptableObjects;
using Chips;
using UnityEngine;
using User;

namespace Bet
{
    /// <summary>
    /// This class is the base class for all bet buttons.
    /// It handles the color transition and the click event.
    /// It holds the logic of specific bet types.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public abstract class BetButton : MonoBehaviour
    {
        #region Fields
        public int TotalChipValue;
        [SerializeField] protected TableColorSettings tableColorSettings;
        [SerializeField] protected MeshRenderer quadRenderer;
        protected ChipSelectionController chipSelectionController;
        protected bool isHighlighted = false;
        protected bool isWinning;
        protected Material quadMaterial;
  
        protected Color targetColor;
        protected Coroutine colorChangeCoroutine;
        protected BoxCollider cachedBoxCollider;
        protected BetController betController;

        private int chipIndex;
        private const float chipStackOffset = 0.01f; // Vertical offset for stacking chips
        private readonly List<Chip> placedChips = new(); // List of chips objects for animations

        #endregion

        #region Unity Lifecycle Methods

        void Awake()
        {
            // Get the box collider
            cachedBoxCollider = GetComponent<BoxCollider>();
        }
        protected virtual void Start()
        {
            // Get the quad renderer and material
            quadRenderer = GetComponentInChildren<MeshRenderer>();
            quadMaterial = new Material(quadRenderer.sharedMaterial);
            quadRenderer.material = quadMaterial;
            // Set the initial color
            targetColor = tableColorSettings.normalColor;
            SetQuadColor(tableColorSettings.normalColor);

        }

        #endregion

        #region Initialization
        //Configure the bet button
        public void SetBetButton(ChipSelectionController a_chipSelectionController, BetController a_betController)
        {
            this.chipSelectionController = a_chipSelectionController;
            this.betController = a_betController;
        }
        #endregion

        #region Mouse Event Handlers
        private void OnMouseDown()
        {
            OnClick();
        }
    
        private void OnMouseEnter()
        {
            if (!isHighlighted && !isWinning)
            {
                TransitionToColor(tableColorSettings.hoverColor);
            }
        }
    
        private void OnMouseExit()
        {
            if (!isHighlighted && !isWinning)
            {
                TransitionToColor(tableColorSettings.normalColor);
            }
        }
        #endregion

        #region Bet Functionality
        // Add bet to the necessary place
        public virtual void OnClick()
        {
            // Check if betting is enabled
            if (!betController.IsBettingEnabled) return;

            // Check if we've reached the chip limit also check user money is enough
            if (ChipHelper.GetChipValue(chipSelectionController.SelectedChipValue) > PlayerSave.Instance.GetCurrentMoney())
            {
                Debug.Log("Not enough money"); 
                SoundManager.Instance.PlaySound("Error");
                return;
            }

            // Play the chip placement sound
            SoundManager.Instance.PlaySound("Chip");

            // Add the chip value to the total chip value for later payout calculation
            TotalChipValue += ChipHelper.GetChipValue(chipSelectionController.SelectedChipValue);

            // Get chip from pool
            Chip chip = ChipPool.Instance.GetChip(chipSelectionController.SelectedChipValue).GetComponent<Chip>();
        
            // Set parent and position
            chip.transform.SetParent(transform);
        
            // Position the chip on top of previous chips, using BoxCollider's center
            Vector3 chipPosition = transform.TransformPoint(cachedBoxCollider.center);
            chipPosition.y += chipStackOffset * chipIndex;
            chip.transform.position = chipPosition;
        
            // Animate the chip using the Chip's own animation method
            chip.PlayPlacementAnimation();
        
            // Add to the list of placed chips
            placedChips.Add(chip);
        
            // Increment chip index for next chip
            chipIndex++;

            // Invoke the bet placed event
            betController.InvokePlaceBet(this);
        }
    
        // Method to reset chips and return them to the pool
        public virtual void ResetChips()
        {
            isWinning = false;
            TransitionToColor(tableColorSettings.normalColor);
            StartCoroutine(AnimateChipsRemoval());
        }

        private IEnumerator AnimateChipsRemoval()
        {
            // Animate each chip's removal
            foreach (Chip chip in placedChips)
            {
                if (chip != null)
                {
                    StartCoroutine(chip.PlayRemovalAnimation());
                    // Add a small delay between each chip's animation
                    yield return new WaitForSeconds(0.05f);
                }
            }

            // Wait for all animations to complete
            yield return new WaitForSeconds(0.3f);

            // Clear the list and reset index after animations
            placedChips.Clear();
            chipIndex = 0;
            TotalChipValue = 0;
        }
    
        /// <summary>
        /// Calculate the payout for the bet
        /// It uses the formula (36 - coveredNumbers) / coveredNumbers
        /// </summary>
        /// <returns>The payout for the bet</returns>
        public virtual float GetPayout()
        {
            int coveredNumbers = GetCoveredNumbersCount();
            if (coveredNumbers <= 0) 
                return 0;
        
            return (36 - coveredNumbers) / (float)coveredNumbers;
        }
    
        /// <summary>
        /// Calculate the payout for the bet
        /// </summary>
        /// <param name="betAmount">The amount of the bet</param>
        /// <returns>The payout for the bet</returns>
        public virtual int CalculatePayout(int betAmount)
        {
            return (int)(betAmount * GetPayout());
        }
        #endregion

        #region Visual Methods
        protected void SetQuadColor(Color color)
        {
            if (quadRenderer != null && quadRenderer.material != null)
            {
                quadRenderer.material.color = color;
            }
        }
    
        protected void TransitionToColor(Color newColor)
        {
            targetColor = newColor;
        
            if (colorChangeCoroutine != null)
            {
                StopCoroutine(colorChangeCoroutine);
                colorChangeCoroutine = null;
            }
        
            // Check if the current color already matches the target
            if (quadRenderer.material.color != targetColor)
            {
                colorChangeCoroutine = StartCoroutine(LerpColor());
            }
        }
    
        protected IEnumerator LerpColor()
        {
            Color currentColor = quadRenderer.material.color;
            float t = 0;
        
            while (t < 1)
            {
                t += Time.deltaTime * tableColorSettings.colorTransitionSpeed;
                quadRenderer.material.color = Color.Lerp(currentColor, targetColor, t);
                yield return null;
            }
        
            quadRenderer.material.color = targetColor;
            colorChangeCoroutine = null;
        }
    
        // Method to show the winning status of the bet
        public virtual void ShowWinningStatus(bool a_isWinning)
        {
            this.isWinning = a_isWinning;
            TransitionToColor(a_isWinning ? tableColorSettings.winningColor : tableColorSettings.normalColor);
        }
        #endregion

        #region Save/Load Methods
        /// <summary>
        /// Get data about placed chips for saving
        /// </summary>
        public List<ChipData> GetPlacedChipsData()
        {
            List<ChipData> chipsData = new List<ChipData>();
        
            foreach (var chip in placedChips)
            {
                ChipData chipData = new ChipData
                {
                    ChipValue = chip.GetChipValue(),
                    PositionY = chip.transform.position.y
                };
            
                chipsData.Add(chipData);
            }
        
            return chipsData;
        }
    
        /// <summary>
        /// Load bet data and recreate the chips
        /// </summary>
        public void LoadBetData(BetData betData)
        {
            // Reset current chip state if any
            if (placedChips.Count > 0)
            {
                // Force immediate reset rather than animated - we're loading a saved state
                foreach (Chip chip in placedChips)
                {
                    if (chip != null)
                    {
                        ChipPool.Instance.ReturnChip(chip.gameObject);
                    }
                }
                placedChips.Clear();
                chipIndex = 0;
                TotalChipValue = 0;
            }
        
            TotalChipValue = betData.TotalChipValue;
        
            // Recreate chips from saved data
            foreach (var chipData in betData.PlacedChipsData)
            {
                // Get chip from pool
                Chip chip = ChipPool.Instance.GetChip(ChipHelper.GetChipTypeFromValue(chipData.ChipValue)).GetComponent<Chip>();
            
                // Set parent
                chip.transform.SetParent(transform);
            
                // Position the chip using the saved position
                Vector3 chipPosition = transform.TransformPoint(cachedBoxCollider.center);
                chipPosition.y = chipData.PositionY;
                chip.transform.position = chipPosition;
            
                // Add to the list of placed chips
                placedChips.Add(chip);
            
                // Increment chip index
                chipIndex++;
            }
        
            // Add this bet to the active bets
            betController.AddActiveBet(this);
        }
        #endregion

        #region Abstract Methods
        public abstract BetType GetBetType();
        public abstract int GetCoveredNumbersCount();
        public abstract bool IsWinner(int winningNumber);
        #endregion
    }
}
