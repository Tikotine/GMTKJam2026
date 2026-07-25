using UnityEngine;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Player player;
    [SerializeField] private Enemy enemy;

    [Header("Turn UI")]
    [SerializeField] private Text turnText;
    [SerializeField] private Text attackerText;
    [SerializeField] private Text phaseText;
    [SerializeField] private Text suddenDeathText;

    [Header("Health UI")]
    [SerializeField] private Text playerHealthText;
    [SerializeField] private Text enemyHealthText;

    [Header("Combat Stats UI")]
    [SerializeField] private Text playerAttackCountText;
    [SerializeField] private Text playerTempoText;
    [SerializeField] private Text playerBreakDurationText;
    [SerializeField] private Text enemyAttackCountText;
    [SerializeField] private Text enemyTempoText;
    [SerializeField] private Text enemyBreakDurationText;

    [Header("Global Modifier UI")]
    [SerializeField] private Text globalAttackCountModifierText;
    [SerializeField] private Text globalTempoModifierText;
    [SerializeField] private Text globalBreakDurationModifierText;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text gameOverText;

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }

        if (player == null)
        {
            player = FindAnyObjectByType<Player>();
        }

        if (enemy == null)
        {
            enemy = FindAnyObjectByType<Enemy>();
        }
    }

    private void Update()
    {
        if (gameManager == null)
        {
            return;
        }

        UpdateTurnUI();
        UpdateHealthUI();
        UpdateCombatStatsUI();
        UpdateGlobalModifierUI();
        UpdateSuddenDeathUI();
        UpdateGameOverUI();
    }

    private void UpdateTurnUI()
    {
        if (turnText != null)
        {
            turnText.text = "Turn: " + gameManager.GetCurrentTurn();
        }

        if (attackerText != null)
        {
            attackerText.text = "Attacker: " + (gameManager.IsPlayerAttacking() ? "PLAYER" : "ENEMY");
        }

        if (phaseText != null)
        {
            phaseText.text = gameManager.IsPlayerAttacking() ? "PLAYER ATTACKING" : "PLAYER DEFENDING";
        }
    }

    private void UpdateHealthUI()
    {
        if (player != null && playerHealthText != null)
        {
            playerHealthText.text = "Player HP: " + player.health;
        }

        if (enemy != null && enemyHealthText != null)
        {
            enemyHealthText.text = "Enemy HP: " + enemy.currentHealth;
        }
    }

    private void UpdateCombatStatsUI()
    {
        CombatStats playerStats = gameManager.GetPlayerStats();
        CombatStats enemyStats = gameManager.GetEnemyStats();

        if (playerStats == null || enemyStats == null)
        {
            return;
        }

        if (playerAttackCountText != null)
        {
            playerAttackCountText.text = "Attack Count: " + playerStats.attackCount;
        }

        if (playerTempoText != null)
        {
            playerTempoText.text = "Tempo: " + playerStats.tempo.ToString("0.##");
        }

        if (playerBreakDurationText != null)
        {
            playerBreakDurationText.text = "Break Duration: " + playerStats.breakDuration.ToString("0.##");
        }

        if (enemyAttackCountText != null)
        {
            enemyAttackCountText.text = "Attack Count: " + enemyStats.attackCount;
        }

        if (enemyTempoText != null)
        {
            enemyTempoText.text = "Tempo: " + enemyStats.tempo.ToString("0.##");
        }

        if (enemyBreakDurationText != null)
        {
            enemyBreakDurationText.text = "Break Duration: " + enemyStats.breakDuration.ToString("0.##");
        }
    }

    private void UpdateGlobalModifierUI()
    {
        int attackCountModifier = gameManager.GetGlobalAttackCountModifier();
        float tempoModifier = gameManager.GetGlobalTempoModifier();
        float breakDurationModifier = gameManager.GetGlobalBreakDurationModifier();

        if (globalAttackCountModifierText != null)
        {
            globalAttackCountModifierText.text = "Global Attack Count: " + FormatModifier(attackCountModifier);
        }

        if (globalTempoModifierText != null)
        {
            globalTempoModifierText.text = "Global Tempo: " + FormatModifier(tempoModifier);
        }

        if (globalBreakDurationModifierText != null)
        {
            globalBreakDurationModifierText.text = "Global Break Duration: " + FormatModifier(breakDurationModifier);
        }
    }

    private void UpdateSuddenDeathUI()
    {
        if (suddenDeathText == null)
        {
            return;
        }

        if (gameManager.IsSuddenDeath())
        {
            suddenDeathText.gameObject.SetActive(true);
            suddenDeathText.text = "SUDDEN DEATH";
        }
        else
        {
            suddenDeathText.gameObject.SetActive(false);
        }
    }

    private void UpdateGameOverUI()
    {
        if (gameOverPanel == null)
        {
            return;
        }

        bool gameOver = player != null && enemy != null && (player.health <= 0 || enemy.currentHealth <= 0);

        gameOverPanel.SetActive(gameOver);

        if (!gameOver || gameOverText == null)
        {
            return;
        }

        if (player.health <= 0 && enemy.currentHealth <= 0)
        {
            gameOverText.text = "DRAW!";
        }
        else if (player.health <= 0)
        {
            gameOverText.text = "ENEMY WINS!";
        }
        else if (enemy.currentHealth <= 0)
        {
            gameOverText.text = "PLAYER WINS!";
        }
    }

    private string FormatModifier(float modifier)
    {
        if (modifier > 0)
        {
            return "+" + modifier.ToString("0.##");
        }

        return modifier.ToString("0.##");
    }
}