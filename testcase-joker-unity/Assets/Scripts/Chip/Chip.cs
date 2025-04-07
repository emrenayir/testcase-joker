using UnityEngine;
using static ChipHelper;
using System.Collections;

/// <summary>
/// Represents a chip in the game with animation capabilities.
/// </summary>
public class Chip : MonoBehaviour
{
    public ChipValue value;

    /// <summary>
    /// Returns the value of this chip
    /// </summary>
    /// <returns>The integer value of the chip</returns>
    public int GetChipValue()
    {
        return ChipHelper.GetChipValue(value);
    }

    /// <summary>
    /// Animate chip placement with scale and position effects
    /// </summary>
    public void PlayPlacementAnimation()
    {
        StartCoroutine(AnimatePlacement());
    }

    /// <summary>
    /// Animate chip removal with scale and position effects
    /// </summary>
    /// <returns>Coroutine to wait for animation completion</returns>
    public IEnumerator PlayRemovalAnimation()
    {
        yield return StartCoroutine(AnimateRemoval());
        ChipPool.Instance.ReturnChip(gameObject);
    }

    private IEnumerator AnimatePlacement()
    {
        // Save original scale and position
        Vector3 targetScale = transform.localScale;
        Vector3 targetPosition = transform.position;
        
        // Start with smaller scale and slightly higher position
        transform.localScale = targetScale * 0.5f;
        transform.position = targetPosition + Vector3.up * 0.2f;
        
        float duration = 0.3f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Ease-out function
            float smoothT = 1 - Mathf.Pow(1 - t, 2);
            
            // Interpolate scale and position
            transform.localScale = Vector3.Lerp(targetScale * 0.5f, targetScale, smoothT);
            transform.position = Vector3.Lerp(targetPosition + Vector3.up * 0.2f, targetPosition, smoothT);
            
            yield return null;
        }
        
        // Ensure final values are set
        transform.localScale = targetScale;
        transform.position = targetPosition;
    }

    private IEnumerator AnimateRemoval()
    {
        Vector3 startScale = transform.localScale;
        Vector3 startPosition = transform.position;
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Ease-in function
            float smoothT = t * t;

            // Scale down and move up slightly while fading
            transform.localScale = Vector3.Lerp(startScale, startScale * 0.1f, smoothT);
            transform.position = Vector3.Lerp(startPosition, startPosition + Vector3.up * 0.2f, smoothT);

            yield return null;
        }
    }
}