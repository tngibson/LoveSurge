using UnityEngine;
using TMPro;

public class ToDDisplay : MonoBehaviour
{
    // References to UI text components
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI charismaText;
    public TextMeshProUGUI courageText;
    public TextMeshProUGUI clevernessText;
    public TextMeshProUGUI creativityText;

    // Cache reference to the global stats to avoid repeated access
    private GlobalInformation globalInfo;

    // Called when the script instance is being loaded
    private void Start()
    {
        // Cache the reference to GlobalInformation instance for better performance
        globalInfo = GlobalInformation.instance;

        // Initial UI update to set values right from the start
        UpdateUI();
    }

    // Update the text elements based on the current state
    private void Update()
    {
        UpdateUI();
    }

    // Method to update the displayed text for time of day and stats
    private void UpdateUI()
    {
        timeText.text = "Time Of Day: " + globalInfo.timeOfDay.ToString();
        charismaText.text = "Charisma: " + globalInfo.statlist[(int)Stats.Charisma];
        courageText.text = "Courage: " + globalInfo.statlist[(int)Stats.Courage];
        clevernessText.text = "Cleverness: " + globalInfo.statlist[(int)Stats.Cleverness];
        creativityText.text = "Creativity: " + globalInfo.statlist[(int)Stats.Creativity];
    }
}
