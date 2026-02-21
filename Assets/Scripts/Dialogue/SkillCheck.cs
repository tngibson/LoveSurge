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

    private float typewriterSpeed = 0.025f;  // Speed of typewriter effect

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

  private EventInstance diceShake;
   private EventInstance playerVoice;
    private EventInstance nokiVoice;
    private EventInstance lotteVoice;
    private EventInstance celciVoice;
    private EventInstance miguelVoice;
    private EventInstance fishVoice;
    private EventInstance ceoVoice;
    private EventInstance wizardVoice;
    private EventInstance deliahVoice;
    private EventInstance noVoice;

    private Coroutine voiceCoroutine; // Tracks the voice playback coroutine

    private Player playerManager;
    private string playerName;

    [SerializeField] private GameObject continueIndicator;

    [SerializeField] private GameObject skillCheckScreen;
    [SerializeField] private GameObject skillCheckArea;
    [SerializeField] private float startingScreenTime;
    [SerializeField] private bool startingScreenOver = false;
    [SerializeField] private Image dice1;
    [SerializeField] private Image dice2;
    [SerializeField] private List<Sprite> diceOptions = new List<Sprite>();
    [SerializeField] private List<Card> cardPrefabs = new List<Card>();
    [SerializeField] private GameObject cardParent;
    [SerializeField] private GameObject modifierText;
    [SerializeField] private TextMeshProUGUI totalText;
    [SerializeField] private TextMeshProUGUI skillCheckText;

    public string SaveID => "SkillCheck";

    void Start()
    {
        if (GameObject.Find("PlayerManager") != null)
        {
            playerManager = GameObject.Find("PlayerManager").GetComponent<Player>();
            playerName = playerManager.GetName();
        }

        mapButton.SetActive(false);
        InitializeAudio();

        StartCoroutine(HandleStartingScreen());
    }

    private IEnumerator HandleStartingScreen()
    {
        yield return new WaitForSeconds(startingScreenTime);
        skillCheckScreen.SetActive(false);
        startingScreenOver = true;
        DisplayLine();
    }

    void Update()
    {
        if (Input.GetButtonDown("Skip") && startingScreenOver)
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
                MusicManager.SetParameterByName("dateProgress", 2);
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
        dialogLines[currentLineIndex] = dialogLines[currentLineIndex].Replace("[Player Name]", playerName);
        dialogLines[currentLineIndex] = dialogLines[currentLineIndex].Replace("[PlayerName]", playerName);
        UpdateAllPortraits(currentSpeaker);
        StartCoroutine(TypewriteText(dialogLines[currentLineIndex], currentSpeaker));

    }

    private void StartSkillCheck()
    {
        isSkillCheckTime = true;
        skillCheckStage = 0;
        skillCheckScreen.SetActive(true);
        skillCheckArea.SetActive(true);

        // Rolls which card is drawn, THIS WILL BE CHANGED LATER WHEN MORE CARDS ARE ADDED
        modifier = Random.Range(1, 6);
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

        Card skillCheckCard = Instantiate(cardPrefabs[skillShake], cardParent.transform);
        skillCheckCard.Power = modifier;
        skillCheckCard.transform.parent = cardParent.transform;
    }

    private IEnumerator ShowRollingEffect()
    {
        isRollingDisplayActive = true;
        while (isRollingDisplayActive)
        {
            int dice1Num = Random.Range(0, 6);
            int dice2Num = Random.Range(0, 6);
            textOutput.text = $"{dice1Num + 1} + {dice2Num + 1}";

            dice1.sprite = diceOptions[dice1Num];
            dice2.sprite = diceOptions[dice1Num];

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void HandleSkillCheckClick()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.UiClick, this.transform.position); // Play click sound

        switch (skillCheckStage)
        {
            case 0:
                StartCoroutine(ShowRollingEffect());
                skillCheckStage = 1;
                diceShake.start();

                break;
            case 1:
                isRollingDisplayActive = false;
                diceRoll1 = Random.Range(0, 6);
                diceRoll2 = Random.Range(0, 6);

                dice1.sprite = diceOptions[diceRoll1];
                dice2.sprite = diceOptions[diceRoll2];

                int rollTotal = (diceRoll1 + 1) + (diceRoll2 + 1) + modifier;
                if (skillShake == currentSkillIndex)
                {
                    rollTotal += 1;
                    modifierText.SetActive(true);
                }

                totalText.text = rollTotal.ToString();

                skillCheckPassed = rollTotal >= skillCheckThreshold;
                if (skillShake == currentSkillIndex)
                {
                    textOutput.text = $"{diceRoll1 + 1} + {diceRoll2 + 1} + ({modifier}) + (1 | same type bonus!) = {rollTotal}";
                }
                else
                {
                    textOutput.text = $"{diceRoll1 + 1} + {diceRoll2 + 1} + ({modifier}) = {rollTotal}";
                }
                skillCheckStage = 2;
                diceShake.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                AudioManager.instance.PlayOneShot(FMODEvents.instance.DiceRoll, this.transform.position);

                break;

            case 2:
                textOutput.text = skillCheckPassed ? "Skill Check Passed!" : "Skill Check Failed...";
                skillCheckText.text = textOutput.text;
                skillCheckStage = 3;
                break;

            case 3:
                LoadSkillCheckPath(skillCheckPassed);
                DisplayLine();
                isSkillCheckTime = false;
                skillCheckScreen.SetActive(false);
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
            AudioManager.instance.PlayOneShot(FMODEvents.instance.GoodResponse, this.transform.position); // Play good sound
        }
        else
        {
            dialogLines = new List<string>(skillCheckPaths.failureDialogLines);
            speakersPerLine = new List<string>(skillCheckPaths.failureSpeakersPerLine);
            characterSprites = new List<SpriteOptions>(skillCheckPaths.failureSpriteOptions);
            AudioManager.instance.PlayOneShot(FMODEvents.instance.BadResponse, this.transform.position); // Play sound sound
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
        continueIndicator.SetActive(false);
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
        continueIndicator.SetActive(true);
        UpdateVoice(speaker);
    }

    public void NextLine()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.UiClick, this.transform.position); // Play click sound
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

    private IEnumerator PlayVoiceLoop(EventInstance voiceInstance)
    {
        PLAYBACK_STATE playbackState;

        while (isTypewriting)
        {
            voiceInstance.getPlaybackState(out playbackState);

            if (playbackState != PLAYBACK_STATE.PLAYING && playbackState != PLAYBACK_STATE.STARTING)
            {
                voiceInstance.start(); // Start audio if not already playing
            }

            yield return null; // Wait for the next frame
        }

        // Once typewriting ends, stop the audio
        voiceInstance.getPlaybackState(out playbackState);
        if (playbackState == PLAYBACK_STATE.PLAYING || playbackState == PLAYBACK_STATE.STARTING)
        {
            //voiceInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    private void UpdateVoice(string speaker)
    {
        EventInstance voiceInstance = noVoice; // Default to player voice
        
        // Determine the correct voice instance
        if (speaker == "You" || speaker == playerName)
        {
            voiceInstance = playerVoice;
        }
        else if (speaker == "Player")
        {
            voiceInstance = playerVoice;
            Debug.Log("Player");
        }
        else if (speaker == "Noki")
        {
            voiceInstance = nokiVoice;
            Debug.Log("Noki");
        }
        else if (speaker == "Lotte")
        {
            voiceInstance = lotteVoice;
        }
        else if (speaker == "Celci")
        {
            voiceInstance = celciVoice;
        }
        else if (speaker == "Miguel")
        {
            voiceInstance = miguelVoice;
        }
        else if (speaker == "John Fishman")
        {
            voiceInstance = fishVoice;
        }
        else if (speaker == "CEO")
        {
            voiceInstance = ceoVoice;
        }
        else if (speaker == "C0D3W1Z4RD")
        {
            voiceInstance = wizardVoice;
        }
        else if (speaker == "Deliah")
        {
            voiceInstance = deliahVoice;
        }
        else if (speaker == "")
        {
            voiceInstance = noVoice;
        }
        // Manage the voice playback coroutine
        if (isTypewriting)
        {
            if (voiceCoroutine == null) // Start the loop if not already running
            {
                voiceCoroutine = StartCoroutine(PlayVoiceLoop(voiceInstance));
            }
        }
        else
        {
            if (voiceCoroutine != null) // Stop the loop when typewriting ends
            {
                StopCoroutine(voiceCoroutine);
                voiceCoroutine = null;

                voiceInstance.getPlaybackState(out var playbackState);
                if (playbackState == PLAYBACK_STATE.PLAYING)
                {
                    //voiceInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); // Stop audio safely
                }
            }
        }
    }

    private void InitializeAudio()
    {
        playerVoice = AudioManager.instance.CreateInstance(FMODEvents.instance.PlayerVoice);
        nokiVoice = AudioManager.instance.CreateInstance(FMODEvents.instance.NokiVoice);
        lotteVoice = AudioManager.instance.CreateInstance(FMODEvents.instance.LotteVoice);
        celciVoice = AudioManager.instance.CreateInstance(FMODEvents.instance.CelciVoice);
        miguelVoice = AudioManager.instance.CreateInstance(FMODEvents.instance.MiguelVoice);
        fishVoice = AudioManager.instance.CreateInstance(FMODEvents.instance.FishVoice);
        ceoVoice = AudioManager.instance.CreateInstance(FMODEvents.instance.CeoVoice);
        wizardVoice = AudioManager.instance.CreateInstance(FMODEvents.instance.WizardVoice);
        deliahVoice = AudioManager.instance.CreateInstance(FMODEvents.instance.DeliahVoice);
        nokiVoice = AudioManager.instance.CreateInstance(FMODEvents.instance.noVoice);
        diceShake = AudioManager.instance.CreateInstance(FMODEvents.instance.DiceShake);
    }
}