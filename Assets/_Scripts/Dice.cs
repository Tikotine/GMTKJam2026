using UnityEngine;

[CreateAssetMenu(fileName = "Dice", menuName = "Combat/Dice")]
public class Dice : ScriptableObject
{
    [Header("Dice Settings")]
    [SerializeField] private int minimumValue = 1;
    [SerializeField] private int maximumValue = 6;

    public int Roll()
    {
        return Random.Range(minimumValue, maximumValue + 1);
    }

    public int GetMinimumValue()
    {
        return minimumValue;
    }

    public int GetMaximumValue()
    {
        return maximumValue;
    }
}