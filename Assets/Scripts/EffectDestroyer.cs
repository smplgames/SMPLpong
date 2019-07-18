using UnityEngine;

public class EffectDestroyer : MonoBehaviour
{
    public float liveTime;

    void Update()
    {
        liveTime -= Time.deltaTime;
        if (liveTime < 0)
        {
            Destroy(gameObject);
        }
    }
}
