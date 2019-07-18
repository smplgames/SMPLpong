using System.Collections;
using UnityEngine;

public class GameSpeedController : MonoBehaviour
{
    public float BallSpeed = 35;
    public float PaddleSpeed = 50;
    public float IncreasePerSecond = 0.01f;

    private void Start()
    {
        PlayerController.Speed = PaddleSpeed;
        BallController.Speed = BallSpeed;
        
        StartCoroutine(IncreaseSpeed());
    }
    
    private IEnumerator IncreaseSpeed()
    {
        while (true)
        {
            PlayerController.Speed += IncreasePerSecond;
            BallController.Speed += IncreasePerSecond;
            
            yield return new WaitForSeconds(1f);
        }
    }

    private void OnDestroy()
    {
        PlayerController.Speed = PaddleSpeed;
        BallController.Speed = BallSpeed;
    }
}