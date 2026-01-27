using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayTutorialDialog : MonoBehaviour
{
    [Header("Dialog")]
    [SerializeField] private List<DialogueLines> Dialog;
    [SerializeField] private ScriptedTutorial scriptedTutorial;

    public void PlayDialog()
    {
        scriptedTutorial.StartCoroutine(scriptedTutorial.PlayDialog(Dialog));
    }
}
