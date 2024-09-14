using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NETManager : MonoBehaviour
{
    // Reference to the UI element for displaying dialogue
    [SerializeField] private TextMeshProUGUI dialogueText;

    // Hides the NETManager and progresses the time of day
    public void Leave()
    {
        gameObject.SetActive(false);
        GlobalInformation.instance.ProgressTimeOfDay();
        UpdateDialogue("Lorem Ipsum! Ja?");
    }

    // Dialog Option 1
    public void Op1()
    {
        dialogueText.text = "Ja!";
    }

    // Dialog Option 2
    public void Op2()
    {
        dialogueText.text = "Ja?";

    }

    // Dialog Option 3
    public void Op3()
    {
        dialogueText.text = "Ja...";
    }
}
