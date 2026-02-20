using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static LocationManager;

public class RandEventHandler : MonoBehaviour
{
    [Header("Portrait Layout")]
    [SerializeField] private float screenPadding = 150f; // Distance from left/right edge
    [SerializeField] private float portraitSpacingPadding = 50f; // Extra spacing between portraits
    [SerializeField] private float additionalCenterSpread = 350f;

    [SerializeField] private Image fadeOverlay;
    [SerializeField] private float fadeDuration = 0.5f;

    [SerializeField] private List<string> dialogLines = new List<string>();     // Holds the main dialog lines
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

    [Header("Celci Specific Options")]
    [SerializeField] private CelciIntroChoiceData celciIntroDialog; // Shown if house is not hot
    [SerializeField] private CelciDate2ChoiceData celciDate2Dialog; // Shown if player threatens Celci

    [Header("Main Event 1.5 Branch Options")]
    // PT2
    [SerializeField] private MainEvent1dot5BranchPt2Data mainEvent1dot5BranchPt2_Branch1True;
    [SerializeField] private MainEvent1dot5BranchPt2Data mainEvent1dot5BranchPt2_Branch1False;
    // PT3
    [SerializeField] private MainEvent1dot5BranchPt3Data mainEvent1dot5BranchPt3_Branch1True;
    [SerializeField] private MainEvent1dot5BranchPt3Data mainEvent1dot5BranchPt3_Branch1False;

    [Header("Main Event 3 Current Route Options")]
    [SerializeField] private MainStory3CurrentRouteData mainStory3_Noki;
    [SerializeField] private MainStory3CurrentRouteData mainStory3_Celci;
    [SerializeField] private MainStory3CurrentRouteData mainStory3_Lotte;

    [SerializeField] private bool isNokiFinalDeepConvo = false;
    [SerializeField] private bool isCelciFinalDeepConvo = false;
    [SerializeField] private bool isLotteFinalDeepConvo = false;

    // Stack-based system for nested choices
    private Stack<DialogState> dialogStateStack = new Stack<DialogState>();

    [SerializeField] private Sprite transparentSprite;

    [SerializeField] private GameObject bigNoki;

    private bool isFading = false;

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

        if (Player.instance.celciRomanticRoute && isCelciFinalDeepConvo)
        {
            dialogLines[90] = "Celci leans over and kisses you on the cheek. It feels like a spot of warm sunlight.";
            dialogLines[91] = "Thanks for the other stuff you said too. ";
            speakersPerLine[91] = "Celci";
            dialogLines[97] = "They hold your arm closely to them. You walk home together as the sun peeks up over the horizon.";
        }

        // If we have finished all Dates, we go to DemoOutro when we finish the Dialog
        if ((isNokiFinalDeepConvo || isCelciFinalDeepConvo || isLotteFinalDeepConvo) && (LocationManager.Instance.characterDates[0].allDatesDone && LocationManager.Instance.characterDates[1].allDatesDone && LocationManager.Instance.characterDates[2].allDatesDone))
        {
            mapButton.GetComponent<MapScript>().locName = "DemoOutro";
        }

