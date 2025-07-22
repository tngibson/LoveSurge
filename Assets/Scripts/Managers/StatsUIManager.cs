using TMPro;
using UnityEngine;

public class StatsUIManager : MonoBehaviour
{
    [Header("Stat Text Fields")]
    public TextMeshProUGUI charismaText;
    public TextMeshProUGUI clevernessText;
    public TextMeshProUGUI courageText;
    public TextMeshProUGUI creativityText;
    public TextMeshProUGUI cashText;

    private Player player;

    void Start()
    {
        // Assuming Player.instance is properly set in Awake
        player = Player.instance;

        if (player == null)
        {
            Debug.LogError("Player instance not found!");
            enabled = false;
            return;
        }

        player.OnStatsChanged += UpdateUI;
        UpdateUI();
    }

    void UpdateUI()
    {
        charismaText.text = $"{player.GetStat(Player.StatType.Charisma)} - Charisma";
        clevernessText.text = $"{player.GetStat(Player.StatType.Cleverness)} - Cleverness";
        courageText.text = $"{player.GetStat(Player.StatType.Courage)} - Courage";
        creativityText.text = $"{player.GetStat(Player.StatType.Creativity)} - Creativity";
        cashText.text = $"${player.cash} - Cash";
    }
}
