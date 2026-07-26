using UnityEngine;

public class QTEVisual : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform shrinkingRing;
    [SerializeField] private Transform windowBackground;

    private float backgroundScale;

    private float totalDuration;
    private float elapsed;

    private bool running;

    public void Initialise(float totalDuration, float perfectDuration)
    {
        this.totalDuration = Mathf.Max(0.0001f, totalDuration);

        elapsed = 0f;
        running = true;

        backgroundScale = perfectDuration / this.totalDuration;
        shrinkingRing.localScale = Vector3.one;
        windowBackground.localScale = Vector3.one * backgroundScale;
    }

    public void Tick(float scaledDeltaTime)
    {
        if (!running)
        {
            return;
        }

        elapsed += scaledDeltaTime;

        float t = Mathf.Clamp01(elapsed / totalDuration);
        float scale = Mathf.Lerp(1f, 0f, t);
        shrinkingRing.localScale = Vector3.one * scale;
    }

    public void Stop()
    {
        running = false;
    }
}