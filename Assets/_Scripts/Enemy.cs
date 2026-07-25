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
    public Difficulty currentDifficulty = Difficulty.MEDIUM;

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

    [Header("Dice")]
    [SerializeField] private Dice diceSlotOne;
    [SerializeField] private Dice diceSlotTwo;
    [SerializeField] private Dice diceSlotThree;

    [Header("Modifier Assignment")]
    private int diceValueOne;
    private int diceValueTwo;
    private int diceValueThree;

    private EnemyModifierAssignment modifierAssignment;

    private void Awake()
    {
        currentHealth = maxHealth;
        modifierAssignment = new EnemyModifierAssignment();
    }

    public QTEController.QTEResult RollAIResult()
    {
        float successChance = GetSuccessChance();
        float perfectChance = GetPerfectChance();

        float roll = Random.Range(0f, 100f);

        if (roll >= successChance)
        {
            return QTEController.QTEResult.MISS;
        }

        roll = Random.Range(0f, 100f);

        if (roll < perfectChance)
        {
            return QTEController.QTEResult.PERFECT;
        }

        return QTEController.QTEResult.SUCCESS;
    }

    public void RollDiceForModifierAssignment(bool isAttacking)
    {
        diceValueOne = diceSlotOne.Roll();
        diceValueTwo = diceSlotTwo.Roll();
        diceValueThree = diceSlotThree.Roll();

        modifierAssignment = new EnemyModifierAssignment();

        AssignModifiers(isAttacking);

        //Debug.Log("Enemy Dice 1: " + diceValueOne);
        //Debug.Log("Enemy Dice 2: " + diceValueTwo);
        //Debug.Log("Enemy Dice 3: " + diceValueThree);

        Debug.Log("Enemy Modifier Assignment: Attack Count " + modifierAssignment.attackCountModifier + ", Tempo " + modifierAssignment.tempoModifier + ", Break Duration " + modifierAssignment.breakDurationModifier);
    }

    private void AssignModifiers(bool isAttacking)
    {
        int[] diceValues = { diceValueOne, diceValueTwo, diceValueThree };
        int[] sortedDice = SortDiceDescending(diceValues);

        switch (currentDifficulty)
        {
            case Difficulty.EASY:
                modifierAssignment.attackCountModifier = sortedDice[2];
                modifierAssignment.tempoModifier = sortedDice[1];
                modifierAssignment.breakDurationModifier = sortedDice[0];
                break;

            case Difficulty.MEDIUM:
                modifierAssignment.attackCountModifier = isAttacking ? sortedDice[0] : sortedDice[1];
                modifierAssignment.tempoModifier = sortedDice[1];
                modifierAssignment.breakDurationModifier = isAttacking ? sortedDice[2] : sortedDice[0];
                break;

            case Difficulty.HARD:
                modifierAssignment.attackCountModifier = isAttacking ? sortedDice[1] : sortedDice[2];
                modifierAssignment.tempoModifier = sortedDice[0];
                modifierAssignment.breakDurationModifier = isAttacking ? sortedDice[2] : sortedDice[0];
                break;
        }
    }

    public EnemyModifierAssignment GetModifierAssignment()
    {
        return modifierAssignment;
    }

    private int[] SortDiceDescending(int[] diceValues)
    {
        int[] sortedDice = new int[diceValues.Length];

        for (int i = 0; i < diceValues.Length; i++)
        {
            sortedDice[i] = diceValues[i];
        }

        for (int i = 0; i < sortedDice.Length - 1; i++)
        {
            for (int j = i + 1; j < sortedDice.Length; j++)
            {
                if (sortedDice[j] > sortedDice[i])
                {
                    int temporaryValue = sortedDice[i];
                    sortedDice[i] = sortedDice[j];
                    sortedDice[j] = temporaryValue;
                }
            }
        }

        return sortedDice;
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

            default:
                return mediumSuccessChance;
        }
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

            default:
                return mediumPerfectChance;
        }
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || currentHealth <= 0)
        {
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log("Enemy Taking " + damage + " damage");
        Debug.Log("Enemy HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || currentHealth <= 0)
        {
            return;
        }

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    private void Die()
    {
        Debug.Log("Enemy Died");
    }
}

[System.Serializable]
public class EnemyModifierAssignment
{
    public int attackCountModifier;
    public float tempoModifier;
    public float breakDurationModifier;
}
