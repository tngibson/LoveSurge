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

    [SerializeField] private List<Sprite> backgrounds = new List<Sprite>(); // List holding all background change objects
    [SerializeField] private Image background; // Scene background
    [SerializeField] private float backgroundFadeDuration = 1f;

    private int currentLineIndex = 0; // Current index for dialog
    private bool isTypewriting = false; // Whether the typewriter coroutine is going
    private bool skipRequested = false; // If the skip button is pressed
    private bool isChoiceTime = false; // If there is a choice to be madech
    private bool isChoiceDialog = false; // If we are reading choice dialog

    // Backup the original dialog and speakers to restore after choices
    private List<string> originalDialogLines;
    private List<string> originalSpeakersPerLine;
    private List<SpriteOptions> originalSpriteOptions;
    private int originalLineIndex;  // Store the current index before the choice
    private string lastTargetSpriteName;

    // Typewriter variables
    private float typewriterSpeed = 0.025f;

    // FMOD Audio
    private EventInstance playerVoice;
    private EventInstance nokiVoice;
    private EventInstance lotteVoice;
    private EventInstance celciVoice;
    private EventInstance miguelVoice;
    private EventInstance fishVoice;
    private EventInstance ceoVoice;
    private EventInstance wizardVoice;
    private EventInstance deliahVoice;

    private EventInstance noVoice; // Narrartion

    private Coroutine voiceCoroutine; // To track the active coroutine

    private Player playerManager;
    private string playerName;

    [SerializeField] private GameObject continueIndicator;

    [SerializeField] private Image cgImage; // The CG image to fade in/out
    [SerializeField] private GameObject uiContainer; // All UI elements that should hide
    [SerializeField] private CanvasGroup uiCanvasGroup; // For fading UI without disabling it
    [SerializeField] private float cgFadeDuration = 0.5f; // How long fade takes

    private bool isCGActive = false; // Are we currently in CG mode?
    private bool isWaitingForCGClick = false; // Waiting for player to click after CG shows

    private Coroutine typewriterCoroutine; // track current typewriter

    [SerializeField] private CelciIntroChoiceData celciIntroDialog; // Shown if house is not hot

    void Start()
    {
        // Set the playerManager and get the player's preferred name
        if (GameObject.Find("PlayerManager") != null)
        {
            playerManager = GameObject.Find("PlayerManager").GetComponent<Player>();
            playerName = playerManager.GetName();
        }
        else
        {
            Debug.LogWarning("Player Manager was null!");
        }

        choicePanel.SetActive(false); // Hide choice panel at start
        mapButton.SetActive(false);   // Hide the map button at start
        InitializeAudio(); // Starts FMOD audio
        DisplayLine(); // Display the first line
    }

    void Update()
    {
        // If we're in CG waiting mode, only handle that
        if (isWaitingForCGClick && Input.GetButtonDown("Skip"))
        {
            isWaitingForCGClick = false;
            StartCoroutine(FadeCanvasGroup(uiCanvasGroup, 0f, 1f, cgFadeDuration));
            StartCoroutine(ResumeAfterCG());
            return; // stop here so normal skip doesn't run
        }

        // Check for skip input
        if (Input.GetButtonDown("Skip"))
        {
            if (isTypewriting && !isWaitingForCGClick)
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

        if (dialogLines[currentLineIndex] == "CGSTART")
        {
            StartCoroutine(HandleCGStart());
            return;
        }

        if (dialogLines[currentLineIndex] == "CGEND")
        {
            StartCoroutine(HandleCGEnd());
            return;
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

        // Checks if we find a CELCIINTROCHOICE
        if (currentLineIndex < dialogLines.Count)
        {
            if (dialogLines[currentLineIndex] == "CELCIINTROCHOICE")
            {
                HandleCelciIntroChoice();
                return;
            }
        }

        // Checks if we find a BACKGROUNDCHANGE
        if (currentLineIndex < dialogLines.Count)
        {
            if (dialogLines[currentLineIndex] == "BACKGROUNDCHANGE")
            {
                ChangeBackground();
                return;
            }
        }

        // Set the speaker name based on the current line's speaker
        string currentSpeaker = speakersPerLine[currentLineIndex];

        // If the speaker is player, we will set their name to playerName. If for whatever reason the playerName variable is empty, we won't set it
        if (currentSpeaker == "You")
        {
            if (playerName != "" && playerName != null)
            {
                currentSpeaker = playerName;
            }
        }

        speakerNameText.text = currentSpeaker;

        dialogLines[currentLineIndex] = dialogLines[currentLineIndex].Replace("[Player]", playerName);
        dialogLines[currentLineIndex] = dialogLines[currentLineIndex].Replace("[PlayerName]", playerName);
        dialogLines[currentLineIndex] = dialogLines[currentLineIndex].Replace("[Player Name]", playerName);

        // Update all portraits based on the current line
        UpdateAllPortraits(currentSpeaker);

        // Start the typewriter effect
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }
        typewriterCoroutine = StartCoroutine(TypewriteText(dialogLines[currentLineIndex], currentSpeaker));
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
                Sprite targetSprite = characterSprites[i].spriteOptions[currentLineIndex];
                if (targetSprite != null)
                {
                    characterPortraits[i].sprite = targetSprite;
                    lastTargetSpriteName = targetSprite.name;

                    // Check if animation can be invoked
                    if (CharacterAnimator.HasAnimator(characterPortraits[i]))
                    {
                        CharacterAnimator.InvokeStartAnimation(this, new AnimatorEventData
                        {
                            State = targetSprite.name,
                            Speaker = currentSpeaker
                        });
                    }
                }

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
                break;
            }
            textOutput.text += letter;
            yield return new WaitForSeconds(typewriterSpeed);
        }

        isTypewriting = false;
        continueIndicator.SetActive(true);

        // Safely invoke animation stop
        for (int i = 0; i <characterPortraits.Count; i++)
        {
            if (CharacterAnimator.HasAnimator(characterPortraits[i]))
            {
                if (i < characterPortraits.Count && characterSprites[i].spriteOptions.Count > 0)
                {
                    Sprite targetSprite = characterSprites[i].spriteOptions[currentLineIndex];
                    if (targetSprite != null)
                    {
                        characterPortraits[i].sprite = targetSprite;
                        lastTargetSpriteName = targetSprite.name;

                        CharacterAnimator.InvokeStopAnimation(this, new AnimatorEventData()
                        {
                            State = lastTargetSpriteName,
                            Speaker = speaker
                        });
                    }
                }
            }
        }

        UpdateVoice(speaker);
    }

    public void NextLine()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.UiClick, this.transform.position); // Play click sound
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

        // Update the layout of the buttons
        UpdateChoiceButtonLayout();
    }

    private void HandleCelciIntroChoice()
    {
        // Backup the current dialog to return to later
        originalDialogLines = new List<string>(dialogLines);
        originalSpeakersPerLine = new List<string>(speakersPerLine);
        originalSpriteOptions = new List<SpriteOptions>(characterSprites);
        originalLineIndex = currentLineIndex + 1; // Resume after this marker
        isChoiceDialog = true;

        // If the house is not hot, skip this block
        if (!Player.instance.isHouseHot)
        {
            currentLineIndex++;
            DisplayLine();
            return;
        }

        // Use the inspector-defined dialog if available
        if (celciIntroDialog != null && celciIntroDialog.lines.Count > 0)
        {
            dialogLines = new List<string>(celciIntroDialog.lines);
            speakersPerLine = new List<string>(celciIntroDialog.speakers);
            characterSprites = new List<SpriteOptions>(celciIntroDialog.spriteOptions);
        }
        else
        {
            Debug.LogWarning("CelciIntroChoiceData not set up in inspector! Using fallback text.");

            dialogLines = new List<string>
        {
            "Celci: Whoa, it’s freezing in here!",
            "You: Sorry about that... the heater’s busted again.",
            "Celci: That’s not great for a guest, [Player]. Let’s fix that later."
        };

            speakersPerLine = new List<string> { "Celci", "You", "Celci" };

            characterSprites = new List<SpriteOptions>();
            for (int i = 0; i < speakersPerLine.Count; i++)
                characterSprites.Add(new SpriteOptions());
        }

        currentLineIndex = 0;
        DisplayLine();
    }



    private void ChangeBackground()
    {
        StartCoroutine(FadeBackground(backgrounds[currentLineIndex], backgroundFadeDuration));
    }

    private IEnumerator FadeBackground(Sprite newSprite, float duration)
    {
        // Create a temporary overlay Image (same parent/layer as the main background)
        GameObject tempObj = new GameObject("TempBackground");
        Image tempImage = tempObj.AddComponent<Image>();
        tempImage.sprite = background.sprite; // start with the current sprite
        tempImage.rectTransform.SetParent(background.transform.parent, false);
        tempImage.rectTransform.anchorMin = background.rectTransform.anchorMin;
        tempImage.rectTransform.anchorMax = background.rectTransform.anchorMax;
        tempImage.rectTransform.offsetMin = background.rectTransform.offsetMin;
        tempImage.rectTransform.offsetMax = background.rectTransform.offsetMax;
        tempImage.preserveAspect = true;
        tempImage.raycastTarget = false; // so it doesn’t block clicks

        // Put it behind UI but over the real background
        tempImage.transform.SetSiblingIndex(background.transform.GetSiblingIndex());

        // Set the new background sprite (invisible for now)
        background.sprite = newSprite;
        background.color = new Color(1, 1, 1, 0f);

        // Fade
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            background.color = new Color(1, 1, 1, t);
            tempImage.color = new Color(1, 1, 1, 1 - t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure fully transitioned
        background.color = Color.white;
        Destroy(tempObj);
    }

    private void UpdateChoiceButtonLayout()
    {
        int activeChoices = 0;

        // Count the number of active choice buttons
        foreach (var button in choiceButtons)
        {
            if (button.gameObject.activeSelf)
            {
                activeChoices++;
            }
        }

        // Ensure we only ever have two layers with two buttons max per layer
        int maxButtonsPerLayer = 2;
        float horizontalSpacing = 650f; // Space between buttons horizontally
        float verticalSpacing = 265f;   // Space between layers vertically

        int index = 0;
        foreach (var button in choiceButtons)
        {
            if (button.gameObject.activeSelf)
            {
                RectTransform buttonRect = button.GetComponent<RectTransform>();

                // Determine layer (0 or 1) and horizontal index (0 or 1)
                int layer = index / maxButtonsPerLayer;
                int horizontalIndex = index % maxButtonsPerLayer;

                // Calculate horizontal position to center the two buttons
                float xPos = -((maxButtonsPerLayer - 1) * horizontalSpacing) / 2 + (horizontalIndex * horizontalSpacing);

                // Calculate vertical position to create two layers
                float yPos = -((layer * verticalSpacing));

                // Apply the position
                buttonRect.anchoredPosition = new Vector2(xPos, yPos + 200);
                index++;
            }
        }
    }

    public void OnChoiceSelected(int choiceIndex)
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.UiClick, this.transform.position);
        isChoiceTime = false;
        choicePanel.SetActive(false);

        // Save the current main dialog state to return to after the nested path
        originalDialogLines = new List<string>(dialogLines);
        originalSpeakersPerLine = new List<string>(speakersPerLine);
        originalSpriteOptions = new List<SpriteOptions>(characterSprites);
        originalLineIndex = currentLineIndex + 1;

        Choices currentChoices = choices[currentLineIndex];
        ChoicePath selectedPath = currentChoices.choicePaths[choiceIndex];

        // Apply stat changes if applicable
        if (!string.IsNullOrEmpty(selectedPath.statTag) && selectedPath.statValue != 0)
            ApplyStatChange(selectedPath.statTag, selectedPath.statValue);

        // Start with the dialog lines for this choice
        dialogLines = selectedPath.afterChoiceDialogLines;
        speakersPerLine = selectedPath.afterChoiceSpeakersPerLine;
        characterSprites = selectedPath.afterChoiceSpriteOptions;

        // Support background change and Celci intro customization for this choice path
        if (selectedPath.backgrounds != null && selectedPath.backgrounds.Count > 0)
            backgrounds = new List<Sprite>(selectedPath.backgrounds);

        if (selectedPath.celciIntroDialog != null)
            celciIntroDialog = selectedPath.celciIntroDialog;

        currentLineIndex = 0;

        // If this choice has its own nested choices, track them
        if (selectedPath.nestedChoices != null && selectedPath.nestedChoices.Count > 0)
        {
            // Temporarily replace the main choices list with the nested ones
            choices = selectedPath.nestedChoices;
            isChoiceDialog = true;
        }

        DisplayLine();
    }

    private void ApplyStatChange(string statTag, int statValue)
    {
        if (playerManager == null) return;

        // Match the statTag with the enum in the Player class
        if (System.Enum.TryParse<Player.StatType>(statTag, out var statType))
        {
            int currentStat = playerManager.GetStat(statType);
            playerManager.SetStat(statType, currentStat + statValue);
            Debug.Log($"{statType} increased by {statValue}. New value: {currentStat + statValue}");
        }
        else if (statTag == "Stress")
        {
            StressManager.instance?.AddToCurrentStress();
        }
        else if (statTag == "CelciDate1IntroChoice1")
        {
            Player.instance.isHouseHot = false;
        }
        else
        {
            Debug.LogWarning($"Invalid stat tag: {statTag}");
        }
    }

    private void EndChoicePath()
    {
        // If we had nested choices, revert to the previous layer
        if (isChoiceDialog)
        {
            dialogLines = originalDialogLines;
            speakersPerLine = originalSpeakersPerLine;
            characterSprites = originalSpriteOptions;
            currentLineIndex = originalLineIndex;
            isChoiceDialog = false;

            // Optional: if you used nested choices, clear them to avoid confusion
            choices = new List<Choices>(choices);

            DisplayLine();
        }
        else
        {
            // No nested choice to return from — just resume normal dialog
            mapButton.SetActive(true);
        }
    }

    private IEnumerator PlayVoiceLoop(EventInstance voiceInstance)
    {
        PLAYBACK_STATE playbackState;

        while (isTypewriting) // Run as long as typewriting is active
        {
            voiceInstance.getPlaybackState(out playbackState);

            if (playbackState != PLAYBACK_STATE.PLAYING && playbackState != PLAYBACK_STATE.STARTING)
            {
                voiceInstance.start(); // Ensure only one instance is playing
            }

            yield return null; // Wait until the next frame
        }

        // Stop the voice playback gracefully when typewriting ends
        voiceInstance.getPlaybackState(out playbackState);

        if (playbackState == PLAYBACK_STATE.PLAYING || playbackState == PLAYBACK_STATE.STARTING)
        {
            //voiceInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    // FMOD Sound Functions
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
        }
        else if (speaker == "Noki")
        {
            voiceInstance = nokiVoice;
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
        noVoice = AudioManager.instance.CreateInstance(FMODEvents.instance.noVoice);
        playerVoice = AudioManager.instance.CreateInstance(FMODEvents.instance.PlayerVoice);
        nokiVoice = AudioManager.instance.CreateInstance(FMODEvents.instance.NokiVoice);
        lotteVoice = AudioManager.instance.CreateInstance(FMODEvents.instance.LotteVoice);
        celciVoice = AudioManager.instance.CreateInstance(FMODEvents.instance.CelciVoice);
        miguelVoice = AudioManager.instance.CreateInstance(FMODEvents.instance.MiguelVoice);
        fishVoice = AudioManager.instance.CreateInstance(FMODEvents.instance.FishVoice);
        ceoVoice = AudioManager.instance.CreateInstance(FMODEvents.instance.CeoVoice);
        wizardVoice = AudioManager.instance.CreateInstance(FMODEvents.instance.WizardVoice);
        deliahVoice = AudioManager.instance.CreateInstance(FMODEvents.instance.DeliahVoice);
    }

    private IEnumerator FadeImage(Image image, float startAlpha, float endAlpha, float duration)
    {
        Color c = image.color;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, endAlpha, t / duration);
            image.color = c;
            yield return null;
        }
        c.a = endAlpha;
        image.color = c;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float startAlpha, float endAlpha, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            group.alpha = Mathf.Lerp(startAlpha, endAlpha, t / duration);
            yield return null;
        }
        group.alpha = endAlpha;
    }

    private IEnumerator HandleCGStart()
    {
        // No text typing during CGSTART
        textOutput.text = "";
        continueIndicator.SetActive(false);

        // Show CG image & fade in while fading out UI
        cgImage.gameObject.SetActive(true);
        StartCoroutine(FadeImage(cgImage, 0f, 1f, cgFadeDuration));
        yield return StartCoroutine(FadeCanvasGroup(uiCanvasGroup, 1f, 0f, cgFadeDuration));

        // Wait for player click
        isWaitingForCGClick = true;
    }

    private IEnumerator HandleCGEnd()
    {
        // Fade CG out while letting text continue
        yield return StartCoroutine(FadeImage(cgImage, 1f, 0f, cgFadeDuration));
        cgImage.gameObject.SetActive(false);

        // Continue immediately
        currentLineIndex++;
        DisplayLine();
    }

    private IEnumerator ResumeAfterCG()
    {
        yield return new WaitForSeconds(cgFadeDuration);
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }
        currentLineIndex++;
        DisplayLine();
    }
}

[System.Serializable]
public class SpriteOptions
{
    public List<Sprite> spriteOptions = new List<Sprite>();
}

[System.Serializable]
public class CelciIntroChoiceData
{
    public List<string> lines = new List<string>();                // Dialog lines for this event
    public List<string> speakers = new List<string>();             // Matching speakers
    public List<SpriteOptions> spriteOptions = new List<SpriteOptions>(); // Matching character sprites
}