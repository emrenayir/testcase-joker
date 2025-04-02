using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouletteBall : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform wheelCenter;
    [SerializeField] private List<Transform> circlePoints;
    [SerializeField] private RouletteSlot[] numberSlotPositions;
    [SerializeField] private float rollDuration = 8f;
    [SerializeField] private float wheelRadius = 2f;
    [SerializeField] private float initialCirclingSpeed = 15f;
    [SerializeField] private float targetApproachThreshold = 10f;
    [SerializeField] private float maxBounceHeight = 0.15f;

    private int targetNumber = -1;
    private bool isRolling = false;
    private Vector3 targetPosition;
    private float targetAngle;
    private Vector3 lastMoveDirection;

    private struct BounceInfo
    {
        public float startTime;
        public float height;
        public float duration;
    }

    public void StartRolling(int targetNum)
    {
        if (isRolling) return;

        if (numberSlotPositions == null || numberSlotPositions.Length == 0)
        {
            Debug.LogError("numberSlotPositions array is null or empty");
            return;
        }

        targetNumber = FindTargetSlotIndex(targetNum);
        if (targetNumber == -1) return;

        StartCoroutine(RollBallCoroutine());
    }

    private int FindTargetSlotIndex(int targetNum)
    {
        for (int i = 0; i < numberSlotPositions.Length; i++)
        {
            if (numberSlotPositions[i] != null && numberSlotPositions[i].SlotNumber == targetNum)
            {
                return i;
            }
        }

        Debug.LogError("Could not find slot with number: " + targetNum);
        return -1;
    }

    private void UpdateTargetPosition()
    {
        targetPosition = numberSlotPositions[targetNumber].transform.position;
    }

    private IEnumerator RollBallCoroutine()
    {
        isRolling = true;
        PlaceBallAtRandomPosition();
        yield return CircleAroundWheel();
        yield return ApproachTargetWithBounces();
        yield return SettleOnTarget();
        isRolling = false;
        var rouletteController = GetComponentInParent<RouletteController>();
        if (rouletteController != null)
        {
            rouletteController.OnSpinCompleted();
        }
        else
        {
            Debug.LogError("RouletteController not found");
        }
    }

    private void PlaceBallAtRandomPosition()
    {
        float randomAngle;
        do
        {
            randomAngle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
        } while (Mathf.Abs(Mathf.DeltaAngle(randomAngle * Mathf.Rad2Deg, targetAngle * Mathf.Rad2Deg)) < 90f);

        transform.position = wheelCenter.position + new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle)) * wheelRadius;
    }

    private IEnumerator CircleAroundWheel()
    {
        float circlingTime = rollDuration * 0.7f;
        float elapsedTime = 0f;
        float maxAllowedTime = rollDuration * 2f;
        float rotationProgress = 0f;
        float rotationSpeed = 2.0f;
        transform.position = circlePoints[0].position;

        bool reachedTarget = false;
        while (!reachedTarget && elapsedTime < maxAllowedTime)
        {
            elapsedTime += Time.deltaTime;
            UpdateTargetPosition();

            rotationProgress += rotationSpeed * Time.deltaTime;
            if (rotationSpeed > 0.2f)
            {
                rotationSpeed -= Time.deltaTime;
            }

            float normalizedAngle = rotationProgress % 1.0f;
            float segmentCount = 4.0f;
            float totalProgress = normalizedAngle * segmentCount;
            int segmentIndex = Mathf.FloorToInt(totalProgress);
            float segmentProgress = totalProgress - segmentIndex;

            int pointIndex1 = segmentIndex % circlePoints.Count;
            int pointIndex2 = (segmentIndex + 1) % circlePoints.Count;

            Vector3 center = wheelCenter.position;
            Vector3 radiusVector1 = circlePoints[pointIndex1].position - center;
            Vector3 radiusVector2 = circlePoints[pointIndex2].position - center;

            Vector3 slerpedDir = Vector3.Slerp(radiusVector1.normalized, radiusVector2.normalized, segmentProgress);
            float radius = radiusVector1.magnitude;
            Vector3 newPosition = center + slerpedDir * radius;
            newPosition.y = transform.position.y;

            transform.position = newPosition;

            var distanceToTarget = Vector3.Distance(transform.position, targetPosition);
            Debug.Log("Distance to target: " + distanceToTarget);

            if (distanceToTarget < targetApproachThreshold && elapsedTime > circlingTime * 0.3f)
            {
                reachedTarget = true;
            }

            yield return null;
        }
    }

    private IEnumerator ApproachTargetWithBounces()
    {
        Debug.Log("Approaching target with bounces");
        float approachTime = rollDuration * 0.3f;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;

        Vector3[] controlPoints = CalculateBezierControlPoints(startPosition);
        BounceInfo[] bounces = GenerateRandomBounces();

        while (elapsedTime < approachTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / approachTime;

            UpdateTargetPosition();
            controlPoints[3] = targetPosition;

            Vector3 bezierPos = CalculateBezierPoint(t, controlPoints);
            float bounceAmount = CalculateBounceAmount(t, bounces);
            bezierPos.y += bounceAmount;
            bezierPos = KeepWithinWheelBounds(bezierPos, bounceAmount);

            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, t * t);
            transform.position = bezierPos;

            yield return null;
        }
    }

    private IEnumerator SettleOnTarget()
    {
        Vector3 finalStartPos = transform.position;
        float finalApproachTime = 1f;
        float elapsedTime = 0f;

        UpdateTargetPosition();

        while (elapsedTime < finalApproachTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsedTime / finalApproachTime);

            UpdateTargetPosition();

            Vector3 position = Vector3.Lerp(finalStartPos, targetPosition, t);

            if (t < 0.7f)
            {
                float finalBounce = Mathf.Sin(t * Mathf.PI * 2) * 0.02f * (1 - t);
                position.y += finalBounce;
            }

            transform.position = position;
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, t);

            yield return null;
        }

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = targetPosition;
        rb.isKinematic = true;
    }

    private Vector3[] CalculateBezierControlPoints(Vector3 startPosition)
    {
        Vector3[] controlPoints = new Vector3[4];
        controlPoints[0] = startPosition;
        controlPoints[3] = targetPosition;

        Vector3 directMidPoint = Vector3.Lerp(startPosition, targetPosition, 0.5f);
        directMidPoint += new Vector3(
            UnityEngine.Random.Range(-0.1f, 0.1f),
            0,
            UnityEngine.Random.Range(-0.1f, 0.1f)
        );

        controlPoints[1] = Vector3.Lerp(startPosition, directMidPoint, 0.5f);
        controlPoints[2] = Vector3.Lerp(directMidPoint, targetPosition, 0.5f);
        controlPoints[1] += lastMoveDirection * UnityEngine.Random.Range(0.05f, 0.15f);

        return controlPoints;
    }

    private BounceInfo[] GenerateRandomBounces()
    {
        int bounceCount = UnityEngine.Random.Range(2, 5);
        BounceInfo[] bounces = new BounceInfo[bounceCount];

        bounces[0].startTime = UnityEngine.Random.Range(0.15f, 0.3f);
        bounces[0].height = UnityEngine.Random.Range(0.7f, 1.0f) * maxBounceHeight;
        bounces[0].duration = UnityEngine.Random.Range(0.15f, 0.25f);

        for (int i = 1; i < bounceCount; i++)
        {
            float minTime = bounces[i - 1].startTime + bounces[i - 1].duration + 0.05f;
            float maxTime = minTime + 0.2f;
            if (maxTime > 0.9f) maxTime = 0.9f;

            bounces[i].startTime = Mathf.Clamp(UnityEngine.Random.Range(minTime, maxTime), 0f, 0.9f);
            bounces[i].height = UnityEngine.Random.Range(0.3f, 0.7f) * bounces[i - 1].height;
            bounces[i].duration = UnityEngine.Random.Range(0.1f, 0.2f);
        }

        return bounces;
    }

    private float CalculateBounceAmount(float t, BounceInfo[] bounces)
    {
        for (int i = 0; i < bounces.Length; i++)
        {
            if (t > bounces[i].startTime && t < bounces[i].startTime + bounces[i].duration)
            {
                float bounceProgress = (t - bounces[i].startTime) / bounces[i].duration;

                float curve;
                if (i == 0)
                {
                    curve = Mathf.Sin(bounceProgress * Mathf.PI);
                }
                else if (i == bounces.Length - 1)
                {
                    curve = 4 * bounceProgress * (1 - bounceProgress);
                }
                else
                {
                    curve = Mathf.Sin(bounceProgress * Mathf.PI);
                }

                return curve * bounces[i].height;
            }
        }

        return 0;
    }

    private Vector3 CalculateBezierPoint(float t, Vector3[] controlPoints)
    {
        float oneMinusT = 1f - t;
        return oneMinusT * oneMinusT * oneMinusT * controlPoints[0] +
               3f * oneMinusT * oneMinusT * t * controlPoints[1] +
               3f * oneMinusT * t * t * controlPoints[2] +
               t * t * t * controlPoints[3];
    }

    private Vector3 KeepWithinWheelBounds(Vector3 position, float bounceAmount)
    {
        Vector3 dirToCenter = position - wheelCenter.position;
        dirToCenter.y = 0;

        if (dirToCenter.magnitude > wheelRadius * 1.1f)
        {
            dirToCenter = dirToCenter.normalized * wheelRadius;
            position = wheelCenter.position + dirToCenter;
            position.y = transform.position.y + bounceAmount;
        }

        return position;
    }
}