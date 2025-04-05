using UnityEngine; 
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// This class is the base class for all bet buttons.
/// It handles the color transition and the click event.
/// It also contains the logic for calculating the payout. //TODO: think is this the best place for this? 
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public abstract class BetButton : MonoBehaviour, IBetButton
{
    [SerializeField] protected TableColorSettings tableColorSettings;
    protected ChipSelectionController chipSelectionController;
    protected bool isHighlighted = false;
    protected bool isWinning = false;
    protected Material quadMaterial;
    protected MeshRenderer quadRenderer;
    protected Color targetColor;
    protected Coroutine colorChangeCoroutine;
    protected BoxCollider cachedBoxCollider;

    public event Action<BetButton> OnBetPlaced;

    protected BetController betController;
   

    private int chipIndex = 0;
    private const int MAX_CHIPS = 20; // Maximum number of chips that can be placed on a single bet
    private const float chipStackOffset = 0.01f; // Vertical offset for stacking chips
    private List<Chip> placedChips = new List<Chip>();

    public int TotalChipValue = 0;


    
    protected virtual void Start()
    {
        quadRenderer = GetComponentInChildren<MeshRenderer>();
        quadMaterial = new Material(quadRenderer.sharedMaterial);
        quadRenderer.material = quadMaterial;
        cachedBoxCollider = GetComponent<BoxCollider>();
        
        targetColor = tableColorSettings.normalColor;
        SetQuadColor(tableColorSettings.normalColor);

        betController.OnBetRemoved += ResetChips;
    }


    public void SetBetButton(ChipSelectionController chipSelectionController, BetController betController)
    {
        this.chipSelectionController = chipSelectionController;
        this.betController = betController;
    }

    protected void InvokeOnBetPlaced()
    {
        OnBetPlaced?.Invoke(this);
    }

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
    
    private void Update()
    {
        if (colorChangeCoroutine == null && quadRenderer.material.color != targetColor)
        {
            colorChangeCoroutine = StartCoroutine(LerpColor());
        }
    }
    
    //TODO: which event should we use? onMouseDown or onMouseUp?
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
    
    // Add bet to the necessary place
    public virtual void OnClick()
    {
        // Check if we've reached the chip limit
        if (chipIndex >= MAX_CHIPS)
        {
            Debug.LogWarning("Maximum chip limit reached for this bet!");
            return;
        }

        InvokeOnBetPlaced();

        TotalChipValue += ChipHelper.GetChipValue(chipSelectionController.SelectedChipValue);

        // Get chip from pool
        Chip chip = ChipPool.Instance.GetChip(chipSelectionController.SelectedChipValue).GetComponent<Chip>();
        
        // Set parent and position
        chip.transform.SetParent(transform);
        
        // Position the chip on top of previous chips, using BoxCollider's center
        Vector3 chipPosition = transform.TransformPoint(cachedBoxCollider.center);
        chipPosition.y += chipStackOffset * chipIndex;
        chip.transform.position = chipPosition;
        
        // Animate the chip
        StartCoroutine(AnimateChipPlacement(chip.gameObject));
        
        // Add to the list of placed chips
        placedChips.Add(chip);
        
        // Increment chip index for next chip
        chipIndex++;
    }
    
    // Animate chip placement with scale and position effects
    private IEnumerator AnimateChipPlacement(GameObject chip)
    {
        // Save original scale and position
        Vector3 targetScale = chip.transform.localScale;
        Vector3 targetPosition = chip.transform.position;
        
        // Start with smaller scale and slightly higher position
        chip.transform.localScale = targetScale * 0.5f;
        chip.transform.position = targetPosition + Vector3.up * 0.2f;
        
        float duration = 0.3f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Ease-out function
            float smoothT = 1 - Mathf.Pow(1 - t, 2);
            
            // Interpolate scale and position
            chip.transform.localScale = Vector3.Lerp(targetScale * 0.5f, targetScale, smoothT);
            chip.transform.position = Vector3.Lerp(targetPosition + Vector3.up * 0.2f, targetPosition, smoothT);
            
            yield return null;
        }
        
        // Ensure final values are set
        chip.transform.localScale = targetScale;
        chip.transform.position = targetPosition;
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
                StartCoroutine(AnimateChipRemoval(chip.gameObject));
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

    private IEnumerator AnimateChipRemoval(GameObject chip)
    {
        Vector3 startScale = chip.transform.localScale;
        Vector3 startPosition = chip.transform.position;
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Ease-in function
            float smoothT = t * t;

            // Scale down and move up slightly while fading
            chip.transform.localScale = Vector3.Lerp(startScale, startScale * 0.1f, smoothT);
            chip.transform.position = Vector3.Lerp(startPosition, startPosition + Vector3.up * 0.2f, smoothT);

            yield return null;
        }

        // Return to pool after animation
        ChipPool.Instance.ReturnChip(chip);
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


    


    //Remove bet calculation logic from here
    public abstract BetType GetBetType();
    
    public abstract int GetCoveredNumbersCount();
    
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
    
    public abstract bool IsWinner(int winningNumber);
    
    /// <summary>
    /// Calculate the payout for the bet
    /// </summary>
    /// <param name="betAmount">The amount of the bet</param>
    /// <returns>The payout for the bet</returns>
    public virtual int CalculatePayout(int betAmount)
    {
        return (int)(betAmount * GetPayout());
    }
}
