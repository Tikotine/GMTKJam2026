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

    [SerializeField] private string label;

    private void Awake()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<Player>();
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

        valueText.text = FormatValue(draggedDice.GetDiceValue());
    }

    public void ClearDisplay()
    {
        valueText.text = label;
    }

    private string FormatValue(int value)
    {
        return value.ToString();
    }
}
