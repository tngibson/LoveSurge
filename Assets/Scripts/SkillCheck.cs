using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;

public class SkillCheck : MonoBehaviour
{
    [SerializeField] private List<string> dialogLines = new List<string>();  // Main dialog lines
    [SerializeField] private List<string> speakersPerLine = new List<string>();  // Speakers per line
    [SerializeField] private List<SpriteOptions> characterSprites = new List<SpriteOptions>();  // Sprites for characters
    [SerializeField] private List<Image> characterPortraits = new List<Image>();  // Character portrait images

    [SerializeField] private TextMeshProUGUI textOutput;  // Dialog output field
    [SerializeField] private TextMeshProUGUI speakerNameText;  // Speaker name output
    [SerializeField] private GameObject mapButton;  // Button to return to map

    [SerializeField] private SkillCheckPaths skillCheckPaths;  // ScriptableObject with paths for success/failure
    [SerializeField] private int skillCheckThreshold = 10;  // Threshold to pass skill check
    [SerializeField] private int currentSkillIndex;  // Index for skill (0-3 for Charisma, Cleverness, Creativity, Courage)

    [SerializeField] private float typewriterSpeed = 0.05f;  // Speed of typewriter effect

    private int currentLineIndex = 0;  // Current index for dialog
    private bool isSkillCheckTime = false;  // Flag for skill check time
    private bool isSkillCheckDialog = false;  // Flag for skill check dialog path
    private bool skillCheckPassed;  // Track if skill check was passed
    private bool isRollingDisplayActive = false;  // Flag for rolling animation
    private bool isTypewriting = false;  // Flag for typewriter effect
    private bool skipRequested = false; // If the skip button is pressed

    private int modifier;
    private int skillShake;
    private int diceRoll1, diceRoll2;  // Final values for dice rolls

    private int skillCheckStage = 0;  // Track which stage of skill check we are in

    private List<string> originalDialogLines;
    private List<string> originalSpeakersPerLine;
    private List<SpriteOptions> originalSpriteOptions;
    private int originalLineIndex;  // Index to return to after skill check dialog path

    private EventInstance levelMusic;
    private EventInstance diceShake;
    private EventInstance dateDialogueVoice;
    private EventInstance date2DialougeVoice;
    private EventInstance playerDialogueVoice; 

    private Player playerManager;
    private string playerName;

    void Start()
    {
        if (GameObject.Find("PlayerManager") != null)
        {
            playerManager = GameObject.Find("PlayerManager").GetComponent<Player>();
            playerName = playerManager.GetName();
        }

        mapButton.SetActive(false);
        InitializeAudio();
        DisplayLine();
    }

    void Update()
    {
        if (Input.GetButtonDown("Skip"))
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        if (isSkillCheckTime)
        {
            HandleSkillCheckClick();
        }
        else if (isTypewriting)
        {
            skipRequested = true;  // Complete the current line immediately
        }
        else
        {
            NextLine();
        }
    }

    private void DisplayLine()
    {
        if (currentLineIndex >= dialogLines.Count)
        {
            if (isSkillCheckDialog)
            {
                EndSkillCheckPath();
                return;
            }
            else
            {
                mapButton.SetActive(true);
                return;
            }
        }

        if (dialogLines[currentLineIndex] == "SKILLCHECK")
        {
            StartSkillCheck();
            return;
        }

        string currentSpeaker = speakersPerLine[currentLineIndex];
        if (currentSpeaker == "You" && !string.IsNullOrEmpty(playerName))
        {
            currentSpeaker = playerName;
        }

        speakerNameText.text = currentSpeaker;
        dialogLines[currentLineIndex] = dialogLines[currentLineIndex].Replace("[Player]", playerName);
        UpdateAllPortraits(currentSpeaker);
        StartCoroutine(TypewriteText(dialogLines[currentLineIndex], currentSpeaker));

    }

    private void StartSkillCheck()
    {
        isSkillCheckTime = true;
        skillCheckStage = 0;
        // Rolls which card is drawn, THIS WILL BE CHANGED LATER WHEN MORE CARDS ARE ADDED
        modifier = Random.Range(1, 5);
        skillShake = Random.Range(0, 4);
        switch (skillShake)
        {
            case 0:
                textOutput.text = $"A Charisma card with a power of {modifier} was drawn!";
                break;
            case 1:
                textOutput.text = $"A Cleverness card with a power of {modifier} was drawn!";
                break;
            case 2:
                textOutput.text = $"A Creativity card with a power of {modifier} was drawn!";
                break;
            case 3:
                textOutput.text = $"A Courage card with a power of {modifier} was drawn!";
                break;
        }
    }

