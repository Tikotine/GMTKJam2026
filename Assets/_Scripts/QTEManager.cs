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
    private QTECotnroller.QTEResult attackResult;
    private QTECotnroller.QTEResult defendResult;

    [Header("Enemy")]
    private Enemy enemyScript;

    [SerializeField] private int reducedDamage;
    [SerializeField] private int successDamage;
    [SerializeField] private int perfectDamage;

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
        enemyScript = FindAnyObjectByType<Enemy>();
        //StartDefendRoutine(3, playerScript);
        StartEnemyAttackRoutine(3);
    }

    public void StartAttackSequence(int attackCount)
    {
        StartCoroutine(AttackRoutine(attackCount));
    }

    public void StartDefendRoutine(int incomingAttacks, Player player)
    {
        StartCoroutine(DefendRoutine(incomingAttacks, player));
    }

    public void StartEnemyAttackRoutine(int attackCount)
    {
        StartCoroutine(EnemyAttackRoutine(attackCount));    
    }

    public void StartEnemyDefendRoutuine(int defendCount)
    {
        StartCoroutine(EnemyDefendRoutine(defendCount));
    }

    //Attack
    private IEnumerator AttackRoutine(int attackCount)
    {
        int successfulHits = 0;
        int perfectHits = 0;

        for (int i = 0; i < attackCount; i++)
        {
            yield return BreakTimer();
            yield return StartQTE(result => attackResult = result); //Player performs attack QTE
            defendResult = enemyScript.RollAIResult(); //Enemy Defends

            int damage = CalculateDamage(attackResult, defendResult);
            enemyScript.TakeDamage(damage);

            //Counting
            switch (attackResult)
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
            yield return StartQTE(result => defendResult = result);

            switch (defendResult)
            { 
                case QTECotnroller.QTEResult.SUCCESS:
                    successfulParries++;
                    Debug.Log("Player Defend Success");
                    break;

                case QTECotnroller .QTEResult.PERFECT:
                    Debug.Log("Player Defend Perfect");
                    perfectParries++;
                    break;

                case QTECotnroller.QTEResult.MISS:
                    Debug.Log("Player Defend Miss");
                    break;
            }
        }

        Debug.Log($"Defend Sequence Finished Success: {successfulParries} , Perfect: {perfectParries}" );

        OnDefendSequenceFinished?.Invoke(successfulParries, perfectParries);

        yield break;
    }

    private IEnumerator EnemyAttackRoutine(int attackCount)
    {
        int successfulHits = 0;
        int perfectHits = 0;

        for (int i = 0; i < attackCount; i++) 
        {
            yield return BreakTimer();

            QTECotnroller.QTEResult enemyAttack = enemyScript.RollAIResult(); //Enemy Attack

            yield return StartQTE(result => defendResult = result);
            int damage = CalculateDamage(enemyAttack, defendResult);

            playerScript.TakeDamage(damage);

            switch (enemyAttack) 
            {
                case QTECotnroller.QTEResult.SUCCESS:
                    successfulHits++;
                    Debug.Log("Enemy Attack Success");
                    //playerScript.TakeDamage(successDamage); 
                    break;

                case QTECotnroller.QTEResult.PERFECT:
                    perfectHits++;
                    Debug.Log("Enemy Attack Perfect");
                    //playerScript.TakeDamage(perfectDamage); 
                    break;

                case QTECotnroller.QTEResult.MISS:
                    Debug.Log("Enemy Attack Interrupted");
                    i = attackCount; //Exit Loop
                    break;
            }
        }

        OnAttackSequenceFinished?.Invoke(successfulHits, perfectHits);
    }

    private IEnumerator EnemyDefendRoutine(int attackCount)
    {
        int successfulParries = 0;
        int perfectParries = 0;

        for (int i = 0; i < attackCount; i++) 
        { 
            yield return BreakTimer();

            QTECotnroller.QTEResult defendResult = enemyScript.RollAIResult();

            switch (defendResult) 
            {
                case QTECotnroller.QTEResult.SUCCESS:
                    successfulParries++;
                    break;

                case QTECotnroller.QTEResult.PERFECT:
                    perfectParries++;
                    break;

                case QTECotnroller.QTEResult.MISS:
                    break;
            }
        }

        OnDefendSequenceFinished?.Invoke(successfulParries, perfectParries);
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

    private IEnumerator StartQTE(Action<QTECotnroller.QTEResult> callback)
    {
        QTECotnroller currentQTE = Instantiate(qtePrefab);

        bool qteFinished = false;

        currentQTE.OnQTEFinished += () =>
        {
            callback(currentQTE.qteResult);

            Debug.Log("Manager Received "+ currentQTE.qteResult);

            qteFinished = true;
        };

        currentQTE.StartQTE();

        //Wait for QTE To finish
        yield return new WaitUntil(() => qteFinished);

        Destroy(currentQTE.gameObject);

        yield break;
    }

    //Helper Function
    private int CalculateDamage(QTECotnroller.QTEResult attack, QTECotnroller.QTEResult defend)
    { 
        switch (attack) 
        {
            case QTECotnroller.QTEResult.PERFECT:

                switch (defend)
                { 
                    case QTECotnroller.QTEResult.MISS:
                        return perfectDamage;

                    case QTECotnroller.QTEResult.SUCCESS: 
                        return reducedDamage;

                    case QTECotnroller.QTEResult.PERFECT:
                        return 0;
                }

                break;


            case QTECotnroller.QTEResult.SUCCESS:

                switch (defend)
                {
                    case QTECotnroller.QTEResult.MISS:
                        return successDamage;

                    case QTECotnroller.QTEResult.SUCCESS:
                        return 0;

                    case QTECotnroller.QTEResult.PERFECT:
                        return 0;
                }

                break;

            case QTECotnroller.QTEResult.MISS:
                return 0;
        }

        return 0;
    }
}
    