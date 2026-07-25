using UnityEngine;

public class QTEVisual : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform shrinkingRing;
    [SerializeField] private Transform windowBackground;

    [Header("Window Display")]
    private float backgroundScale;

    private float totalDuration;
    private float elapsed;

    private bool running;

    public void Initialise(float totalDuration, float perfectDuration)
    {
        this.totalDuration = totalDuration;

        elapsed = 0f;
        running = true;

        backgroundScale = perfectDuration / totalDuration;

        shrinkingRing.localScale = Vector3.one;
        windowBackground.localScale = Vector3.one * backgroundScale;
    }

    public void Tick(float deltaTime)
    {
        if (!running)
            return;

        elapsed += deltaTime;

        float t = Mathf.Clamp01(elapsed / totalDuration);

        float scale = Mathf.Lerp(1f, 0f, t);

        shrinkingRing.localScale = Vector3.one * scale;
    }

    public void Stop()
    {
        running = false;
    }
}