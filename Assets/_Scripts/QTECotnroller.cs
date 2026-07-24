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
    public float perfectWindow;
    public float successWindow;

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

    // Update is called once per frame
    void Update()
    {
        
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

    private void OnDestroy()
    {
        if (playerScript != null) 
        {
            playerScript.onActionPerformed -= OnPlayerActionPerformed;
        }
    }

    private IEnumerator QTESequence()
    {
        qteActive = true;
        inputReceived = false;

        //Perfect Window
        float perfectTimer = perfectWindow;

        while (perfectTimer > 0) 
        {
            if (inputReceived)
            { 
                qteResult = QTEResult.PERFECT;
                FinishQTE();

                yield break;
            }

            perfectTimer -= Time.deltaTime;

            yield return null;
        }


        //Success Window
        float successTimer = successWindow;
        inputReceived = false;

        while (successTimer > 0)
        {
            if (inputReceived)
            {
                qteResult = QTEResult.SUCCESS;
                FinishQTE();

                yield break;
            }

            successTimer -= Time.deltaTime;

            yield return null;
        }

        //Miss

        qteResult = QTEResult.MISS;

        FinishQTE();
    }

    public void StartQTE()
    {
        if (qteActive)
        {
            return;
        }

        StartCoroutine(QTESequence());
    }

    private void FinishQTE()
    { 
        qteActive = false;
        Debug.Log("Result: " + qteResult);
        OnQTEFinished?.Invoke();
    }
}
