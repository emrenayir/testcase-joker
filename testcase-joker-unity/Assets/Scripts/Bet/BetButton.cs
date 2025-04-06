using UnityEngine; 
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// This class is the base class for all bet buttons.
/// It handles the color transition and the click event.
/// It holds the logic of specific bet types.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public abstract class BetButton : MonoBehaviour, IBetButton
{
    #region Fields
    public int TotalChipValue = 0;
    [SerializeField] protected TableColorSettings tableColorSettings;
    protected ChipSelectionController chipSelectionController;
    protected bool isHighlighted = false;
    protected bool isWinning = false;
    protected Material quadMaterial;
    protected MeshRenderer quadRenderer;
    protected Color targetColor;
    protected Coroutine colorChangeCoroutine;
    protected BoxCollider cachedBoxCollider;
    protected BetController betController;
    protected UserMoney userMoney;

    private int chipIndex = 0;
    private const int MAX_CHIPS = 20; // Maximum number of chips that can be placed on a single bet
    private const float chipStackOffset = 0.01f; // Vertical offset for stacking chips
    private List<Chip> placedChips = new List<Chip>(); // List of chips objects for animations
    #endregion

    #region Unity Lifecycle Methods
    protected virtual void Start()
    {
        // Get the quad renderer and material
        quadRenderer = GetComponentInChildren<MeshRenderer>();
        quadMaterial = new Material(quadRenderer.sharedMaterial);
        quadRenderer.material = quadMaterial;
        // Set the initial color
        targetColor = tableColorSettings.normalColor;
        SetQuadColor(tableColorSettings.normalColor);

        // Get the box collider
        cachedBoxCollider = GetComponent<BoxCollider>();

        // Subscribe to the bet removed event
        betController.OnBetRemoved += ResetChips;
    }

    protected void OnDisable()
    {
        betController.OnBetRemoved -= ResetChips;
    }

    private void Update()
    {
        if (colorChangeCoroutine == null && quadRenderer.material.color != targetColor)
        {
            colorChangeCoroutine = StartCoroutine(LerpColor());
        }
    }
    #endregion

    #region Initialization
    //Configure the bet button
    public void SetBetButton(ChipSelectionController chipSelectionController, BetController betController, UserMoney userMoney)
    {
        this.chipSelectionController = chipSelectionController;
        this.betController = betController;
        this.userMoney = userMoney;
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
        if (chipIndex >= MAX_CHIPS || ChipHelper.GetChipValue(chipSelectionController.SelectedChipValue) > userMoney.GetCurrentMoney())
        {
            return;
        } 

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
        }
        
        colorChangeCoroutine = StartCoroutine(LerpColor());
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
    public virtual void ShowWinningStatus(bool isWinning)
    {
        this.isWinning = isWinning;
        if (isWinning)
        {
            TransitionToColor(tableColorSettings.winningColor);
        }
        else
        {
            TransitionToColor(tableColorSettings.normalColor);
        }
    }
    #endregion

    #region Abstract Methods
    public abstract BetType GetBetType();
    public abstract int GetCoveredNumbersCount();
    public abstract bool IsWinner(int winningNumber);
    #endregion
}
