using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

public class RandEventHandler : MonoBehaviour
{
    [SerializeField] private List<string> dialogLines = new List<string>();  // Holds the main dialog lines
    [SerializeField] private List<string> speakersPerLine = new List<string>(); // Holds the speaker names per line
    [SerializeField] private List<SpriteOptions> characterSprites = new List<SpriteOptions>(); // Holds sprite options for each character
    [SerializeField] private List<Image> characterPortraits = new List<Image>(); // Holds references to character portrait Images

    [SerializeField] private TextMeshProUGUI textOutput; // Where dialog will be output
    [SerializeField] private TextMeshProUGUI speakerNameText; // Where the name of the current speaker will be output
    [SerializeField] private GameObject choicePanel; // Panel that holds choice buttons
    [SerializeField] private List<Button> choiceButtons = new List<Button>(); // Buttons to display the choices
    [SerializeField] private List<TextMeshProUGUI> choiceButtonTexts = new List<TextMeshProUGUI>(); // Text fields for the buttons
    [SerializeField] private GameObject mapButton;  // Map button to go back to the map

    [SerializeField] private List<Choices> choices = new List<Choices>(); // List holding all choice objects

    private int currentLineIndex = 0; // Current index for dialog
    private bool isTypewriting = false; // Whether the typewriter coroutine is going
    private bool skipRequested = false; // If the skip button is pressed
    private bool isChoiceTime = false; // If there is a choice to be made
    private bool isChoiceDialog = false; // If we are reading choice dialog

    // Backup the original dialog and speakers to restore after choices
    private List<string> originalDialogLines;
    private List<string> originalSpeakersPerLine;
    private List<SpriteOptions> originalSpriteOptions;
    private int originalLineIndex;  // Store the current index before the choice

    // Typewriter variables
    [SerializeField] private float typewriterSpeed = 0.05f;

    // FMOD Audio
    private EventInstance levelMusic;
    private EventInstance dateDialogueVoice;
    private EventInstance playerDialogueVoice;

    void Start()
    {
        choicePanel.SetActive(false); // Hide choice panel at start
        mapButton.SetActive(false);   // Hide the map buttonW at start
        InitializeAudio(); // Starts FMOD audio
        DisplayLine(); // Display the first line
    }

    void Update()
    {
        // Check for skip input
        if (Input.GetButtonDown("Skip"))
        {
            if (isTypewriting)
            {
                // Skip typewriter effect
                skipRequested = true;
            }
            else if (!isChoiceTime)
            {
                // Move to the next line
                NextLine();
            }
        }
    }

    private void DisplayLine()
    {
        // Check if we reached the end of the current dialog (main or choice)
        if (currentLineIndex >= dialogLines.Count)
        {
            if (isChoiceDialog)
            {
                // If we're at the end of the choice dialog, go back to the main dialog
                EndChoicePath();
                return;
            }
            else
            {
                // End of the main dialog
                mapButton.SetActive(true);  // Show the map button when dialog is done
                return;
            }
        }

        // Checks if we find a CHOICE
        if (currentLineIndex < dialogLines.Count)
        {
            if (dialogLines[currentLineIndex] == "CHOICE")
            {
                ShowChoices();
                return;
            }
        }

        // Set the speaker name based on the current line's speaker
        string currentSpeaker = speakersPerLine[currentLineIndex];
        speakerNameText.text = currentSpeaker;

        // Update all portraits based on the current line
        UpdateAllPortraits(currentSpeaker);

        // Start the typewriter effect
        StartCoroutine(TypewriteText(dialogLines[currentLineIndex], currentSpeaker));
    }

    // Coroutine to smoothly transition the character color
    private IEnumerator FadeCharacter(Image portrait, Color startColor, Color endColor, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            portrait.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final color is set exactly
        portrait.color = endColor;
    }

    // This method updates all portraits on each dialog line.
    private void UpdateAllPortraits(string currentSpeaker)
    {
        for (int i = 0; i < characterPortraits.Count; i++)
        {
            if (i < characterSprites.Count && characterSprites[i].spriteOptions.Count > 0)
            {
                // Update sprite for all characters based on the current line index
                characterPortraits[i].sprite = characterSprites[i].spriteOptions[currentLineIndex];

                // Determine if the character is speaking or not
                if (characterPortraits[i].name == currentSpeaker)
                {
                    // Undim the speaker with a smooth transition to full opacity (Color.white)
                    StartCoroutine(FadeCharacter(characterPortraits[i], characterPortraits[i].color, Color.white, 0.5f));
                }
                else
                {
                    // Dim non-speaking characters with a smooth transition to gray
                    StartCoroutine(FadeCharacter(characterPortraits[i], characterPortraits[i].color, Color.gray, 0.5f));
                }
            }
        }
    }

