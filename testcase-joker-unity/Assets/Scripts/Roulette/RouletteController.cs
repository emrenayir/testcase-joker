using UnityEngine;

public class RouletteController : MonoBehaviour
{
    [SerializeField] private RouletteWheel wheel;
    [SerializeField] private RouletteBall ball;
    [SerializeField] private RouletteOutcomeManager outcomeManager;
    
    private bool isSpinning = false;


    //for testing
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartSpin();
        }
    }
    public void StartSpin()
    {
        if (isSpinning) return;
        
        isSpinning = true;
        int targetNumber = outcomeManager.GetTargetNumber();
        wheel.Spin();
        ball.StartRolling(targetNumber);
    }
    
    public void OnSpinCompleted()
    {
        isSpinning = false;
    }
}