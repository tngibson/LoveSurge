using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToDDisplay : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI charismaText;
    public TextMeshProUGUI courageText;
    public TextMeshProUGUI clevernessText;
    public TextMeshProUGUI creativityText;

    // Update is called once per frame
    void Update()
    {
        timeText.text = "Time Of Day: " + GlobalInformation.instance.timeOfDay.ToString();
        charismaText.text = "Charisma: " + GlobalInformation.instance.statlist[(int)Stats.Charisma];
        courageText.text = "Courage: " + GlobalInformation.instance.statlist[(int)Stats.Courage];
        clevernessText.text = "Cleverness: " + GlobalInformation.instance.statlist[(int)Stats.Cleverness];
        creativityText.text = "Creativity: " + GlobalInformation.instance.statlist[(int)Stats.Creativity];
    }
}
