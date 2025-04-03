using System.Collections;
using UnityEngine;

/// <summary>
/// This script is responsible for spinning the roulette wheel.
/// </summary>
public class RouletteWheel : MonoBehaviour
{
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

}