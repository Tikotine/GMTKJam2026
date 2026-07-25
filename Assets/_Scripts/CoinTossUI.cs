using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinTossUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;

    [Header("UI")]
    [SerializeField] private Button headsButton;
    [SerializeField] private Button tailsButton;
    [SerializeField] private GameObject coinSelectionPanel;
    [SerializeField] private GameObject coinResultPanel;
    [SerializeField] private TMP_Text coinResultText;

    private void Awake()
    {
        if (gameManager == null) gameManager = FindAnyObjectByType<GameManager>();
        BindButtons();
    }

    private void OnDestroy()
    {
        UnbindButtons();
    }

    private void BindButtons()
    {
        if (headsButton != null) headsButton.onClick.AddListener(SelectHeads);
        if (tailsButton != null) tailsButton.onClick.AddListener(SelectTails);
    }

    private void UnbindButtons()
    {
        if (headsButton != null) headsButton.onClick.RemoveListener(SelectHeads);
        if (tailsButton != null) tailsButton.onClick.RemoveListener(SelectTails);
    }

    public void SelectHeads()
    {
        SelectCoinSide(GameManager.CoinSide.HEADS);
    }

    public void SelectTails()
    {
        SelectCoinSide(GameManager.CoinSide.TAILS);
    }

    private void SelectCoinSide(GameManager.CoinSide choice)
    {
        if (gameManager == null)
        {
            Debug.LogError("CoinTossUI could not find GameManager.");
            return;
        }

        gameManager.SetPlayerCoinChoice(choice);

        if (coinSelectionPanel != null)
        {
            coinSelectionPanel.SetActive(false);
        }

        gameManager.StartGame();
    }

    public void DisplayCoinResult(GameManager.CoinSide result)
    {
        if (coinResultPanel != null)
        {
            coinResultPanel.SetActive(true);
        }

        if (coinResultText != null)
        {
            coinResultText.text = "Coin Result: " + result;
        }
    }
}