    private IEnumerator ShowRollingEffect()
    {
        isRollingDisplayActive = true;
        while (isRollingDisplayActive)
        {
            textOutput.text = $"{Random.Range(1, 7)} + {Random.Range(1, 7)}";
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void HandleSkillCheckClick()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.uiClick, this.transform.position); // Play click sound

        switch (skillCheckStage)
        {
            case 0:
                StartCoroutine(ShowRollingEffect());
                skillCheckStage = 1;
                diceShake.start();

                break;
            case 1:
                isRollingDisplayActive = false;
                diceRoll1 = Random.Range(1, 7);
                diceRoll2 = Random.Range(1, 7);
                int rollTotal = diceRoll1 + diceRoll2 + modifier;
                if (skillShake == currentSkillIndex)
                {
                    rollTotal += 1;
                }
                //
                skillCheckPassed = rollTotal >= skillCheckThreshold;
                if (skillShake == currentSkillIndex)
                {
                    textOutput.text = $"{diceRoll1} + {diceRoll2} + ({modifier}) + (1 | same type bonus!) = {rollTotal}";
                }
                else
                {
                    textOutput.text = $"{diceRoll1} + {diceRoll2} + ({modifier}) = {rollTotal}";
                }
                skillCheckStage = 2;
                diceShake.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                AudioManager.instance.PlayOneShot(FMODEvents.instance.diceRoll, this.transform.position);

                break;

            case 2:
                textOutput.text = skillCheckPassed ? "Skill Check Passed!" : "Skill Check Failed...";
                skillCheckStage = 3;
                break;

            case 3:
                LoadSkillCheckPath(skillCheckPassed);
                DisplayLine();
                isSkillCheckTime = false;
                break;
        }
    }

    private void LoadSkillCheckPath(bool success)
    {

        isSkillCheckDialog = true;
        originalDialogLines = new List<string>(dialogLines);
        originalSpeakersPerLine = new List<string>(speakersPerLine);
        originalSpriteOptions = new List<SpriteOptions>(characterSprites);
        originalLineIndex = currentLineIndex + 1;

        if (success)
        {
            dialogLines = new List<string>(skillCheckPaths.successDialogLines);
            speakersPerLine = new List<string>(skillCheckPaths.successSpeakersPerLine);
            characterSprites = new List<SpriteOptions>(skillCheckPaths.successSpriteOptions);
            AudioManager.instance.PlayOneShot(FMODEvents.instance.goodResponse, this.transform.position); // Play good sound
        }
        else
        {
            dialogLines = new List<string>(skillCheckPaths.failureDialogLines);
            speakersPerLine = new List<string>(skillCheckPaths.failureSpeakersPerLine);
            characterSprites = new List<SpriteOptions>(skillCheckPaths.failureSpriteOptions);
            AudioManager.instance.PlayOneShot(FMODEvents.instance.badResponse, this.transform.position); // Play sound sound
        }

        currentLineIndex = 0;  // Reset to start of skill check dialog
    }

    private void EndSkillCheckPath()
    {
        dialogLines = originalDialogLines;
        speakersPerLine = originalSpeakersPerLine;
        characterSprites = originalSpriteOptions;
        currentLineIndex = originalLineIndex;  // Resume from where we left off
        isSkillCheckDialog = false;
        DisplayLine();
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
                textOutput.text = line;
                skipRequested = false;  // Reset skip after displaying the full line
                break;
            }
            textOutput.text += letter;
            yield return new WaitForSeconds(typewriterSpeed);
        }

        isTypewriting = false;  // Set to false once typewriting is completed
        UpdateVoice(speaker);

    }

    public void NextLine()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.uiClick, this.transform.position); // Play click sound
        currentLineIndex++;
        DisplayLine();
    }

    private void UpdateAllPortraits(string currentSpeaker)
    {
        for (int i = 0; i < characterPortraits.Count; i++)
        {
            if (i < characterSprites.Count && characterSprites[i].spriteOptions.Count > 0)
            {
                characterPortraits[i].sprite = characterSprites[i].spriteOptions[currentLineIndex];
                if (characterPortraits[i].name == currentSpeaker)
                {
                    StartCoroutine(FadeCharacter(characterPortraits[i], characterPortraits[i].color, Color.white, 0.5f));
                }
                else
                {
                    StartCoroutine(FadeCharacter(characterPortraits[i], characterPortraits[i].color, Color.gray, 0.5f));
                }
            }
        }
    }

    private IEnumerator FadeCharacter(Image portrait, Color startColor, Color endColor, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            portrait.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        portrait.color = endColor;
    }

    private void UpdateVoice(string speaker)
    {
        var voiceInstance = (speaker == "You" || string.IsNullOrEmpty(speaker)) ? playerDialogueVoice : date2DialougeVoice;
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
        //levelMusic = AudioManager.instance.CreateInstance(FMODEvents.instance.music);
        playerDialogueVoice = AudioManager.instance.CreateInstance(FMODEvents.instance.playerVoice);
        dateDialogueVoice = AudioManager.instance.CreateInstance(FMODEvents.instance.dateVoice);
        date2DialougeVoice = AudioManager.instance.CreateInstance(FMODEvents.instance.dateVoice2);  
        diceShake = AudioManager.instance.CreateInstance(FMODEvents.instance.diceShake);

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
}