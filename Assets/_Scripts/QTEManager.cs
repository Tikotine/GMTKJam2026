using System;
using System.Collections;
using UnityEngine;

public class QTEManager : MonoBehaviour
{
    [Header("Attack Settings")]
    public int breakDuration;
    public int tempo;

    [Header("QTE")]
    [SerializeField] private QTECotnroller qtePrefab;

    public event Action<int, int> OnAttackSequenceFinished;
    public event Action<int, int> OnDefendSequenceFinished;

    [Header("Result")]
    private QTECotnroller.QTEResult currentResult;

    private int currentAttack;
    private bool attackInProgress;

    private Player playerScript;

    public enum SequenceType
    { 
        ATTACK,
        DEFEND
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerScript = FindAnyObjectByType<Player>();
        StartDefendRoutine(3, playerScript);
    }

    public void StartAttackSequence(int attackCount)
    {
        StartCoroutine(AttackRoutine(attackCount));
    }

    public void StartDefendRoutine(int incomingAttacks, Player player)
    {
        StartCoroutine(DefendRoutine(incomingAttacks, player));
    }


    //Attack
    private IEnumerator AttackRoutine(int attackCount)
    {
        int successfulHits = 0;
        int perfectHits = 0;

        for (int i = 0; i < attackCount; i++)
        {
            yield return BreakTimer();
            yield return StartQTE();

            switch (currentResult)
            { 
                case QTECotnroller.QTEResult.SUCCESS:
                    successfulHits++;
                    break;

                case QTECotnroller.QTEResult.PERFECT:
                    perfectHits++; 
                    break;

                case QTECotnroller.QTEResult.MISS:
                    Debug.Log("Attack Interrupted");
                    i = attackCount; //Exit Loop
                    break;
            }
        }

        Debug.Log($"Attack Finished. Success: {successfulHits} , Perfect: {perfectHits}");

        OnAttackSequenceFinished?.Invoke(successfulHits, perfectHits);

        yield break;
    }

    private IEnumerator DefendRoutine(int incomingAttacks, Player player)
    {
        int successfulParries = 0;
        int perfectParries = 0;

        for (int i = 0; i < incomingAttacks; i++)
        {
            yield return BreakTimer();
            yield return StartQTE();

            switch (currentResult)
            { 
                case QTECotnroller.QTEResult.SUCCESS:
                    successfulParries++;
                    break;

                case QTECotnroller .QTEResult.PERFECT:
                    perfectParries++;
                    break;

                case QTECotnroller.QTEResult.MISS:
                    break;
            }
        }

        Debug.Log($"Defend Sequence Finished Success: {successfulParries} , Perfect: {perfectParries}" );

        OnDefendSequenceFinished?.Invoke(successfulParries, perfectParries);

        yield break;
    }

    private IEnumerator BreakTimer()
    { 
        float timer = breakDuration;

        while (timer > 0) 
        {
            timer -= Time.deltaTime * tempo;
            yield return null;
        }

        yield break;
    }

    private IEnumerator StartQTE()
    {
        QTECotnroller currentQTE = Instantiate(qtePrefab);

        bool qteFinished = false;

        currentQTE.OnQTEFinished += () =>
        {
            currentResult = currentQTE.qteResult;

            Debug.Log("Manager Received "+ currentResult);

            qteFinished = true;
        };

        currentQTE.StartQTE();

        //Wait for QTE To finish
        yield return new WaitUntil(() => qteFinished);

        Destroy(currentQTE.gameObject);

        yield break;
    }

}
    