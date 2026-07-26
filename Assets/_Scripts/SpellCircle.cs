using System.Collections;
using UnityEngine;

public class SpellCircle : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 90f;

    private float currentRotation;

    [SerializeField] private float fadeDuration;
    [SerializeField] private Vector3 maxScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        RotateCircle();
    }

    private void RotateCircle()
    {
        currentRotation += rotationSpeed * Time.deltaTime;

        if (currentRotation >= 360f)
        {
            currentRotation -= 360f;
        }

        transform.rotation = Quaternion.Euler(90f, 0f, currentRotation);
    }

    public void CircleAppear()
    {
        StartCoroutine(ScaleCircle(Vector3.zero, maxScale));
    }

    public void CircleDisappear()
    {
        StartCoroutine(ScaleCircle(maxScale, Vector3.zero));
    }

    private IEnumerator ScaleCircle(Vector3 start, Vector3 end)
    {
        float elapsedTime = 0;

        while (elapsedTime < fadeDuration) 
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime + Time.deltaTime / fadeDuration;
            t = Mathf.Clamp01(t);

            float x = EasingFunction.EaseOutCubic(start.x, end.x, t);
            float y = EasingFunction.EaseOutCubic(start.y, end.y, t);
            float z = EasingFunction.EaseOutCubic(start.z, end.z, t);

            transform.localScale = new Vector3(x, y, z);
            yield return null;
        }

        transform.localScale = end;
    }
}
