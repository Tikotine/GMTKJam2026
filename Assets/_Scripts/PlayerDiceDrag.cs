using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerDiceDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum DiceType { ATTACK_COUNT, TEMPO, BREAK_DURATION }

    [Header("Dice")]
    [SerializeField] private int diceValue;

    [Header("Drag Settings")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform dragTransform;

    [Header("References")]
    [SerializeField] private Player player;

    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Transform originalParent;
    private bool isAssigned;

    private void Awake()
    {
        if (player == null) player = FindAnyObjectByType<Player>();
        if (canvas == null) canvas = GetComponentInParent<Canvas>();
        if (dragTransform == null) dragTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        isAssigned = false;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void SetDiceValue(int value)
    {
        diceValue = value;
        isAssigned = false;
        GetComponentInChildren<Text>().text = value.ToString();
    }

    public int GetDiceValue() => diceValue;
    public bool IsAssigned() => isAssigned;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isAssigned) return;
        originalPosition = dragTransform.anchoredPosition;
        originalParent = dragTransform.parent;
        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;
        dragTransform.SetParent(canvas.transform, true);
        dragTransform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isAssigned) return;
        RectTransform canvasRect = canvas.transform as RectTransform;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, canvas.worldCamera, out Vector2 localPoint)) dragTransform.localPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isAssigned) return;
        ResetDragVisual();
    }

    public bool AssignDice(DiceType target)
    {
        if (isAssigned) return false;
        bool wasAssigned;
        switch (target)
        {
            case DiceType.ATTACK_COUNT: wasAssigned = player.AssignAttackCountModifier(diceValue); break;
            case DiceType.TEMPO: wasAssigned = player.AssignTempoModifier(diceValue); break;
            case DiceType.BREAK_DURATION: wasAssigned = player.AssignBreakDurationModifier(diceValue); break;
            default: return false;
        }
        if (!wasAssigned) return false;
        isAssigned = true;
        ResetDragVisual();
        gameObject.SetActive(false);
        return true;
    }

    private void ResetDragVisual()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        dragTransform.SetParent(originalParent, true);
        dragTransform.anchoredPosition = originalPosition;
    }
}
