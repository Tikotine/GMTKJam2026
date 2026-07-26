using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum CoinSide
    {
        HEADS,
        TAILS
    }

    [Header("Game Settings")]
    [SerializeField] private int maxTurns = 10;
    [SerializeField] private int modifierInterval = 2;

    [Header("Base Combat Values")]
    [SerializeField] private int baseAttackCount = 0;
    [SerializeField] private float baseTempo = 1f;
    [SerializeField] private float baseBreakDuration = 4f;

    [Header("Sudden Death")]
    [SerializeField] private bool suddenDeathModifiersFavorAttacker = true;

    [Header("References")]
    [SerializeField] private QTEManager qteManager;
    [SerializeField] private Player playerScript;
    [SerializeField] private Enemy enemyScript;
    [SerializeField] private CoinTossUI coinTossUI;

    [Header("Combat Stats")]
    [SerializeField] private CombatStats playerStats;
    [SerializeField] private CombatStats enemyStats;

    [Header("Turn State")]
    private int currentTurn;
    private bool playerIsAttacking;
    private bool gameInProgress;
    private bool waitingForCombatSequence;

    [Header("Coin Toss")]
    private CoinSide playerCoinChoice;
    private CoinSide coinResult;
    private bool coinChoiceMade;

    [Header("Sudden Death")]
    private bool suddenDeath;

    [Header("Global Modifiers")]
    private int globalAttackCountModifier;
    private float globalTempoModifier;
    private float globalBreakDurationModifier;

    private float currentCombatTempo;

    private void Awake()
    {
        if (playerScript == null)
        {
            playerScript = FindAnyObjectByType<Player>();
        }

        if (enemyScript == null)
        {
            enemyScript = FindAnyObjectByType<Enemy>();
        }

        if (qteManager == null)
        {
            qteManager = FindAnyObjectByType<QTEManager>();
        }

        if (coinTossUI == null)
        {
            coinTossUI = FindAnyObjectByType<CoinTossUI>();
        }

        playerStats = new CombatStats();
        enemyStats = new CombatStats();

        InitializeCombatStats();
    }

    private void OnEnable()
    {
        if (qteManager != null)
        {
            qteManager.OnSequenceFinished += OnCombatSequenceFinished;
        }
    }

    private void OnDisable()
    {
        if (qteManager != null)
        {
            qteManager.OnSequenceFinished -= OnCombatSequenceFinished;
        }
    }

    private void InitializeCombatStats()
    {
        playerStats.SetBaseValues(baseAttackCount, baseTempo, baseBreakDuration);
        enemyStats.SetBaseValues(baseAttackCount, baseTempo, baseBreakDuration);
    }

    public void StartGame()
    {
        if (gameInProgress)
        {
            Debug.LogWarning("Game is already in progress.");
            return;
        }

        currentTurn = 0;
        suddenDeath = false;
        gameInProgress = true;
        waitingForCombatSequence = false;

        globalAttackCountModifier = 0;
        globalTempoModifier = 0f;
        globalBreakDurationModifier = 0f;

        playerStats.ResetAllModifiers();
        enemyStats.ResetAllModifiers();

        Debug.Log("========== GAME START ==========");

        if (!coinChoiceMade)
        {
            Debug.LogWarning("Player has not selected a coin side.");
            return;
        }

        PerformCoinToss();
    }

    public void SetPlayerCoinChoice(CoinSide choice)
    {
        if (gameInProgress)
        {
            Debug.LogWarning("Cannot change coin choice while the game is in progress.");
            return;
        }

        playerCoinChoice = choice;
        coinChoiceMade = true;

        Debug.Log("Player selected: " + playerCoinChoice);
    }

    public void SelectHeads()
    {
        SetPlayerCoinChoice(CoinSide.HEADS);
    }

    public void SelectTails()
    {
        SetPlayerCoinChoice(CoinSide.TAILS);
    }

    public void RegisterCoinTossUI(CoinTossUI ui)
    {
        coinTossUI = ui;
    }

    public void PerformCoinToss()
    {
        if (!gameInProgress)
        {
            Debug.LogWarning("Cannot perform coin toss because the game has not started.");
            return;
        }

        if (!coinChoiceMade)
        {
            Debug.LogWarning("Player must select Heads or Tails first.");
            return;
        }

        coinResult = GetRandomCoinSide();

        if (coinTossUI != null)
        {
            coinTossUI.DisplayCoinResult(coinResult);
        }

        Debug.Log("Coin toss result: " + coinResult);

        if (playerCoinChoice == coinResult)
        {
            playerIsAttacking = true;
            Debug.Log("Player won the coin toss.");
            Debug.Log("Player will attack first.");
        }
        else
        {
            playerIsAttacking = false;
            Debug.Log("Player lost the coin toss.");
            Debug.Log("Player will defend first.");
        }

        StartCoroutine(BeginNextTurn());
    }

    private CoinSide GetRandomCoinSide()
    {
        return Random.Range(0, 2) == 0 ? CoinSide.HEADS : CoinSide.TAILS;
    }

    private IEnumerator BeginNextTurn()
    {
        if (!gameInProgress)
        {
            yield break;
        }

        if (IsGameOver())
        {
            EndGame();
            yield break;
        }

        if (!suddenDeath && currentTurn >= maxTurns)
        {
            EnterSuddenDeath();
            yield break;
        }

        currentTurn++;

        Debug.Log("================================");
        Debug.Log("STARTING TURN " + currentTurn);
        Debug.Log("================================");

        yield return ModifierAssignmentPhase();

        CalculateCombatStats();

        StartCombatSequence();
    }

    private IEnumerator ModifierAssignmentPhase()
    {
        Debug.Log("========== MODIFIER ASSIGNMENT ==========");

        bool shouldApplyModifier = suddenDeath || (modifierInterval > 0 && currentTurn % modifierInterval == 0);
        if (shouldApplyModifier) ApplyGlobalModifier();
        else Debug.Log("No global modifier this turn.");

        playerScript.PlayerRollDice();
        enemyScript.RollDiceForModifierAssignment(!playerIsAttacking);

        yield return new WaitUntil(() => playerScript.HasAssignedAllModifiers());

        ApplyDiceModifiers();

        Debug.Log("Modifier assignment completed.");
    }

    private void ApplyDiceModifiers()
    {
        int playerAttackCountModifier = playerScript.GetAssignedAttackCountModifier();
        float playerTempoModifier = playerScript.GetAssignedTempoModifier();
        float playerBreakDurationModifier = playerScript.GetAssignedBreakDurationModifier();

        EnemyModifierAssignment enemyAssignment = enemyScript.GetModifierAssignment();

        playerStats.SetDiceModifiers(playerAttackCountModifier, playerTempoModifier, playerBreakDurationModifier);
        enemyStats.SetDiceModifiers(enemyAssignment.attackCountModifier, enemyAssignment.tempoModifier, enemyAssignment.breakDurationModifier);

        Debug.Log("Player Dice Modifiers: Attack Count " + playerAttackCountModifier + ", Tempo " + playerTempoModifier + ", Break Duration " + playerBreakDurationModifier);
        Debug.Log("Enemy Dice Modifiers: Attack Count " + enemyAssignment.attackCountModifier + ", Tempo " + enemyAssignment.tempoModifier + ", Break Duration " + enemyAssignment.breakDurationModifier);
    }

    private void ApplyGlobalModifier()
    {
        int modifierType = Random.Range(0, 3);
        int modifierAmount = Random.Range(1, 4);
        bool positiveModifier = Random.Range(0, 2) == 0;

        if (suddenDeath && suddenDeathModifiersFavorAttacker)
        {
            modifierType = Random.Range(0, 3);
            positiveModifier = true;

            if (modifierType == 2)
            {
                positiveModifier = false;
            }
        }

        switch (modifierType)
        {
            case 0:
                int attackCountChange = positiveModifier ? modifierAmount : -modifierAmount;
                globalAttackCountModifier += attackCountChange;
                Debug.Log("GLOBAL MODIFIER: Attack Count " + FormatModifier(attackCountChange));
                break;

            case 1:
                float tempoChange = positiveModifier ? modifierAmount : -modifierAmount;
                globalTempoModifier = Mathf.Max(0f,globalTempoModifier + tempoChange);
                Debug.Log("GLOBAL MODIFIER: Tempo Change " + FormatModifier(tempoChange) + " | Cumulative Tempo Modifier: " + globalTempoModifier);
                break;

            case 2:
                float breakDurationChange = positiveModifier ? -modifierAmount : modifierAmount;
                globalBreakDurationModifier += breakDurationChange;
                Debug.Log("GLOBAL MODIFIER: Break Duration " + FormatModifier(breakDurationChange));
                break;
        }

        playerStats.SetGlobalModifiers(globalAttackCountModifier, globalTempoModifier, globalBreakDurationModifier);
        enemyStats.SetGlobalModifiers(globalAttackCountModifier, globalTempoModifier, globalBreakDurationModifier);
    }

    private string FormatModifier(float modifier)
    {
        return modifier >= 0 ? "+" + modifier : modifier.ToString();
    }

    private void CalculateCombatStats()
    {
        Debug.Log("========== CALCULATING COMBAT STATS ==========");

        // Calculate individual stats.
        playerStats.CalculateAttackerStats();
        enemyStats.CalculateDefenderStats();

        // The attacking side determines the tempo for the entire turn.
        // This same tempo is used during:
        // - Attack break timers
        // - Attack QTEs
        // - Defender cooldowns
        // - Defend QTEs
        if (playerIsAttacking)
        {
            currentCombatTempo = playerStats.tempo;
        }
        else
        {
            currentCombatTempo = enemyStats.tempo;
        }

        currentCombatTempo = Mathf.Max(0.1f, currentCombatTempo);

        Debug.Log("Player Stats: Attack Count = " + playerStats.attackCount + ", Tempo = " + playerStats.tempo + ", Break Duration = " + playerStats.breakDuration);
        Debug.Log("Enemy Stats: Attack Count = " + enemyStats.attackCount + ", Tempo = " + enemyStats.tempo + ", Break Duration = " + enemyStats.breakDuration);
        Debug.Log("SHARED COMBAT TEMPO: " + currentCombatTempo);
    }

    private void StartCombatSequence()
    {
        if (waitingForCombatSequence)
        {
            return;
        }

        if (IsGameOver())
        {
            EndGame();
            return;
        }

        waitingForCombatSequence = true;

        int attackerAttackCount;
        float attackerBreakDuration;

        if (playerIsAttacking)
        {
            attackerAttackCount = playerStats.attackCount;
            attackerBreakDuration = playerStats.breakDuration;
        }
        else
        {
            attackerAttackCount = enemyStats.attackCount;
            attackerBreakDuration = enemyStats.breakDuration;
        }

        Debug.Log("========== COMBAT SEQUENCE START ==========");
        Debug.Log("Attacker: " + (playerIsAttacking ? "PLAYER" : "ENEMY"));

        Debug.Log("Shared Combat Tempo: " + currentCombatTempo);

        qteManager.StartCombatSequence(playerIsAttacking, attackerAttackCount, currentCombatTempo, attackerBreakDuration);
    }

    private void OnCombatSequenceFinished(QTEManager.SequenceResult result)
    {
        waitingForCombatSequence = false;

        Debug.Log("========== COMBAT SEQUENCE FINISHED ==========");
        Debug.Log("Successful Attacks: " + result.successfulAttacks);
        Debug.Log("Perfect Attacks: " + result.perfectAttacks);
        Debug.Log("Successful Defends: " + result.successfulDefends);
        Debug.Log("Perfect Defends: " + result.perfectDefends);
        Debug.Log("Total Damage: " + result.totalDamage);

        if (IsGameOver())
        {
            EndGame();
            return;
        }

        SwapAttackerAndDefender();

        StartCoroutine(BeginNextTurn());
    }

    private void SwapAttackerAndDefender()
    {
        playerIsAttacking = !playerIsAttacking;

        Debug.Log("Sides swapped.");
        Debug.Log("New Attacker: " + (playerIsAttacking ? "PLAYER" : "ENEMY"));
    }

    private void EnterSuddenDeath()
    {
        if (suddenDeath)
        {
            return;
        }

        suddenDeath = true;

        Debug.Log("================================");
        Debug.Log("ENTERING SUDDEN DEATH");
        Debug.Log("Global modifiers now apply every turn.");
        Debug.Log("Modifiers favor the attacking side.");
        Debug.Log("================================");

        StartCoroutine(BeginNextTurn());
    }

    private bool IsGameOver()
    {
        if (playerScript == null || enemyScript == null)
        {
            return false;
        }

        return playerScript.health <= 0 || enemyScript.currentHealth <= 0;
    }

    private void EndGame()
    {
        if (!gameInProgress)
        {
            return;
        }

        gameInProgress = false;
        waitingForCombatSequence = false;

        Debug.Log("========== GAME OVER ==========");

        if (playerScript.health <= 0 && enemyScript.currentHealth <= 0)
        {
            Debug.Log("DRAW!");
        }
        else if (playerScript.health <= 0)
        {
            Debug.Log("ENEMY WINS!");
        }
        else if (enemyScript.currentHealth <= 0)
        {
            Debug.Log("PLAYER WINS!");
        }
    }

    public int GetCurrentTurn()
    {
        return currentTurn;
    }

    public bool IsPlayerAttacking()
    {
        return playerIsAttacking;
    }

    public bool IsSuddenDeath()
    {
        return suddenDeath;
    }

    public bool IsGameInProgress()
    {
        return gameInProgress;
    }

    public int GetGlobalAttackCountModifier()
    {
        return globalAttackCountModifier;
    }

    public float GetGlobalTempoModifier()
    {
        return globalTempoModifier;
    }

    public float GetGlobalBreakDurationModifier()
    {
        return globalBreakDurationModifier;
    }

    public CombatStats GetPlayerStats()
    {
        return playerStats;
    }

    public CombatStats GetEnemyStats()
    {
        return enemyStats;
    }
}
