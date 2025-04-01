using System.Collections;
using UnityEngine;

public class RouletteWheel : MonoBehaviour
{
    [SerializeField] private float maxRotationSpeed = 720f; 
    [SerializeField] private float spinDuration = 5f;
    [SerializeField] private AnimationCurve spinCurve;
    
    private float currentSpeed = 0f;
    private bool isSpinning = false;
    
    public void Spin()
    {
        if (isSpinning) return;
        
        StartCoroutine(SpinCoroutine());
    }
    
    private IEnumerator SpinCoroutine()
    {
        isSpinning = true;
        float elapsedTime = 0f;
        
        while (elapsedTime < spinDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / spinDuration;
            
            currentSpeed = maxRotationSpeed * spinCurve.Evaluate(normalizedTime);
            
            transform.Rotate(Vector3.up, currentSpeed * Time.deltaTime);
            
            yield return null;
        }
        
        isSpinning = false;
        GetComponentInParent<RouletteController>().OnSpinCompleted();
    }
}