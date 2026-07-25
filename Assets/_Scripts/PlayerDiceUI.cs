using UnityEngine;
using UnityEngine.UI;

public class PlayerDiceUI : MonoBehaviour
{
    [Header("Dice UI")]
    [SerializeField] private PlayerDiceDrag diceOne;
    [SerializeField] private PlayerDiceDrag diceTwo;
    [SerializeField] private PlayerDiceDrag diceThree;

    [Header("Dice Images")]
    [SerializeField] private Image diceOneImage;
    [SerializeField] private Image diceTwoImage;
    [SerializeField] private Image diceThreeImage;

    [Header("Dice Sprites")]
    [SerializeField] private Sprite[] diceSprites;

    [Header("References")]
    [SerializeField] private Player player;

    private void Awake()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<Player>();
        }

        if (player != null)
        {
            player.onDiceRolled += RefreshDiceUI;
        } 
    }

    private void OnDestroy()
    {
        if (player != null) player.onDiceRolled -= RefreshDiceUI;
    }

    public void RefreshDiceUI()
    {
        if (player == null)
        {
            return;
        }

        ShowAllDice();
        UpdateDice(diceOne, diceOneImage, player.GetDiceValueOne());
        UpdateDice(diceTwo, diceTwoImage, player.GetDiceValueTwo());
        UpdateDice(diceThree, diceThreeImage, player.GetDiceValueThree());
    }

    private void UpdateDice(PlayerDiceDrag dice, Image image, int value)
    {
        if (dice == null)
        {
            return;
        }

        dice.SetDiceValue(value);

        if (image == null || diceSprites == null)
        {
            return;
        }

        int spriteIndex = value - 1;

        if (spriteIndex >= 0 && spriteIndex < diceSprites.Length)
        {
            image.sprite = diceSprites[spriteIndex];
        }

        image.gameObject.SetActive(true);
    }

    public void HideDice(PlayerDiceDrag dice)
    {
        if (dice == null)
        {
            return;
        }

        dice.gameObject.SetActive(false);
    }

    public void ShowAllDice()
    {
        if (diceOne != null)
        {
            diceOne.gameObject.SetActive(true);
        }

        if (diceTwo != null)
        {
            diceTwo.gameObject.SetActive(true);
        }

        if (diceThree != null)
        {
            diceThree.gameObject.SetActive(true);
        }
    }
}
