using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayTutorialDialog : MonoBehaviour
{
    [Header("Dialog")]
    [SerializeField] private List<DialogueLines> Dialog;
    [SerializeField] private bool oneTime = true;
    [SerializeField] private ScriptedTutorial scriptedTutorial;
    private bool triggered = false;

    public void PlayDialog()
    {
        if (!triggered)
        {
            scriptedTutorial.StartCoroutine(scriptedTutorial.PlayDialog(Dialog));
        }
        if (oneTime)
        {
            triggered = true;
        }
    }
}
