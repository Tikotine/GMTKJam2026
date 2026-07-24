using System.Collections;
using UnityEngine;

public class QTEManager : MonoBehaviour
{
    [Header("Attack Settings")]
    public int attackNumber;
    public int breakDuration;
    public int tempo;

    [Header("QTE")]
    public QTECotnroller qtePrefab;

    private int currentAttack;
    private bool attackInProgress;

    //[Header("Break")]


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        attackInProgress = true;
        currentAttack = 0;

        while (currentAttack < attackNumber) 
        {
            //Wait before starting QTE
            yield return BreakTimer();

            //Start QTE
            yield return StartQTE();
            
            currentAttack++;
        }

        attackInProgress = false;
        Debug.Log("Attack Sequence Complete");
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
            qteFinished = true;
        };

        currentQTE.StartQTE();

        //Wait for QTE To finish
        yield return new WaitUntil(() => qteFinished);

        Destroy(currentQTE.gameObject);

    }
}
    