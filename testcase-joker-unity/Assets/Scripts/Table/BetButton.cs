using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public abstract class BetButton : MonoBehaviour, IBetButton
{
    [SerializeField] protected Color normalColor = new Color(0.8f, 0.8f, 0.8f, 0f);
    [SerializeField] protected Color hoverColor = new Color(0.8f, 0.8f, 0.2f, 0.5f);
    [SerializeField] protected Color winningColor = new Color(0f, 0.8f, 0f, 0.7f);
    [SerializeField] protected float colorTransitionSpeed = 5f;
    
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
        
        targetColor = normalColor;
        SetQuadColor(normalColor);
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
            t += Time.deltaTime * colorTransitionSpeed;
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
    
    private void OnMouseUp()
    {
        Debug.Log("OnMouseUp");
        OnClick();
    }
    
    private void OnMouseEnter()
    {
        Debug.Log("OnMouseEnter");
        if (!isHighlighted && !isWinning)
        {
            TransitionToColor(hoverColor);
        }
    }
    
    private void OnMouseExit()
    {
        if (!isHighlighted && !isWinning)
        {
            TransitionToColor(normalColor);
        }
    }
    
    public virtual void OnClick()
    {
    }
    
    public virtual void ShowWinningStatus(bool isWinning)
    {
        this.isWinning = isWinning;
        if (isWinning)
        {
            TransitionToColor(winningColor);
        }
        else
        {
            TransitionToColor(normalColor);
        }
    }
    
    public abstract BetType GetBetType();
    
    public abstract int GetCoveredNumbersCount();
    
    public virtual float GetPayout()
    {
        int coveredNumbers = GetCoveredNumbersCount();
        if (coveredNumbers <= 0) 
            return 0;
        
        return (36 - coveredNumbers) / (float)coveredNumbers;
    }
    
    public abstract bool IsWinner(int winningNumber);
    
    public virtual float CalculatePayout(float betAmount)
    {
        return betAmount * GetPayout();
    }
    
    protected static int CalculatePaymentMultiplier(int coveredNumbers)
    {
        return (36 - coveredNumbers) / coveredNumbers;
    }
}
