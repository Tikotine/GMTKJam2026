using System;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine;

public class QTECotnroller : MonoBehaviour
{
    public enum QTEResult
    { 
        PERFECT,
        SUCCESS,
        MISS
    }

    [Header("QTE Timing")]
    public float earlyMissWindow;
    public float earlySuccessWindow;
    public float perfectWindow;
    public float lateSuccessWindow;
    public float lateMissWindow;

    public event Action OnQTEFinished;

    private bool qteActive;
    private bool inputReceived;

    [Header("References")]
    private Player playerScript;
    public QTEResult qteResult;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerScript = FindAnyObjectByType<Player>();
        playerScript.onActionPerformed += OnPlayerActionPerformed;
    }

    private void OnDestroy()
    {
        if (playerScript != null)
        {
            playerScript.onActionPerformed -= OnPlayerActionPerformed;
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
        if (!qteActive)
        {
            return;
        }

        if (inputReceived) 
        {
            return;
        }

        inputReceived = true;
    }

    public void StartQTE()
    {
        if (qteActive)
        {
            return;
        }

        StartCoroutine(QTESequence());
    }

    private IEnumerator QTESequence()
    {
        qteActive = true;
        inputReceived = false;

        //Early Miss
        yield return WaitForWindow(earlyMissWindow, QTEResult.MISS);
        if (!qteActive)
        {
            yield break;
        }

        //Early Success
        yield return WaitForWindow(earlySuccessWindow, QTEResult.SUCCESS);
        if (!qteActive)
        {
            yield break;
        }

        //Perfect
        yield return WaitForWindow(perfectWindow, QTEResult.PERFECT);
        if (!qteActive)
        {
            yield break;
        }

        //Late Success
        yield return WaitForWindow(lateSuccessWindow, QTEResult.SUCCESS);
        if (!qteActive)
        {
            yield break;
        }

        //Late Miss
        yield return WaitForWindow(lateMissWindow, QTEResult.MISS);
        if (!qteActive)
        {
            yield break;
        }

        //No Input
        qteResult = QTEResult.MISS;

        FinishQTE();
    }

    private IEnumerator WaitForWindow(float duration, QTEResult result)
    {
        float timer = duration;

        while (timer > 0) 
        {
            if (inputReceived)
            {
                qteResult = result;
                FinishQTE();

                yield break;
            }

            timer -= Time.deltaTime;

            yield return null;
        }
    }

    private void FinishQTE()
    { 
        qteActive = false;
        Debug.Log("Result: " + qteResult);  //Correct
        OnQTEFinished?.Invoke();
    }
}
