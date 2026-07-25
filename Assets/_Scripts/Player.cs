using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Dice")]
    public Dice diceSlotOne;
    public Dice diceSlotTwo;
    public Dice diceSlotThree;

    [SerializeField] private int diceValueOne;
    [SerializeField] private int diceValueTwo;
    [SerializeField] private int diceValueThree;

    [Header("Player Status")]
    public int health = 100;
    [SerializeField] private float parryCooldown = 0.5f;
    private float currentParryCooldown;

    [Header("State & Actions")]
    public bool playerActions = true;
    public event Action onActionPerformed;
    public event Action onDiceRolled;

    [Header("Modifier Assignment")]
    private int assignedAttackCountModifier;
    private float assignedTempoModifier;
    private float assignedBreakDurationModifier;

    private bool attackCountAssigned;
    private bool tempoAssigned;
    private bool breakDurationAssigned;

    private void Update()
    {
        if (currentParryCooldown > 0f) currentParryCooldown -= Time.deltaTime;
    }

    public void PlayerRollDice()
    {
        diceValueOne = diceSlotOne.Roll();
        diceValueTwo = diceSlotTwo.Roll();
        diceValueThree = diceSlotThree.Roll();

        ResetModifierAssignment();

        //Debug.Log("Dice 1: " + diceValueOne);
        //Debug.Log("Dice 2: " + diceValueTwo);
        //Debug.Log("Dice 3: " + diceValueThree);
        onDiceRolled?.Invoke();
    }

    public int GetDiceValueOne()
    {
        return diceValueOne;
    }

    public int GetDiceValueTwo()
    {
        return diceValueTwo;
    }

    public int GetDiceValueThree()
    {
        return diceValueThree;
    }

    public void SetModifierAssignment(int attackCountModifier, float tempoModifier, float breakDurationModifier)
    {
        assignedAttackCountModifier = attackCountModifier;
        assignedTempoModifier = tempoModifier;
        assignedBreakDurationModifier = breakDurationModifier;

        attackCountAssigned = true;
        tempoAssigned = true;
        breakDurationAssigned = true;
    }

    public bool AssignAttackCountModifier(int modifier)
    {
        if (attackCountAssigned) return false;

        assignedAttackCountModifier = modifier;
        attackCountAssigned = true;

        Debug.Log("Player assigned " + modifier + " to Attack Count.");
        return true;
    }

    public bool AssignTempoModifier(float modifier)
    {
        if (tempoAssigned) return false;

        assignedTempoModifier = modifier;
        tempoAssigned = true;

        Debug.Log("Player assigned " + modifier + " to Tempo.");
        return true;
    }

    public bool AssignBreakDurationModifier(float modifier)
    {
        if (breakDurationAssigned) return false;

        assignedBreakDurationModifier = modifier;
        breakDurationAssigned = true;

        Debug.Log("Player assigned " + modifier + " to Break Duration.");
        return true;
    }

    public void AssignDiceToAttackCount()
    {
        if (attackCountAssigned)
        {
            return;
        }

        AssignAttackCountModifier(diceValueOne);
    }

    public void AssignDiceToTempo()
    {
        if (tempoAssigned)
        {
            return;
        }

        AssignTempoModifier(diceValueTwo);
    }

    public void AssignDiceToBreakDuration()
    {
        if (breakDurationAssigned)
        {
            return;
        }

        AssignBreakDurationModifier(diceValueThree);
    }

    public int GetAssignedAttackCountModifier()
    {
        return assignedAttackCountModifier;
    }

    public float GetAssignedTempoModifier()
    {
        return assignedTempoModifier;
    }

    public float GetAssignedBreakDurationModifier()
    {
        return assignedBreakDurationModifier;
    }

    public bool IsAttackCountAssigned()
    {
        return attackCountAssigned;
    }

    public bool IsTempoAssigned()
    {
        return tempoAssigned;
    }

    public bool IsBreakDurationAssigned()
    {
        return breakDurationAssigned;
    }

    public bool HasAssignedAllModifiers()
    {
        return attackCountAssigned && tempoAssigned && breakDurationAssigned;
    }

    public void ResetModifierAssignment()
    {
        assignedAttackCountModifier = 0;
        assignedTempoModifier = 0f;
        assignedBreakDurationModifier = 0f;

        attackCountAssigned = false;
        tempoAssigned = false;
        breakDurationAssigned = false;
    }

    public void SetPlayerActions(bool canAct)
    {
        playerActions = canAct;
    }

    public void PerformAction(InputAction.CallbackContext context)
    {
        if (context.performed && playerActions)
        {
            onActionPerformed?.Invoke();
        }
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0)
        {
            return;
        }

        health -= damage;
        health = Mathf.Max(health, 0);

        Debug.Log("Player Taking " + damage + " damage");
        Debug.Log("Player HP: " + health);

        if (health <= 0)
        {
            health = 0;
            Debug.Log("Player Died");
        }
    }

    public void HealDamage(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        health += amount;
    }

    public bool CanParry()
    {
        return currentParryCooldown <= 0f;
    }

    public void TriggerParryCooldown()
    {
        currentParryCooldown = parryCooldown;
    }

    public float GetRemainingParryCooldown()
    {
        return Mathf.Max(currentParryCooldown, 0f);
    }
}