    private IEnumerator TypewriteText(string line, string speaker)
    {
        textOutput.text = "";
        isTypewriting = true;
        skipRequested = false;

        UpdateVoice(speaker);

        foreach (char letter in line.ToCharArray())
        {
            if (skipRequested)
            {
                // If skip is requested, display the full line immediately
                textOutput.text = line;
                break;
            }
            textOutput.text += letter;
            yield return new WaitForSeconds(typewriterSpeed);
        }

        isTypewriting = false;

        UpdateVoice(speaker);
    }

    public void NextLine()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.uiClick, this.transform.position); // Play click sound
        currentLineIndex++;
        DisplayLine();
    }

    private void ShowChoices()
    {
        isChoiceTime = true;
        isChoiceDialog = true;
        choicePanel.SetActive(true);

        // Fetch the current set of choices
        Choices currentChoices = choices[currentLineIndex];

        for (int i = 0; i < currentChoices.choiceOptions.Count; i++)
        {
            // Show button and set text
            choiceButtons[i].gameObject.SetActive(true);
            choiceButtonTexts[i].text = currentChoices.choiceOptions[i];
        }

        // Hide any unused buttons
        for (int i = currentChoices.choiceOptions.Count; i < choiceButtons.Count; i++)
        {
            choiceButtons[i].gameObject.SetActive(false);
        }
    }

    public void OnChoiceSelected(int choiceIndex)
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.uiClick, this.transform.position); // Play click sound
        isChoiceTime = false;
        choicePanel.SetActive(false);

        // Store the original dialog and index before branching to the choice dialog
        originalDialogLines = new List<string>(dialogLines);
        originalSpeakersPerLine = new List<string>(speakersPerLine);
        originalSpriteOptions = new List<SpriteOptions>(characterSprites);
        originalLineIndex = currentLineIndex + 1;  // Index to resume after choice

        // Retrieve the choice paths
        Choices currentChoices = choices[currentLineIndex];
        dialogLines = currentChoices.choicePaths[choiceIndex].afterChoiceDialogLines;
        speakersPerLine = currentChoices.choicePaths[choiceIndex].afterChoiceSpeakersPerLine;
        characterSprites = currentChoices.choicePaths[choiceIndex].afterChoiceSpriteOptions;

        // Start the choice dialog from the beginning
        currentLineIndex = 0;
        DisplayLine();
    }

    private void EndChoicePath()
    {
        // After a choice is completed, restore the original dialog flow
        dialogLines = originalDialogLines;
        speakersPerLine = originalSpeakersPerLine;
        characterSprites = originalSpriteOptions;
        currentLineIndex = originalLineIndex;  // Resume from the saved line index
        isChoiceDialog = false;  // We're no longer in a choice
        DisplayLine();  // Continue the main dialog
    }

    // FMOD Sound Functions
    private void UpdateVoice(string speaker)
    {
        var voiceInstance = (speaker == "You" || string.IsNullOrEmpty(speaker)) ? playerDialogueVoice : dateDialogueVoice;

        if (isTypewriting)
        {
            voiceInstance.start();
        }
        else
        {
            voiceInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    private void InitializeAudio()
    {
        levelMusic = AudioManager.instance.CreateInstance(FMODEvents.instance.music);
        playerDialogueVoice = AudioManager.instance.CreateInstance(FMODEvents.instance.playerVoice);
        dateDialogueVoice = AudioManager.instance.CreateInstance(FMODEvents.instance.dateVoice);
        PlayBackgroundMusic();
    }

    private void PlayBackgroundMusic()
    {
        levelMusic.getPlaybackState(out PLAYBACK_STATE playbackState);
        if (playbackState == PLAYBACK_STATE.STOPPED)
        {
            levelMusic.start();
        }
    }

    // Button Event: Return to Map
    public void OnMap()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.uiClick, this.transform.position); // Play click sound
        levelMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);  // Stop the background music
        SceneManager.LoadScene("Map"); // Load the "Map" scene
    }
}

[System.Serializable]
public class SpriteOptions
{
    public List<Sprite> spriteOptions = new List<Sprite>();
}