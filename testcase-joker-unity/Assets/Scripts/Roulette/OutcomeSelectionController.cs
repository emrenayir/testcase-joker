using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script is used to handle the outcome selection.
/// It is attached to the outcome selection panel in the roulette game.
/// </summary>
public class OutcomeSelectionController : MonoBehaviour
{

    public Action<int> OnOutcomeSelected;
    [SerializeField] private RouletteOutcomeManager rouletteOutcomeManager;
    [SerializeField] private List<Sprite> outcomeSprites;
    [SerializeField] private GameObject container;
    [SerializeField] private GameObject outcomeButtonPrefab;

    [SerializeField] private Transform closeLocation;

    [SerializeField] private Button openCloseButton;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private bool isOpen = false;
    private bool isAnimating = false;
    private Vector2 openPosition;
    private Vector2 closedPosition;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        
        var index = 0;
        foreach (var outcomeSprite in outcomeSprites)
        {
            var outcomeButton = Instantiate(outcomeButtonPrefab, container.transform);
            outcomeButton.GetComponent<Image>().sprite = outcomeSprite;
            outcomeButton.GetComponent<OutcomeButton>().SetOutcomeIndex(index, this);
            index++;
        }

        OnOutcomeSelected += OutcomeSelected;

        openCloseButton.onClick.AddListener(OpenClose);
        
        // Store initial positions
        openPosition = rectTransform.anchoredPosition;
        closedPosition = closeLocation.GetComponent<RectTransform>().anchoredPosition;
        
        // Start in closed position
        rectTransform.anchoredPosition = closedPosition;
    }

    private void OpenClose()
    {
        if (isAnimating) return;
        StartCoroutine(AnimateOpenClose());
    }

    private IEnumerator AnimateOpenClose()
    {
        isAnimating = true;
        float elapsed = 0f;
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 targetPos = isOpen ? closedPosition : openPosition;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            float easedT = easeCurve.Evaluate(t);

            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, easedT);
            yield return null;
        }

        rectTransform.anchoredPosition = targetPos;
        isOpen = !isOpen;
        isAnimating = false;
    }

    void OnDisable()
    {
        OnOutcomeSelected = null;
        openCloseButton.onClick.RemoveListener(OpenClose);
    }

    public void OutcomeSelected(int outcomeIndex)
    {
        rouletteOutcomeManager.SetSelectedNumber(outcomeIndex);
        OpenClose();
    }
}