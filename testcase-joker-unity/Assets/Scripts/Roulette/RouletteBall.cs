using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouletteBall : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float rollDuration = 8f;
    [SerializeField] private RouletteSlot[] numberSlotPositions;
    [SerializeField] private Transform wheelCenter;
    [SerializeField] private float wheelRadius = 2f;
    [SerializeField] private float initialCirclingSpeed = 15f;
    [SerializeField] private AnimationCurve speedDecayCurve;
    [SerializeField] private float targetApproachThreshold = 10f;
    [SerializeField] private float maxBounceHeight = 0.15f;

    private int targetNumber = -1;
    private bool isRolling = false;
    private Vector3 targetPosition;
    private float targetAngle;
    private Vector3 lastMoveDirection;

    private enum ApproachType
    {
        Direct,
        WideCurve,
        ZigZag,
        Spiral
    }

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

        rb.isKinematic = false;
        StartCoroutine(RollBallCoroutine());
    }

    private void UpdateTargetPosition()
    {
        targetPosition = numberSlotPositions[targetNumber].transform.position;
        Vector3 dirToCenter = targetPosition - wheelCenter.position;
        dirToCenter.y = 0;
        targetAngle = Mathf.Atan2(dirToCenter.z, dirToCenter.x);
    }

    private IEnumerator RollBallCoroutine()
    {
        isRolling = true;
        UpdateTargetPosition();

        float randomAngle;
        do
        {
            randomAngle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
        } while (Mathf.Abs(Mathf.DeltaAngle(randomAngle * Mathf.Rad2Deg, targetAngle * Mathf.Rad2Deg)) < 90f);

        transform.position = wheelCenter.position + new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle)) * wheelRadius;

        float circlingTime = rollDuration * 0.7f;
        float elapsedTime = 0f;
        float currentSpeed = initialCirclingSpeed;
        float minimumCircleTime = circlingTime * 0.5f;

        while (elapsedTime < circlingTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / circlingTime;

            UpdateTargetPosition();

            Vector3 dirToCenter = transform.position - wheelCenter.position;
            dirToCenter.y = 0;
            float currentAngle = Mathf.Atan2(dirToCenter.z, dirToCenter.x);

            float targetSpeed = initialCirclingSpeed * (1 - speedDecayCurve.Evaluate(t));
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 2f);

            float newAngle = currentAngle + currentSpeed * Time.deltaTime;
            Vector3 newPos = wheelCenter.position + new Vector3(Mathf.Cos(newAngle), 0, Mathf.Sin(newAngle)) * wheelRadius;

            lastMoveDirection = (newPos - transform.position).normalized;

            rb.velocity = (newPos - transform.position) / Time.deltaTime;
            transform.position = newPos;

            float angleDifference = Mathf.Abs(Mathf.DeltaAngle(currentAngle * Mathf.Rad2Deg, targetAngle * Mathf.Rad2Deg));
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

            if ((elapsedTime > minimumCircleTime && angleDifference < targetApproachThreshold) ||
                elapsedTime > circlingTime * 0.9f)
            {
                break;
            }

            yield return null;
        }

        float approachTime = rollDuration * 0.3f;
        elapsedTime = 0f;
        Vector3 startPosition = transform.position;

        ApproachType approachType = (ApproachType)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(ApproachType)).Length);

        float pathOffset = UnityEngine.Random.Range(0.1f, 0.3f);
        float curveStrength = UnityEngine.Random.Range(0.1f, 0.3f);
        float randomAngleOffset = UnityEngine.Random.Range(-30f, 30f) * Mathf.Deg2Rad;

        Vector3[] controlPoints = new Vector3[4];
        controlPoints[0] = startPosition;
        controlPoints[3] = targetPosition;
        approachType = ApproachType.Direct;
        switch (approachType)
        {
            case ApproachType.Direct:
                Vector3 directMidPoint = Vector3.Lerp(startPosition, targetPosition, 0.5f);
                directMidPoint += new Vector3(
                    UnityEngine.Random.Range(-0.1f, 0.1f),
                    0,
                    UnityEngine.Random.Range(-0.1f, 0.1f)
                );

                controlPoints[1] = Vector3.Lerp(startPosition, directMidPoint, 0.5f);
                controlPoints[2] = Vector3.Lerp(directMidPoint, targetPosition, 0.5f);
                break;

            case ApproachType.WideCurve:
                Vector3 perpendicularDir = Vector3.Cross(Vector3.up, (targetPosition - startPosition).normalized);
                perpendicularDir = Quaternion.AngleAxis(randomAngleOffset * Mathf.Rad2Deg, Vector3.up) * perpendicularDir;

                float distanceToTarget = Vector3.Distance(startPosition, targetPosition);
                Vector3 curveCenter = Vector3.Lerp(startPosition, targetPosition, 0.5f);
                Vector3 curveOffset = perpendicularDir * distanceToTarget * curveStrength;

                controlPoints[1] = Vector3.Lerp(startPosition, curveCenter, 0.3f) + curveOffset * 0.5f;
                controlPoints[2] = Vector3.Lerp(curveCenter, targetPosition, 0.7f) + curveOffset * 0.3f;
                break;

            case ApproachType.ZigZag:
                Vector3 zigDir1 = Quaternion.AngleAxis(30, Vector3.up) * (targetPosition - startPosition).normalized;
                Vector3 zigDir2 = Quaternion.AngleAxis(-30, Vector3.up) * (targetPosition - startPosition).normalized;

                float zigDistance = Vector3.Distance(startPosition, targetPosition) * 0.2f;
                controlPoints[1] = startPosition + zigDir1 * zigDistance;
                controlPoints[2] = Vector3.Lerp(startPosition, targetPosition, 0.6f) + zigDir2 * zigDistance * 0.5f;
                break;

            case ApproachType.Spiral:
                Vector3 toTarget = (targetPosition - startPosition).normalized;
                Vector3 spiralCenter = Vector3.Lerp(startPosition, targetPosition, 0.5f);

                Vector3 spiralPerp = Vector3.Cross(Vector3.up, toTarget);

                controlPoints[1] = startPosition + toTarget * Vector3.Distance(startPosition, targetPosition) * 0.3f
                                + spiralPerp * pathOffset * wheelRadius * 0.7f;
                controlPoints[2] = spiralCenter + toTarget * Vector3.Distance(startPosition, targetPosition) * 0.2f
                                - spiralPerp * pathOffset * 0.3f * wheelRadius;
                break;
        }

        controlPoints[1] += lastMoveDirection * pathOffset * 0.5f;

        int bounceCount = UnityEngine.Random.Range(2, 5);
        float[] bounceTimes = new float[bounceCount];
        float[] bounceHeights = new float[bounceCount];
        float[] bounceDurations = new float[bounceCount];

        bounceTimes[0] = UnityEngine.Random.Range(0.15f, 0.3f);
        bounceHeights[0] = UnityEngine.Random.Range(0.7f, 1.0f) * maxBounceHeight;
        bounceDurations[0] = UnityEngine.Random.Range(0.15f, 0.25f);

        for (int i = 1; i < bounceCount; i++)
        {
            float minTime = bounceTimes[i - 1] + bounceDurations[i - 1] + 0.05f;
            float maxTime = minTime + 0.2f;
            if (maxTime > 0.9f) maxTime = 0.9f;

            bounceTimes[i] = Mathf.Clamp(UnityEngine.Random.Range(minTime, maxTime), 0f, 0.9f);
            bounceHeights[i] = UnityEngine.Random.Range(0.3f, 0.7f) * bounceHeights[i - 1];
            bounceDurations[i] = UnityEngine.Random.Range(0.1f, 0.2f);
        }

        UpdateTargetPosition();
        controlPoints[3] = targetPosition;

        while (elapsedTime < approachTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / approachTime;

            UpdateTargetPosition();
            controlPoints[3] = targetPosition;

            float oneMinusT = 1f - t;
            Vector3 bezierPos = oneMinusT * oneMinusT * oneMinusT * controlPoints[0] +
                              3f * oneMinusT * oneMinusT * t * controlPoints[1] +
                              3f * oneMinusT * t * t * controlPoints[2] +
                              t * t * t * controlPoints[3];

            float bounceAmount = 0;

            for (int i = 0; i < bounceCount; i++)
            {
                if (t > bounceTimes[i] && t < bounceTimes[i] + bounceDurations[i])
                {
                    float bounceProgress = (t - bounceTimes[i]) / bounceDurations[i];

                    float curve;
                    if (i == 0)
                    {
                        curve = Mathf.Sin(bounceProgress * Mathf.PI);
                    }
                    else if (i == bounceCount - 1)
                    {
                        curve = 4 * bounceProgress * (1 - bounceProgress);
                    }
                    else
                    {
                        curve = Mathf.Sin(bounceProgress * Mathf.PI);
                    }

                    bounceAmount = curve * bounceHeights[i];
                    break;
                }
            }

            bezierPos.y += bounceAmount;

            Vector3 dirToCenter = bezierPos - wheelCenter.position;
            dirToCenter.y = 0;
            if (dirToCenter.magnitude > wheelRadius * 1.1f)
            {
                dirToCenter = dirToCenter.normalized * wheelRadius;
                bezierPos = wheelCenter.position + dirToCenter;
                bezierPos.y = transform.position.y + bounceAmount;
            }

            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, t * t);
            transform.position = bezierPos;

            yield return null;
        }

        Vector3 finalStartPos = transform.position;
        Vector3 finalTargetPos = targetPosition;
        float finalApproachTime = 0.2f;
        elapsedTime = 0f;

        UpdateTargetPosition();

        while (elapsedTime < finalApproachTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsedTime / finalApproachTime);

            UpdateTargetPosition();

            transform.position = Vector3.Lerp(finalStartPos, targetPosition, t);

            if (t < 0.7f)
            {
                float finalBounce = Mathf.Sin(t * Mathf.PI * 2) * 0.02f * (1 - t);
                transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y + finalBounce,
                    transform.position.z
                );
            }

            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, t);

            yield return null;
        }

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = targetPosition;
        rb.isKinematic = true;

        isRolling = false;
        GetComponentInParent<RouletteController>().OnSpinCompleted();
    }
}