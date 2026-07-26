using UnityEngine;

[System.Serializable]
public class CombatStats
{
    [Header("Base Values")]
    public int baseAttackCount;
    public float baseTempo = 1f;
    public float baseBreakDuration = 4f;

    [Header("Global Modifiers")]
    public int globalAttackCountModifier;
    public float globalTempoModifier;
    public float globalBreakDurationModifier;

    [Header("Current Dice Assignment")]
    public int attackCountDiceModifier;
    public float tempoDiceModifier;
    public float breakDurationDiceModifier;

    [Header("Final Combat Values")]
    public int attackCount;
    public float tempo;
    public float breakDuration;

    public void SetBaseValues(int attackCount, float tempo, float breakDuration)
    {
        baseAttackCount = attackCount;
        baseTempo = tempo;
        baseBreakDuration = breakDuration;
    }

    public void SetGlobalModifiers(int attackCountModifier, float tempoModifier, float breakDurationModifier)
    {
        globalAttackCountModifier = attackCountModifier;

        // Global tempo modifier can never be negative.
        globalTempoModifier = Mathf.Max(0f, tempoModifier);

        globalBreakDurationModifier = breakDurationModifier;
    }

    public void SetDiceModifiers(int attackCountModifier,float tempoModifier,float breakDurationModifier)
    {
        attackCountDiceModifier = attackCountModifier;
        tempoDiceModifier = tempoModifier;
        breakDurationDiceModifier = breakDurationModifier;
    }

    public void CalculateAttackerStats()
    {
        attackCount = baseAttackCount + globalAttackCountModifier + attackCountDiceModifier;
        tempo = CalculateTempo();
        breakDuration = baseBreakDuration + globalBreakDurationModifier - breakDurationDiceModifier;

        ClampStats();
    }

    public void CalculateDefenderStats()
    {
        attackCount = baseAttackCount + globalAttackCountModifier + attackCountDiceModifier;
        tempo = CalculateTempo();
        breakDuration = baseBreakDuration + globalBreakDurationModifier - breakDurationDiceModifier;

        ClampStats();
    }

    private float CalculateTempo()
    {
        // Formula:
        // Final Tempo =
        // Base Tempo x (Final Dice Tempo Value x 0.1)
        // + Cumulative Global Tempo Modifier

        float finalTempo = baseTempo * (tempoDiceModifier * 0.1f) + globalTempoModifier;

        return finalTempo;
    }

    public void ResetDiceModifiers()
    {
        attackCountDiceModifier = 0;
        tempoDiceModifier = 0f;
        breakDurationDiceModifier = 0f;
    }

    public void ResetGlobalModifiers()
    {
        globalAttackCountModifier = 0;
        globalTempoModifier = 0f;
        globalBreakDurationModifier = 0f;
    }

    public void ResetAllModifiers()
    {
        ResetDiceModifiers();
        ResetGlobalModifiers();
    }

    private void ClampStats()
    {
        attackCount = Mathf.Max(1, attackCount);

        // Tempo must always be positive.
        tempo = Mathf.Max(0.1f, tempo);

        breakDuration = Mathf.Max(0f, breakDuration);
    }
}