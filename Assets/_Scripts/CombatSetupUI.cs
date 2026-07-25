using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class CombatSetupUI : MonoBehaviour
{
    private const string UiRootName = "Combat Setup UI";

    private Player player;
    private GameManager gameManager;
    private GameObject dicePanel;
    private PlayerDiceUI diceUI;
    private Font font;
    private Text phaseText;
    private readonly List<ModifierDropZone> dropZones = new List<ModifierDropZone>();

    private void Awake()
    {
        if (GameObject.Find(UiRootName) != null) return;

        gameManager = GetComponent<GameManager>();
        player = FindAnyObjectByType<Player>();
        if (gameManager == null || player == null) throw new MissingReferenceException("CombatSetupUI needs a GameManager and Player in the scene.");

        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        CreateEventSystem();
        Canvas canvas = CreateCanvas();
        CreateCoinTossUI(canvas, gameManager);
        CreateDiceAssignmentUI(canvas);
    }

    private void Update()
    {
        if (dicePanel.activeSelf && player.HasAssignedAllModifiers()) dicePanel.SetActive(false);
    }

    private void CreateEventSystem()
    {
        if (FindAnyObjectByType<EventSystem>() != null) return;

        GameObject eventSystemObject = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        eventSystemObject.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
    }

    private Canvas CreateCanvas()
    {
        GameObject canvasObject = new GameObject(UiRootName, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        return canvas;
    }

    private void CreateCoinTossUI(Canvas canvas, GameManager gameManager)
    {
        GameObject selectionPanel = CreatePanel("Coin Selection", canvas.transform, new Color(0.07f, 0.1f, 0.16f, 0.94f));
        SetRect(selectionPanel.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(620f, 360f));
        CreateText("Title", selectionPanel.transform, "CALL THE COIN", 42, Vector2.zero, new Vector2(560f, 80f));
        Button heads = CreateButton("Heads", selectionPanel.transform, "HEADS", new Vector2(-155f, -80f));
        Button tails = CreateButton("Tails", selectionPanel.transform, "TAILS", new Vector2(155f, -80f));

        GameObject resultPanel = CreatePanel("Coin Result", canvas.transform, new Color(0f, 0f, 0f, 0.6f));
        SetRect(resultPanel.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -80f), new Vector2(560f, 90f));
        Text resultText = CreateText("Result", resultPanel.transform, string.Empty, 28, Vector2.zero, new Vector2(520f, 70f));
        resultPanel.SetActive(false);

        CoinTossUI coinTossUI = selectionPanel.AddComponent<CoinTossUI>();
        coinTossUI.Configure(gameManager, heads, tails, selectionPanel, resultPanel, resultText);
        gameManager.RegisterCoinTossUI(coinTossUI);
    }

    private void CreateDiceAssignmentUI(Canvas canvas)
    {
        dicePanel = CreatePanel("Dice Assignment", canvas.transform, new Color(0.06f, 0.08f, 0.12f, 0.94f));
        SetRect(dicePanel.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 230f), new Vector2(1100f, 360f));
        CreateText("Instructions", dicePanel.transform, "DRAG EACH DIE TO A MODIFIER", 28, new Vector2(0f, 140f), new Vector2(900f, 55f));
        phaseText = CreateText("Phase", dicePanel.transform, string.Empty, 22, new Vector2(0f, 105f), new Vector2(900f, 40f));

        PlayerDiceDrag diceOne = CreateDice("Dice One", dicePanel.transform, new Vector2(-260f, 50f));
        PlayerDiceDrag diceTwo = CreateDice("Dice Two", dicePanel.transform, new Vector2(0f, 50f));
        PlayerDiceDrag diceThree = CreateDice("Dice Three", dicePanel.transform, new Vector2(260f, 50f));
        CreateDropZone("Attack Count", dicePanel.transform, new Vector2(-260f, -105f), PlayerDiceDrag.DiceType.ATTACK_COUNT);
        CreateDropZone("Tempo", dicePanel.transform, new Vector2(0f, -105f), PlayerDiceDrag.DiceType.TEMPO);
        CreateDropZone("Break Duration", dicePanel.transform, new Vector2(260f, -105f), PlayerDiceDrag.DiceType.BREAK_DURATION);

        diceUI = dicePanel.AddComponent<PlayerDiceUI>();
        diceUI.Configure(player, diceOne, diceTwo, diceThree);
        player.onDiceRolled += ShowDicePanel;
        dicePanel.SetActive(false);
    }

    private void ShowDicePanel()
    {
        dicePanel.SetActive(true);
        phaseText.text = gameManager.IsPlayerAttacking() ? "YOU ARE ATTACKING" : "YOU ARE DEFENDING";
        foreach (ModifierDropZone zone in dropZones) zone.ClearDisplay();
        diceUI.RefreshDiceUI();
    }

    private PlayerDiceDrag CreateDice(string name, Transform parent, Vector2 position)
    {
        GameObject dice = CreatePanel(name, parent, new Color(0.9f, 0.75f, 0.22f, 1f));
        SetRect(dice.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), position, new Vector2(120f, 120f));
        CreateText("Value", dice.transform, "?", 60, Vector2.zero, new Vector2(100f, 100f));
        return dice.AddComponent<PlayerDiceDrag>();
    }

    private void CreateDropZone(string label, Transform parent, Vector2 position, PlayerDiceDrag.DiceType type)
    {
        GameObject zone = CreatePanel(label + " Zone", parent, new Color(0.18f, 0.32f, 0.45f, 1f));
        SetRect(zone.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), position, new Vector2(230f, 90f));
        Text valueText = CreateText("Label", zone.transform, label, 20, Vector2.zero, new Vector2(210f, 70f));
        ModifierDropZone dropZone = zone.AddComponent<ModifierDropZone>();
        dropZone.Configure(type, player, valueText, label);
        dropZones.Add(dropZone);
    }

    private GameObject CreatePanel(string name, Transform parent, Color color)
    {
        GameObject panel = new GameObject(name, typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(parent, false);
        panel.GetComponent<Image>().color = color;
        return panel;
    }

    private Button CreateButton(string name, Transform parent, string label, Vector2 position)
    {
        GameObject buttonObject = CreatePanel(name, parent, new Color(0.75f, 0.2f, 0.16f, 1f));
        SetRect(buttonObject.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), position, new Vector2(250f, 100f));
        Button button = buttonObject.AddComponent<Button>();
        CreateText("Label", buttonObject.transform, label, 28, Vector2.zero, new Vector2(220f, 70f));
        return button;
    }

    private Text CreateText(string name, Transform parent, string value, int fontSize, Vector2 position, Vector2 size)
    {
        GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
        textObject.transform.SetParent(parent, false);
        SetRect(textObject.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), position, size);
        Text text = textObject.GetComponent<Text>();
        text.font = font;
        text.text = value;
        text.fontSize = fontSize;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        return text;
    }

    private void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 size)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }
}
