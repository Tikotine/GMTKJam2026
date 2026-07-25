using System;
using System.Collections;
using UnityEngine;

public class QTEController : MonoBehaviour
{
    public enum QTEResult
    {
        PERFECT,
        SUCCESS,
        MISS
    }

    [Header("QTE Timing")]
    [Tooltip("Duration of the early miss window.")]
    [SerializeField] private float earlyMissWindow = 0.5f;

    [Tooltip("Duration of the early success window.")]
    [SerializeField] private float earlySuccessWindow = 0.5f;

    [Tooltip("Duration of the perfect window.")]
    [SerializeField] private float perfectWindow = 0.25f;

    [Tooltip("Duration of the late success window.")]
    [SerializeField] private float lateSuccessWindow = 0.5f;

    [Tooltip("Duration of the late miss window.")]
    [SerializeField] private float lateMissWindow = 0.5f;

    public event Action OnQTEFinished;

    private bool qteActive;
    private bool inputReceived;
    private bool qteFinished;
    private float tempo = 1f;

    private Player playerScript;

    public QTEResult qteResult { get; private set; }

    private QTEVisual visual;

    private void Awake()
    {
        playerScript = FindAnyObjectByType<Player>();
        visual = GetComponent<QTEVisual>();
    }

    private void OnEnable()
    {
        if (playerScript != null)
        {
            playerScript.onActionPerformed += OnPlayerActionPerformed;
        }
    }

    private void OnDisable()
    {
        if (playerScript != null)
        {
            playerScript.onActionPerformed -= OnPlayerActionPerformed;
        }
    }

    private void OnPlayerActionPerformed()
    {
        if (!qteActive || inputReceived)
        {
            return;
        }

        inputReceived = true;
    }

    public void StartQTE(float qteTempo = 1f)
    {
        if (qteActive)
        {
            return;
        }

        tempo = Mathf.Max(0.1f, qteTempo);

        float totalDuration =(earlyMissWindow + earlySuccessWindow +perfectWindow + lateSuccessWindow + lateMissWindow) / tempo;

        visual.Initialise(totalDuration, perfectWindow / tempo);

        StartCoroutine(QTESequence());
    }

    private IEnumerator QTESequence()
    {
        qteActive = true;
        inputReceived = false;
        qteFinished = false;
        qteResult = QTEResult.MISS;

        yield return WaitForWindow(earlyMissWindow, QTEResult.MISS);

        if (qteFinished)
        {
            yield break;
        }

        yield return WaitForWindow(earlySuccessWindow, QTEResult.SUCCESS);

        if (qteFinished)
        {
            yield break;
        }

        yield return WaitForWindow(perfectWindow, QTEResult.PERFECT);

        if (qteFinished)
        {
            yield break;
        }

        yield return WaitForWindow(lateSuccessWindow, QTEResult.SUCCESS);

        if (qteFinished)
        {
            yield break;
        }

        yield return WaitForWindow(lateMissWindow, QTEResult.MISS);

        if (qteFinished)
        {
            yield break;
        }

        qteResult = QTEResult.MISS;
        FinishQTE();
    }

    private IEnumerator WaitForWindow(float duration, QTEResult result)
    {
        float timer = duration / tempo;

        while (timer > 0f)
        {
            if (inputReceived)
            {
                qteResult = result;
                FinishQTE();
                yield break;
            }

            float delta = Time.deltaTime;

            timer -= delta;

            visual.Tick(delta);

            yield return null;
        }
    }

    private void FinishQTE()
    {
        if (!qteActive)
        {
            return;
        }

        qteActive = false;
        qteFinished = true;

        Debug.Log("QTE Result: " + qteResult);

        visual.Stop();
        OnQTEFinished?.Invoke();
    }

    public bool IsQTEActive()
    {
        return qteActive;
    }

    public bool HasFinished()
    {
        return qteFinished;
    }
}
