using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum Difficulty
    {
        EASY,
        MEDIUM,
        HARD
    }

    [Header("Enemy")]
    public Difficulty currentDifficulty;

    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("AI Success Chances (%)")]
    [SerializeField] private float easySuccessChance = 40f;
    [SerializeField] private float mediumSuccessChance = 70f;
    [SerializeField] private float hardSuccessChance = 90f;

    [Header("Perfect Chance (%)")]
    [SerializeField] private float easyPerfectChance = 10f;
    [SerializeField] private float mediumPerfectChance = 20f;
    [SerializeField] private float hardPerfectChance = 35f;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public QTECotnroller.QTEResult RollAIResult()
    {
        float successChance = GetSuccessChance();
        float perfectChance = GetPerfectChance();

        float roll = Random.Range(0f, 100f);

        if (roll > successChance)
            return QTECotnroller.QTEResult.MISS;

        roll = Random.Range(0f, 100f);

        if (roll < perfectChance)
            return QTECotnroller.QTEResult.PERFECT;

        return QTECotnroller.QTEResult.SUCCESS;
    }

    private float GetSuccessChance()
    {
        switch (currentDifficulty)
        {
            case Difficulty.EASY:
                return easySuccessChance;

            case Difficulty.MEDIUM:
                return mediumSuccessChance;

            case Difficulty.HARD:
                return hardSuccessChance;
        }

        return mediumSuccessChance;
    }

    private float GetPerfectChance()
    {
        switch (currentDifficulty)
        {
            case Difficulty.EASY:
                return easyPerfectChance;

            case Difficulty.MEDIUM:
                return mediumPerfectChance;

            case Difficulty.HARD:
                return hardPerfectChance;
        }

        return mediumPerfectChance;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"Enemy Taking {damage} damage");
        Debug.Log($"Enemy HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    private void Die()
    {
        Debug.Log("Enemy Died");
    }
}