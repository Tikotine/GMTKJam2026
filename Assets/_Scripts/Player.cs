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
    public int health;
    [SerializeField] private float parryCooldown;
    private float currentParryCooldown;

    [Header("State & Actions")]
    public bool playerActions;
    public event Action onActionPerformed;

    [Header("Controllers/Managers")]
    private CharacterController controller;
    private QTEManager qteManager;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        qteManager = FindAnyObjectByType<QTEManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerRollDice();
        Debug.Log(diceValueOne);
        Debug.Log(diceValueTwo);
        Debug.Log(diceValueThree);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentParryCooldown > 0)
        {
            currentParryCooldown -= Time.deltaTime;
        }
    }

    //Roll the player's existing dice
    public void PlayerRollDice()
    {
        diceValueOne = diceSlotOne.Roll();
        diceValueTwo = diceSlotTwo.Roll();
        diceValueThree = diceSlotThree.Roll();
    }

    public void PerformAction(InputAction.CallbackContext context)
    {
        if (context.performed && playerActions)
        {
            onActionPerformed?.Invoke();
        }
    }


    //Health
    public void TakeDamage(int damage)
    { 
        health -= damage;

        if (health <= 0)
        { 
            health = 0;
            Debug.Log("Die");
        }
    }

    public void HealDamage(int amount)
    { 
        health += amount;
    }


    //Helper Functions
    public bool CanParry()
    {
        return currentParryCooldown <= 0;
    }

    public void TriggerParryCooldown()
    { 
        currentParryCooldown = parryCooldown;
    }

    public float GetRemainingParryCooldown()
    {
        return MathF.Max(currentParryCooldown, 0f);
    }
}
