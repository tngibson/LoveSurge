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
    [SerializeField] private UnityEvent onDialogStart;
    [SerializeField] private UnityEvent onDialogComplete;
    private bool triggered = false;

    public void PlayDialog()
    {
        StartCoroutine(PlayDialogCoroutine());
    }

    private IEnumerator PlayDialogCoroutine()
    {
        onDialogStart?.Invoke();
        
        if (!triggered)
        {
            yield return scriptedTutorial.StartCoroutine(scriptedTutorial.PlayDialog(Dialog));
        }
        if (oneTime)
        {
            triggered = true;
        }

        onDialogComplete?.Invoke();
    }
}
