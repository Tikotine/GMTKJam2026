using UnityEngine;

[CreateAssetMenu(fileName = "New Dice", menuName = "Dice")]
public class Dice : ScriptableObject
{
    //Dice Stats
    public string diceName;
    public int minValue;
    public int maxValue;

    //Return a roll value
    public int Roll()
    {
        return Random.Range(minValue, maxValue + 1);
    }
}
