using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float duration;
    float age;
    private void Update()
    {
        age += Time.deltaTime;
        if (age > duration)
            Destroy(gameObject);
    }
}
