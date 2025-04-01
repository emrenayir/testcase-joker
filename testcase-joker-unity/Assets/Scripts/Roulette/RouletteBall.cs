using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouletteBall : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float initialForce = 10f;
    [SerializeField] private float rollDuration = 8f;
    [SerializeField] private RouletteSlot[] numberSlotPositions;
    [SerializeField] private Transform wheelCenter;
    [SerializeField] private float wheelRadius = 2f;
    [SerializeField] private float initialCirclingSpeed = 15f;
    [SerializeField] private AnimationCurve speedDecayCurve;
    [SerializeField] private float targetApproachThreshold = 20f;
    
    private int targetNumber = -1;
    private bool isRolling = false;
    
    public void StartRolling(int targetNum)
    {
        if (isRolling) return;
        
        if (numberSlotPositions == null || numberSlotPositions.Length == 0)
        {
            Debug.LogError("numberSlotPositions array is null or empty");
            return;
        }
        
        targetNumber = -1;
        for (int i = 0; i < numberSlotPositions.Length; i++)
        {
            if (numberSlotPositions[i] != null && numberSlotPositions[i].SlotNumber == targetNum)
            {
                targetNumber = i;
                break;
            }
        }
        
        if (targetNumber == -1)
        {
            Debug.LogError("Could not find slot with number: " + targetNum);
            return;
        }
        
        StartCoroutine(RollBallCoroutine());
    }
    
    private IEnumerator RollBallCoroutine()
    {
        isRolling = true;
        
        Vector3 targetPosition = numberSlotPositions[targetNumber].transform.position;
        
        Vector3 targetDirFromCenter = targetPosition - wheelCenter.position;
        targetDirFromCenter.y = 0;
        float targetAngle = Mathf.Atan2(targetDirFromCenter.z, targetDirFromCenter.x);
        
        float randomAngle;
        do {
            randomAngle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
        } while (Mathf.Abs(Mathf.DeltaAngle(randomAngle * Mathf.Rad2Deg, targetAngle * Mathf.Rad2Deg)) < 90f);
        
        Vector3 startPos = wheelCenter.position + new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle)) * wheelRadius;
        rb.position = startPos;
        
        float circlingTime = rollDuration * 0.7f;
        float elapsedTime = 0f;
        float maxTime = circlingTime;
        bool approachingTarget = false;
        
        while (elapsedTime < maxTime && !approachingTarget)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / circlingTime;
            
            float currentSpeed = initialCirclingSpeed * (1 - speedDecayCurve.Evaluate(t));
            
            Vector3 dirToCenter = rb.position - wheelCenter.position;
            dirToCenter.y = 0;
            float currentAngle = Mathf.Atan2(dirToCenter.z, dirToCenter.x);
            
            float angleDifference = Mathf.Abs(Mathf.DeltaAngle(currentAngle * Mathf.Rad2Deg, targetAngle * Mathf.Rad2Deg));
            
            if (elapsedTime > circlingTime * 0.5f && angleDifference < targetApproachThreshold)
            {
                approachingTarget = true;
                break;
            }
            
            float angleChange = currentSpeed * Time.deltaTime;
            
            float newAngle = currentAngle + angleChange;
            Vector3 newPos = wheelCenter.position + new Vector3(Mathf.Cos(newAngle), 0, Mathf.Sin(newAngle)) * wheelRadius;
            
            rb.velocity = (newPos - rb.position) / Time.deltaTime;
            rb.position = newPos;
            
            yield return null;
        }
        
        float guidedTime = rollDuration * 0.3f;
        elapsedTime = 0f;
        Vector3 startPosition = rb.position;
        
        Vector3 midPoint = Vector3.Lerp(startPosition, targetPosition, 0.5f);
        midPoint = Vector3.Lerp(midPoint, wheelCenter.position, 0.2f);
        
        while (elapsedTime < guidedTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / guidedTime;
            
            float oneMinusT = 1f - t;
            Vector3 bezierPos = oneMinusT * oneMinusT * startPosition + 
                               2f * oneMinusT * t * midPoint + 
                               t * t * targetPosition;
            
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, t * t);
            rb.position = bezierPos;
            
            yield return null;
        }
        
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.position = targetPosition;
        
        isRolling = false;
        
        GetComponentInParent<RouletteController>().OnSpinCompleted();
    }
    
}