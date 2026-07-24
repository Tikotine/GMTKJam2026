using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraFlashInterval : MonoBehaviour
{
    [SerializeField] private float intervalMax;
    [SerializeField] private float intervalMin;
    [SerializeField] private float currentInterval;
    private float flashTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FlashLight();
    }

    // Update is called once per frame
    void Update()
    {
        if (flashTimer < currentInterval)
        {
            flashTimer += Time.unscaledDeltaTime;
        }

        if (flashTimer >= currentInterval)
        {
            FlashLight();
        }
    }

    private void RandomizeInterval()
    {
        currentInterval = Random.Range(intervalMin, intervalMax);
    }

    public void FlashLight()
    {
        flashTimer = 0;

        gameObject.SetActive(false);
        gameObject.SetActive(true);

        RandomizeInterval();
    }
}   
