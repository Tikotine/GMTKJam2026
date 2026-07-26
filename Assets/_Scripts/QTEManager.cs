using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QTEManager : MonoBehaviour
{
    [Header("QTE")]
    [SerializeField] private QTEController qtePrefab;
    [SerializeField] private Vector3 qteSpawnLocation;

    [Header("Damage")]
    [SerializeField] private int reducedDamage = 5;
    [SerializeField] private int successDamage = 10;
    [SerializeField] private int perfectDamage = 20;

    [Header("Defend Timing")]
    [SerializeField] private float defenderParryCooldown = 0.5f;

    [Header("References")]
    private Player playerScript;
    private Enemy enemyScript;

    [Header("Defender Targets")]
    [SerializeField] private Transform playerDefenderTarget;
    [SerializeField] private Transform enemyDefenderTarget;

    [Header("Combat State")]
    private bool combatInProgress;

    public event Action<SequenceResult> OnSequenceFinished;

    public TextMeshProUGUI counter;

    [Header("Orbs")]
    private AttackOrbController playerOrbs;
    private AttackOrbController enemyOrbs;

    [Header("Animations")]
    [SerializeField] private CombatAnimationController playerAnimationCotnroller;
    [SerializeField] private CombatAnimationController enemyAnimationCotnroller;

    private void Awake()
    {
        playerScript = FindAnyObjectByType<Player>();
        enemyScript = FindAnyObjectByType<Enemy>();

        playerOrbs = playerScript.GetComponentInChildren<AttackOrbController>();
        enemyOrbs = enemyScript.GetComponentInChildren<AttackOrbController>();

        playerAnimationCotnroller = playerScript.gameObject.GetComponent<CombatAnimationController>();
        enemyAnimationCotnroller = enemyScript.gameObject.GetComponent<CombatAnimationController>();
    }

    public void StartCombatSequence(bool playerIsAttacking, int attackerAttackCount, float combatTempo, float attackerBreakDuration)
    {
        if (combatInProgress)
        {
            Debug.LogWarning("A combat sequence is already in progress.");
            return;
        }

        StartCoroutine(CombatSequence(playerIsAttacking, attackerAttackCount, combatTempo, attackerBreakDuration));
    }

    private IEnumerator CombatSequence(bool playerIsAttacking, int attackerAttackCount, float combatTempo, float attackerBreakDuration)
    {
        AttackOrbController attackerOrbs = playerIsAttacking ? playerOrbs : enemyOrbs;
        Transform defenderTarget = playerIsAttacking ? enemyDefenderTarget : playerDefenderTarget;

        Debug.Log($"Attacker is {(playerIsAttacking ? "Player" : "Enemy")}");
        Debug.Log($"Orb controller = {attackerOrbs?.gameObject.name}");
        combatInProgress = true;

        int successfulAttacks = 0;
        int perfectAttacks = 0;
        int successfulDefends = 0;
        int perfectDefends = 0;
        int totalDamage = 0;

        List<QTEController.QTEResult> attackResults = new List<QTEController.QTEResult>();

        Debug.Log("========== ATTACK PHASE ==========");

        if (attackerAttackCount <= 0)
        {
            Debug.Log("Attacker has 0 attacks.");
            Debug.Log("Attacker's turn is skipped.");

            combatInProgress = false;
            OnSequenceFinished?.Invoke(new SequenceResult(successfulAttacks, perfectAttacks, successfulDefends, perfectDefends, totalDamage));
            yield break;
        }

        for (int i = 0; i < attackerAttackCount; i++)
        {
            yield return BreakTimer(attackerBreakDuration, combatTempo);

            QTEController.QTEResult attackResult = QTEController.QTEResult.MISS;

            if (playerIsAttacking)
            {
                yield return StartPlayerQTE(combatTempo, result => attackResult = result);
            }
            else
            {
                attackResult = enemyScript.RollAIResult();
            }

            attackResults.Add(attackResult);

            Debug.Log("Attack " + (i + 1) + " Result: " + attackResult);

            switch (attackResult)
            {
                case QTEController.QTEResult.SUCCESS:
                    successfulAttacks++;
                    GetAttackerAnimationController(playerIsAttacking).PlayCast();
                    attackerOrbs.AddOrb(false);
                    break;

                case QTEController.QTEResult.PERFECT:
                    perfectAttacks++;
                    GetAttackerAnimationController(playerIsAttacking).PlayCast();
                    attackerOrbs.AddOrb(true);
                    break;

                case QTEController.QTEResult.MISS:
                    Debug.Log("Attacker missed.");
                    Debug.Log("Attack phase interrupted.");
                    i = attackerAttackCount;
                    break;
            }
        }

        int totalSuccessfulAttacks = successfulAttacks + perfectAttacks;

        Debug.Log("Attack Phase Finished.");
        Debug.Log("Successful Attacks: " + successfulAttacks);
        Debug.Log("Perfect Attacks: " + perfectAttacks);

        if (totalSuccessfulAttacks <= 0)
        {
            Debug.Log("No successful or perfect attacks.");
            Debug.Log("Attacker's turn is skipped.");

            combatInProgress = false;
            OnSequenceFinished?.Invoke(new SequenceResult(successfulAttacks, perfectAttacks, successfulDefends, perfectDefends, totalDamage));
            yield break;
        }

        Debug.Log("========== DEFEND PHASE ==========");

        int attacksToDefend = totalSuccessfulAttacks;

        for (int i = 0; i < attacksToDefend; i++)
        {
            
            if (i > 0)
            {
                yield return DefenderCooldown(defenderParryCooldown, combatTempo);
            }

            QTEController.QTEResult defendResult = QTEController.QTEResult.MISS;

            if (playerIsAttacking)
            {
                defendResult = enemyScript.RollAIResult();
            }
            else
            {
                yield return StartPlayerQTE(combatTempo, result => defendResult = result);
            }

            Debug.Log("Defend " + (i + 1) + " Result: " + defendResult);

            QTEController.QTEResult attackResult = GetSuccessfulAttackResult(attackResults, i);
            int damage = CalculateDamage(attackResult, defendResult);

            totalDamage += damage;
            ApplyDamage(playerIsAttacking, damage);

            // Move the orb toward the defending character.
            // The QTEManager has already determined who the defender is.
            yield return StartCoroutine(attackerOrbs.RemoveFirstOrb(defenderTarget));

            switch (defendResult)
            {
                case QTEController.QTEResult.SUCCESS:
                    GetDefenderAnimationController(playerIsAttacking).PlayParry();
                    successfulDefends++;
                    break;

                case QTEController.QTEResult.PERFECT:
                    GetDefenderAnimationController(playerIsAttacking).PlayParry();
                    perfectDefends++;
                    break;

                case QTEController.QTEResult.MISS:
                    GetDefenderAnimationController(playerIsAttacking).PlayFlinch();
                    break;
            }

            if (IsCombatOver())
            {
                break;
            }
        }

        Debug.Log("========== COMBAT SEQUENCE FINISHED ==========");
        Debug.Log("Total Damage: " + totalDamage);

        attackerOrbs.Clear();
        combatInProgress = false;

        OnSequenceFinished?.Invoke(new SequenceResult(successfulAttacks, perfectAttacks, successfulDefends, perfectDefends, totalDamage));
    }

    private QTEController.QTEResult GetSuccessfulAttackResult(List<QTEController.QTEResult> attackResults, int successfulAttackIndex)
    {
        int currentSuccessfulAttack = 0;

        for (int i = 0; i < attackResults.Count; i++)
        {
            QTEController.QTEResult result = attackResults[i];

            if (result != QTEController.QTEResult.SUCCESS && result != QTEController.QTEResult.PERFECT)
            {
                continue;
            }

            if (currentSuccessfulAttack == successfulAttackIndex)
            {
                return result;
            }

            currentSuccessfulAttack++;
        }

        return QTEController.QTEResult.MISS;
    }

    private IEnumerator BreakTimer(float breakDuration, float tempo)
    {
        tempo = Mathf.Max(0.1f, tempo);
        float timer = breakDuration;

        while (timer > 0f)
        {
            timer -= Time.deltaTime * tempo;
            float displayTime = Mathf.Max(timer, 0f);
            counter.text = displayTime.ToString("F2");

            yield return null;
        }

        counter.text = "0.00";
    }

    private IEnumerator DefenderCooldown(float cooldownDuration, float tempo)
    {
        tempo = Mathf.Max(0.1f, tempo);
        float timer = cooldownDuration;

        while (timer > 0f)
        {
            timer -= Time.deltaTime * tempo;

            yield return null;
        }
    }

    private IEnumerator StartPlayerQTE(float tempo, Action<QTEController.QTEResult> callback)
    {
        QTEController currentQTE = Instantiate(qtePrefab, qteSpawnLocation, Quaternion.identity);
        bool qteFinished = false;

        currentQTE.OnQTEFinished += () =>
        {
            callback(currentQTE.qteResult);

            Debug.Log("Manager Received " + currentQTE.qteResult);

            qteFinished = true;
        };

        currentQTE.StartQTE(tempo);
        yield return new WaitUntil(() => qteFinished);

        Destroy(currentQTE.gameObject);
    }

    private int CalculateDamage(QTEController.QTEResult attack, QTEController.QTEResult defend)
    {
        switch (attack)
        {
            case QTEController.QTEResult.PERFECT:
                switch (defend)
                {
                    case QTEController.QTEResult.MISS:
                        return perfectDamage;

                    case QTEController.QTEResult.SUCCESS:
                        return reducedDamage;

                    case QTEController.QTEResult.PERFECT:
                        return 0;
                }
                break;

            case QTEController.QTEResult.SUCCESS:
                switch (defend)
                {
                    case QTEController.QTEResult.MISS:
                        return successDamage;

                    case QTEController.QTEResult.SUCCESS:
                        return 0;

                    case QTEController.QTEResult.PERFECT:
                        return 0;
                }
                break;

            case QTEController.QTEResult.MISS:
                return 0;
        }

        return 0;
    }

    private void ApplyDamage(bool playerIsAttacking, int damage)
    {
        if (damage <= 0)
        {
            return;
        }

        if (playerIsAttacking)
        {
            enemyScript.TakeDamage(damage);
        }
        else
        {
            playerScript.TakeDamage(damage);
        }
    }

    private bool IsCombatOver()
    {
        if (playerScript == null || enemyScript == null)
        {
            return false;
        }

        return playerScript.health <= 0 || enemyScript.currentHealth <= 0;
    }

    public bool IsCombatInProgress()
    {
        return combatInProgress;
    }

    public class SequenceResult
    {
        public int successfulAttacks;
        public int perfectAttacks;
        public int successfulDefends;
        public int perfectDefends;
        public int totalDamage;

        public SequenceResult(int successfulAttacks, int perfectAttacks, int successfulDefends, int perfectDefends, int totalDamage)
        {
            this.successfulAttacks = successfulAttacks;
            this.perfectAttacks = perfectAttacks;
            this.successfulDefends = successfulDefends;
            this.perfectDefends = perfectDefends;
            this.totalDamage = totalDamage;
        }
    }

    private CombatAnimationController GetAttackerAnimationController(bool playerIsAttacking)
    { 
        return playerIsAttacking ? playerAnimationCotnroller : enemyAnimationCotnroller;
    }

    private CombatAnimationController GetDefenderAnimationController(bool playerIsAttacking)
    {
        return playerIsAttacking ? enemyAnimationCotnroller : playerAnimationCotnroller;
    }
}
