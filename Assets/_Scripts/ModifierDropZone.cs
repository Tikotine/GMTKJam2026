using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ModifierDropZone : MonoBehaviour, IDropHandler
{
    [Header("Modifier Type")]
    [SerializeField] private PlayerDiceDrag.DiceType modifierType;

    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private TMP_Text valueText;

    [SerializeField] private TMP_Text label;
    private string defaultText;

    private void Awake()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<Player>();
        }

        if (label != null)
        {
            defaultText = label.text;
        }

        ClearDisplay();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        PlayerDiceDrag draggedDice = eventData.pointerDrag.GetComponent<PlayerDiceDrag>();
        if (draggedDice == null || draggedDice.IsAssigned()) return;

        if (!draggedDice.AssignDice(modifierType))
        {
            Debug.Log("" + label + " already has a die assigned.");
            return;
        }

        valueText.text = label + "\n<color=#FFD54A>" + FormatValue(draggedDice.GetDiceValue()) + "</color>";
    }

    public void ClearDisplay()
    {
        valueText.text = label + "\n<size=16>Drop die here</size>";
    }

    private string FormatValue(int value)
    {
        return value.ToString();
    }
}
