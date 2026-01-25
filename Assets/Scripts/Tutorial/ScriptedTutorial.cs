using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using TMPro;

public class ScriptedTutorial : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textOutput;    // UI Text component for displaying dialogue
    [SerializeField] private GameObject continueIndicator;  // UI element indicating to continue
    [SerializeField] private GameObject mapButton;          // Reference to the map button UI element
    [SerializeField] private float typewriterSpeed = 0.025f;
    private Player playerManager;
    private string playerName;
    private EventInstance wizardVoice;
    private bool isTypewriting = false;
    private bool skipRequested = false;
    private int currentLineIndex = 0;

    void Start()
    {
        if (GameObject.Find("PlayerManager") != null)
        {
            playerManager = GameObject.Find("PlayerManager").GetComponent<Player>();
            playerName = playerManager.GetName();
        }
        else
        {
            Debug.LogWarning("Player Manager was null!");
        }

        mapButton.SetActive(false);   // Hide the map button at start
        InitializeAudio(); // Starts FMOD audio
    }

    private void InitializeAudio()
    {
        wizardVoice = AudioManager.instance.CreateInstance(FMODEvents.instance.WizardVoice);
    }

    private IEnumerator TypewriteText(string line, string speaker)
    {
        textOutput.text = "";
        continueIndicator.SetActive(false);
        isTypewriting = true;
        skipRequested = false;

        // UpdateVoice(speaker);

        // foreach (char letter in line.ToCharArray())
        // {
        //     if (skipRequested)
        //     {
        //         textOutput.text = line;
        //         break;
        //     }
        //     textOutput.text += letter;
        //     yield return new WaitForSeconds(typewriterSpeed);
        // }

        // isTypewriting = false;
        // continueIndicator.SetActive(true);

        // // Safely invoke animation stop
        // for (int i = 0; i <characterPortraits.Count; i++)
        // {
        //     if (CharacterAnimator.HasAnimator(characterPortraits[i]))
        //     {
        //         if (i < characterPortraits.Count && characterSprites[i].spriteOptions.Count > 0)
        //         {
        //             Sprite targetSprite = characterSprites[i].spriteOptions[currentLineIndex];
        //             if (targetSprite != null)
        //             {
        //                 characterPortraits[i].sprite = targetSprite;
        //                 lastTargetSpriteName = targetSprite.name;

        //                 CharacterAnimator.InvokeStopAnimation(this, new AnimatorEventData()
        //                 {
        //                     State = lastTargetSpriteName,
        //                     Speaker = speaker
        //                 });
        //             }
        //         }
        //     }
        // }

        // UpdateVoice(speaker);
        yield return null;
    }

    public void NextLine()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.UiClick, this.transform.position); // Play click sound
        currentLineIndex++;
        //DisplayLine();
    }
}

[System.Serializable]
public class DialogueLines
{
    [TextArea(3, 10)]
    public string dialogue;       // main dialogue lines
    public string speaker;        // speakers corresponding to each line

}