        DisplayLine(); // Display the first line
    }

    void Update()
    {
        if (isFading) return;

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

        // Checks if we find a CELCIDATE2CHOICE
        if (currentLineIndex < dialogLines.Count)
        {
            if (dialogLines[currentLineIndex] == "CELCIDATE2CHOICE")
            {
                HandleCelciDate2Choice();
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

        if (dialogLines[currentLineIndex] == "MAINEVENT1.5BRANCHPT2")
        {
            HandleMainEvent1dot5BranchPt2();
            return;
        }

        if (dialogLines[currentLineIndex] == "MAINEVENT1.5BRANCHPT3")
        {
            HandleMainEvent1dot5BranchPt3();
            return;
        }

        if (dialogLines[currentLineIndex] == "MAINSTORY3CURRENTROUTE")
        {
            HandleMainStory3CurrentRoute();
            return;
        }

        if (currentLineIndex < dialogLines.Count)
        {
            if (dialogLines[currentLineIndex] == "PLAYCREEPYSTART")
            {
                if (MusicManager.Instance != null)
                {
                    MusicManager.Instance.StopMusic();
                    MusicManager.Instance.PlayMusic(MusicManager.Instance.CreepyTheme);
                }
                NextLine();
                return;
            }
        }

        if (currentLineIndex < dialogLines.Count)
        {
            if (dialogLines[currentLineIndex] == "PLAYCREEPYEND")
            {
                if (MusicManager.Instance != null)
                {
                    MusicManager.Instance.StopMusic();
                    MusicManager.Instance.PlayMusic(MusicManager.Instance.DeepConversation);
                }
                NextLine();
                return;
            }
        }

        if (currentLineIndex < dialogLines.Count)
        {
            if (dialogLines[currentLineIndex] == "PLAYFUTURISTICSTART")
            {
                if (MusicManager.Instance != null)
                {
                    MusicManager.Instance.StopMusic();
                    //MusicManager.Instance.PlayMusic(MusicManager.Instance.FuturisticMystery);
                }
                NextLine();
                return;
            }
        }

        if (currentLineIndex < dialogLines.Count)
        {
            if (dialogLines[currentLineIndex] == "PLAYFUTURISTICEND")
            {
                if (MusicManager.Instance != null)
                {
                    MusicManager.Instance.StopMusic();
                    MusicManager.Instance.PlayMusic(MusicManager.Instance.DeepConversation);
                }
                NextLine();
                return;
            }
        }

        if (dialogLines[currentLineIndex] == "BIGNOKISTART")
        {
            bigNoki.SetActive(true);
            NextLine();
            return;
        }

        if (dialogLines[currentLineIndex] == "PLAYCHIME")
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.OmniverseJingle, this.transform.position);
            NextLine();
            return;
        }

        if (dialogLines[currentLineIndex] == "BIGNOKIEND")
        {
            bigNoki.SetActive(false);
            NextLine();
            return;
        }

        if (dialogLines[currentLineIndex] == "FADETOBLACK")
        {
            StartCoroutine(FadeToBlack());
            return;
        }

        if (dialogLines[currentLineIndex] == "FADEBACKNORMAL")
        {
            StartCoroutine(FadeBackToNormal());
            return;
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

        RecenterPortraits();
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
        for (int i = 0; i < characterPortraits.Count; i++)
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
            "Celci: Whoa, it�s freezing in here!",
            "You: Sorry about that... the heater�s busted again.",
            "Celci: That�s not great for a guest, [Player]. Let�s fix that later."
        };

            speakersPerLine = new List<string> { "Celci", "You", "Celci" };

            characterSprites = new List<SpriteOptions>();
            for (int i = 0; i < speakersPerLine.Count; i++)
                characterSprites.Add(new SpriteOptions());
        }

        currentLineIndex = 0;
        DisplayLine();
    }

    private void HandleCelciDate2Choice()
    {
        // Backup the current dialog to return to later
        originalDialogLines = new List<string>(dialogLines);
        originalSpeakersPerLine = new List<string>(speakersPerLine);
        originalSpriteOptions = new List<SpriteOptions>(characterSprites);
        originalLineIndex = currentLineIndex + 1; // Resume after this marker
        isChoiceDialog = true;

        // If Celci is not threatened, skip this block
        if (!Player.instance.isCelciThreatened)
        {
            currentLineIndex++;
            DisplayLine();
            return;
        }

        // Use the inspector-defined dialog if available
        if (celciDate2Dialog != null && celciDate2Dialog.lines.Count > 0)
        {
            dialogLines = new List<string>(celciDate2Dialog.lines);
            speakersPerLine = new List<string>(celciDate2Dialog.speakers);
            characterSprites = new List<SpriteOptions>(celciDate2Dialog.spriteOptions);
        }
        else
        {
            Debug.LogWarning("CelciDate2ChoiceData not set up in inspector! Using fallback text.");

            dialogLines = new List<string>
        {
            "Celci: Whoa, it�s freezing in here!",
            "You: Sorry about that... the heater�s busted again.",
            "Celci: That�s not great for a guest, [Player]. Let�s fix that later."
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
        tempImage.raycastTarget = false; // so it doesn�t block clicks

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

        // Save the current state so we can return after nested choices
        dialogStateStack.Push(new DialogState(dialogLines, speakersPerLine, characterSprites, currentLineIndex + 1, choices));

        Choices currentChoices = choices[currentLineIndex];
        ChoicePath selectedPath = currentChoices.choicePaths[choiceIndex];

        // Apply stat effects if applicable
        if (!string.IsNullOrEmpty(selectedPath.statTag) && selectedPath.statValue != 0)
            ApplyStatChange(selectedPath.statTag, selectedPath.statValue);

        // Jump into the chosen path
        dialogLines = selectedPath.afterChoiceDialogLines;
        speakersPerLine = selectedPath.afterChoiceSpeakersPerLine;
        characterSprites = selectedPath.afterChoiceSpriteOptions;
        currentLineIndex = 0;

        // If this path contains nested choices, replace the list
        if (selectedPath.nestedChoices != null && selectedPath.nestedChoices.Count > 0)
        {
            choices = selectedPath.nestedChoices;
            isChoiceDialog = true;
        }
        else
        {
            // Keep the current choices if none are nested
            choices = new List<Choices>();
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
        else if (statTag == "CelciDate2Choice1")
        {
            Player.instance.isCelciThreatened = true;
        }
        else if (statTag == "CelciDateEnd")
        {
            LocationManager.Instance.characterDates[1].allDatesDone = true;
            LocationManager.Instance.characterDates[1].date3Stage = Date3Stage.Done;
        }
        else if (statTag == "NokiRomantic")
        {
            Player.instance.nokiRomanticRoute = true;
        }
        else if (statTag == "CelciRomantic")
        {
            Player.instance.celciRomanticRoute = true;
        }
        else if (statTag == "LotteRomantic")
        {
            Player.instance.lotteRomanticRoute = true;
        }
        else if (statTag == "MainEvent1.5Branch1")
        {
            Player.instance.MainEvent1dot5Branch1 = true;
        }
        else if (statTag == "MainEvent1.5Branch2")
        {
            Player.instance.MainEvent1dot5Branch2 = true;
        }
        else
        {
            Debug.LogWarning($"Invalid stat tag: {statTag}");
        }
    }

    private void EndChoicePath()
    {
        // If there are previous states saved, pop back to them
        if (dialogStateStack.Count > 0)
        {
            DialogState previousState = dialogStateStack.Pop();

            dialogLines = previousState.dialogLines;
            speakersPerLine = previousState.speakersPerLine;
            characterSprites = previousState.spriteOptions;
            currentLineIndex = previousState.lineIndex;
            choices = previousState.choicesList;

            // If there are still states left, we�re still inside nested dialogue
            isChoiceDialog = dialogStateStack.Count > 0;

            DisplayLine();
        }
        else
        {
            // Stack is empty then all choices finished, end the event
            isChoiceDialog = false;
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

    private void HandleMainEvent1dot5BranchPt1()
    {
        // Save current state so we return after the branch finishes
        dialogStateStack.Push(new DialogState(
            dialogLines,
            speakersPerLine,
            characterSprites,
            currentLineIndex + 1, // Resume AFTER the marker
            choices
        ));

        isChoiceDialog = true;

        List<string> branchLines = new List<string>();
        List<string> branchSpeakers = new List<string>();
        List<SpriteOptions> branchSprites = new List<SpriteOptions>();

        // Branch 1
        if (Player.instance.MainEvent1dot5Branch1)
        {
            branchLines = new List<string>
        {
            "Hello [PlayerName],",
            "It is I, Steven Stoveton (call me Steve).",
            "I am reaching out in regards to the email you sent our support line a while back.",
            "Thank you for your diligence and transparency!",
            "I, Steven Stoveton (Steven Stoveton was my father so please feel free to call me steve), would like you and your companions to come to my office next Monday.",
            "I have some questions about your experience.",
            "Thank you for being the SPARK! in OMNISPARK!",
            "Your Boss, Steve"
        };

            branchSpeakers = new List<string>
        {
            "Steve",
            "Steve",
            "Steve",
            "Steve",
            "Steve",
            "Steve",
            "Steve",
            "Steve",
        };
        }
        // Branch 2 (assumed true if Branch1 is false)
        else
        {
            branchLines = new List<string>
        {
            "Hello [PlayerName],",
            "It is I, Steven Stoveton (call me Steve).",
            "You are a valued and important intern for this company.",
            "Because of your great successes in our program you have been selected to report to me personally.",
            "I, Steven Stoveton (Steven Stoveton was my father so please feel free to call me steve), would like you and your companions to come to my office next Monday.",
            "I have some questions about your experience.",
            "Thank you for being the SPARK! in OMNISPARK!",
            "Your Boss, Steve"
        };

            branchSpeakers = new List<string>
        {
            "Steve",
            "Steve",
            "Steve",
            "Steve",
            "Steve",
            "Steve",
            "Steve",
            "Steve",
        };
        }

        // Build empty sprite options matching speaker count
        int spriteOptionCount = characterPortraits.Count;

        // Ensure we have enough sprite entries for the longest possible branch
        int spriteEntriesPerOption = Mathf.Max(branchLines.Count, 1);

        for (int i = 0; i < spriteOptionCount; i++)
        {
            SpriteOptions option = new SpriteOptions();

            for (int j = 0; j < spriteEntriesPerOption; j++)
            {
                option.spriteOptions.Add(transparentSprite);
            }

            branchSprites.Add(option);
        }

        // Replace current dialog with branch dialog
        dialogLines = branchLines;
        speakersPerLine = branchSpeakers;
        characterSprites = branchSprites;
        choices = new List<Choices>();

        currentLineIndex = 0;

        DisplayLine();
    }

    private void HandleMainEvent1dot5BranchPt2()
    {
        // Backup
        originalDialogLines = new List<string>(dialogLines);
        originalSpeakersPerLine = new List<string>(speakersPerLine);
        originalSpriteOptions = new List<SpriteOptions>(characterSprites);
        originalLineIndex = currentLineIndex + 1;
        isChoiceDialog = true;

        MainEvent1dot5BranchPt2Data selectedData;

        if (Player.instance.MainEvent1dot5Branch1)
            selectedData = mainEvent1dot5BranchPt2_Branch1True;
        else
            selectedData = mainEvent1dot5BranchPt2_Branch1False;

        if (selectedData == null ||
            selectedData.lines == null ||
            selectedData.lines.Count == 0 ||
            selectedData.speakers == null ||
            selectedData.speakers.Count != selectedData.lines.Count)
        {
            Debug.LogWarning("MainEvent1dot5BranchPt2 data invalid or incomplete.");
            currentLineIndex++;
            DisplayLine();
            return;
        }

        dialogLines = new List<string>(selectedData.lines);
        speakersPerLine = new List<string>(selectedData.speakers);
        characterSprites = new List<SpriteOptions>(selectedData.spriteOptions);

        currentLineIndex = 0;
        DisplayLine();
    }

    private void HandleMainEvent1dot5BranchPt3()
    {
        originalDialogLines = new List<string>(dialogLines);
        originalSpeakersPerLine = new List<string>(speakersPerLine);
        originalSpriteOptions = new List<SpriteOptions>(characterSprites);
        originalLineIndex = currentLineIndex + 1;
        isChoiceDialog = true;

        MainEvent1dot5BranchPt3Data selectedData;

        if (Player.instance.MainEvent1dot5Branch1)
            selectedData = mainEvent1dot5BranchPt3_Branch1True;
        else
            selectedData = mainEvent1dot5BranchPt3_Branch1False;

        if (selectedData == null ||
            selectedData.lines == null ||
            selectedData.lines.Count == 0 ||
            selectedData.speakers == null ||
            selectedData.speakers.Count != selectedData.lines.Count)
        {
            Debug.LogWarning("MainEvent1dot5BranchPt3 data invalid or incomplete.");
            currentLineIndex++;
            DisplayLine();
            return;
        }

        dialogLines = new List<string>(selectedData.lines);
        speakersPerLine = new List<string>(selectedData.speakers);
        characterSprites = new List<SpriteOptions>(selectedData.spriteOptions);

        currentLineIndex = 0;
        DisplayLine();
    }

    private void HandleMainStory3CurrentRoute()
    {
        // Backup current dialog state
        originalDialogLines = new List<string>(dialogLines);
        originalSpeakersPerLine = new List<string>(speakersPerLine);
        originalSpriteOptions = new List<SpriteOptions>(characterSprites);
        originalLineIndex = currentLineIndex + 1;
        isChoiceDialog = true;

        MainStory3CurrentRouteData selectedData = null;

        switch (Player.instance.lastCharacterCompleted)
        {
            case 0:
                selectedData = mainStory3_Noki;
                break;

            case 1:
                selectedData = mainStory3_Celci;
                break;

            case 2:
                selectedData = mainStory3_Lotte;
                break;

            default:
                selectedData = mainStory3_Noki;
                break;
        }

        // Validate data before swapping
        if (selectedData == null ||
            selectedData.lines == null ||
            selectedData.lines.Count == 0 ||
            selectedData.speakers == null ||
            selectedData.speakers.Count != selectedData.lines.Count)
        {
            Debug.LogWarning("MAINSTORY3CURRENTROUTE data missing or invalid.");
            currentLineIndex++;
            DisplayLine();
            return;
        }

        // Swap dialog
        dialogLines = new List<string>(selectedData.lines);
        speakersPerLine = new List<string>(selectedData.speakers);
        characterSprites = new List<SpriteOptions>(selectedData.spriteOptions);

        currentLineIndex = 0;
        DisplayLine();
    }

    private IEnumerator FadeToBlack()
    {
        if (fadeOverlay == null)
        {
            Debug.LogWarning("Fade overlay not assigned.");
            currentLineIndex++;
            DisplayLine();
            yield break;
        }

        isFading = true;

        float timer = 0f;
        Color color = fadeOverlay.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            fadeOverlay.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        fadeOverlay.color = new Color(color.r, color.g, color.b, 1f);

        isFading = false;

        currentLineIndex++;
        DisplayLine();
    }


    private IEnumerator FadeBackToNormal()
    {
        if (fadeOverlay == null)
        {
            Debug.LogWarning("Fade overlay not assigned.");
            currentLineIndex++;
            DisplayLine();
            yield break;
        }

        isFading = true;

        float timer = 0f;
        Color color = fadeOverlay.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            fadeOverlay.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        fadeOverlay.color = new Color(color.r, color.g, color.b, 0f);

        isFading = false;

        currentLineIndex++;
        DisplayLine();
    }

    private void RecenterPortraits()
    {
        List<Image> visiblePortraits = new List<Image>();

        foreach (var portrait in characterPortraits)
        {
            if (portrait.sprite != null && portrait.sprite != transparentSprite)
            {
                portrait.gameObject.SetActive(true);
                visiblePortraits.Add(portrait);
            }
            else
            {
                portrait.gameObject.SetActive(false);
            }
        }

        int count = visiblePortraits.Count;
        if (count == 0)
            return;

        RectTransform canvasRect = background.canvas.GetComponent<RectTransform>();
        float screenWidth = canvasRect.rect.width;

        float safeHalfWidth = (screenWidth / 2f) - screenPadding;

        // Estimated visual width of character body
        float estimatedVisualWidth = 500f;

        // Base spacing between portraits (tweakable)
        float baseSpacing = estimatedVisualWidth + portraitSpacingPadding;

        float spacing = baseSpacing + additionalCenterSpread;

        // If layout would spill off screen, compress spacing
        float halfTotalWidth = ((count - 1) * spacing) / 2f;

        if (halfTotalWidth + estimatedVisualWidth / 2f > safeHalfWidth)
        {
            spacing = ((safeHalfWidth - estimatedVisualWidth / 2f) * 2f) / (count - 1);
        }

        // --- CENTER OUTWARD DISTRIBUTION ---

        if (count % 2 == 1)
        {
            // Odd count → true center portrait
            int centerIndex = count / 2;

            for (int i = 0; i < count; i++)
            {
                float offset = (i - centerIndex) * spacing;

                RectTransform rect = visiblePortraits[i].GetComponent<RectTransform>();
                Vector2 pos = rect.anchoredPosition;
                pos.x = offset;
                rect.anchoredPosition = pos;
            }
        }
        else
        {
            // Even count → no exact center, split around 0
            float halfSpacing = spacing / 2f;

            for (int i = 0; i < count; i++)
            {
                float offset = (i - (count / 2f - 0.5f)) * spacing;

                RectTransform rect = visiblePortraits[i].GetComponent<RectTransform>();
                Vector2 pos = rect.anchoredPosition;
                pos.x = offset;
                rect.anchoredPosition = pos;
            }
        }
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

[System.Serializable]
public class CelciDate2ChoiceData
{
    public List<string> lines = new List<string>();                // Dialog lines for this event
    public List<string> speakers = new List<string>();             // Matching speakers
    public List<SpriteOptions> spriteOptions = new List<SpriteOptions>(); // Matching character sprites
}

[System.Serializable]
public class MainEvent1dot5BranchPt2Data
{
    public List<string> lines = new List<string>();
    public List<string> speakers = new List<string>();
    public List<SpriteOptions> spriteOptions = new List<SpriteOptions>();
}

[System.Serializable]
public class MainEvent1dot5BranchPt3Data
{
    public List<string> lines = new List<string>();
    public List<string> speakers = new List<string>();
    public List<SpriteOptions> spriteOptions = new List<SpriteOptions>();
}

[System.Serializable]
public class MainStory3CurrentRouteData
{
    public List<string> lines = new List<string>();
    public List<string> speakers = new List<string>();
    public List<SpriteOptions> spriteOptions = new List<SpriteOptions>();
}

[System.Serializable]
public class DialogState
{
    public List<string> dialogLines;
    public List<string> speakersPerLine;
    public List<SpriteOptions> spriteOptions;
    public int lineIndex;
    public List<Choices> choicesList;

    public DialogState(List<string> d, List<string> s, List<SpriteOptions> sp, int index, List<Choices> c)
    {
        dialogLines = new List<string>(d);
        speakersPerLine = new List<string>(s);
        spriteOptions = new List<SpriteOptions>(sp);
        lineIndex = index;
        choicesList = new List<Choices>(c);
    }
}
