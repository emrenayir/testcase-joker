using UnityEngine;
using System.Collections;

/// <summary>
/// This class is the base class for all bet buttons.
/// It handles the color transition and the click event.
/// It also contains the logic for calculating the payout. //TODO: think is this the best place for this? 
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public abstract class BetButton : MonoBehaviour, IBetButton
{
    [SerializeField] protected TableColorSettings tableColorSettings;
    protected bool isHighlighted = false;
    protected bool isWinning = false;
    protected Material quadMaterial;
    protected MeshRenderer quadRenderer;
    protected Color targetColor;
    protected Coroutine colorChangeCoroutine;
    
    protected virtual void Awake()
    {
        quadRenderer = GetComponentInChildren<MeshRenderer>();
        quadMaterial = new Material(quadRenderer.sharedMaterial);
        quadRenderer.material = quadMaterial;
        
        targetColor = tableColorSettings.normalColor;
        SetQuadColor(tableColorSettings.normalColor);
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
        //TODO: implement this
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
    public virtual float CalculatePayout(float betAmount)
    {
        return betAmount * GetPayout();
    }
}
