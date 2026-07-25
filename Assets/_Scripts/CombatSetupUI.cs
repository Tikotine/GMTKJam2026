using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class CombatSetupUI : MonoBehaviour
{
    private const string UiRootName = "Combat Setup UI";

    [SerializeField] private Player player;
    [SerializeField] private GameManager gameManager;

    [SerializeField] private GameObject dicePanel;
    [SerializeField] private TMP_Text phaseText;
    private readonly List<ModifierDropZone> dropZones = new List<ModifierDropZone>();

    [SerializeField] private PlayerDiceDrag diceOne;
    [SerializeField] private PlayerDiceDrag diceTwo;
    [SerializeField] private PlayerDiceDrag diceThree;

    [SerializeField] private ModifierDropZone attackZone;
    [SerializeField] private ModifierDropZone tempoZone;
    [SerializeField] private ModifierDropZone breakZone;

    [SerializeField] private PlayerDiceUI diceUI;

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }


        if (player == null)
        { 
            player = FindAnyObjectByType<Player>();   
        }

        dropZones.Clear();
        dropZones.Add(attackZone);
        dropZones.Add(tempoZone);
        dropZones.Add(breakZone);

        player.onDiceRolled += ShowDicePanel;

        dicePanel.SetActive(false);
    }

    private void Update()
    {
        if (dicePanel.activeSelf && player.HasAssignedAllModifiers()) dicePanel.SetActive(false);
    }

    private void ShowDicePanel()
    {
        dicePanel.SetActive(true);
        phaseText.text = gameManager.IsPlayerAttacking() ? "YOU ARE ATTACKING" : "YOU ARE DEFENDING";
        foreach (ModifierDropZone zone in dropZones) zone.ClearDisplay();
        diceUI.RefreshDiceUI();
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.onDiceRolled -= ShowDicePanel;
        }
    }
}
