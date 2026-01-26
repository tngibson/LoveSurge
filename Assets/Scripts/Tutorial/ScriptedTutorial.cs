using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class ScriptedTutorial : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    [Header("Dialog UI Components")]
    [SerializeField] private TextMeshProUGUI dialogText;    // UI Text component for displaying dialogue
    [SerializeField] private ScrollRect scrollRect;         // Reference to the Scroll Rect and Content Transform
    [SerializeField] private RectTransform textRectTransform;
    [SerializeField] private float typewriterSpeed = 0.025f;

    [Header("Starting Dialog Events")]
    public UnityEvent<string> dialogEvent;

    [Header("Starting Dialog")]
    [SerializeField] private List<DialogueLines> startingDialog;

    private Player playerManager;
    private string playerName;
    private EventInstance wizardVoice;
    private int lineNum = 0;
    private bool isTypewriting;
    private bool skipRequested;
    [SerializeField] private Playtest currentSession;
    private int maxLineNum;

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

        InitializeAudio(); // Starts FMOD audio
        
        StartCoroutine(PlayDialog(startingDialog));
    }

    private void InitializeAudio()
    {
        wizardVoice = AudioManager.instance.CreateInstance(FMODEvents.instance.WizardVoice);
    }

    private IEnumerator PlayDialog(List<DialogueLines> lines)
    {
        Cursor.lockState = CursorLockMode.Locked;

        maxLineNum = lines.Count;

        if (lineNum >= maxLineNum) yield break; // Ensure we don't go out of bounds

        // Check if there is a next line before proceeding
        while (lineNum < maxLineNum)
        {
            bool isPCSpeaker = lines[lineNum].speaker == "PC";

            if (isPCSpeaker)
            {
                currentSession.ReadPlayerText();
            }
            else
            {
                currentSession.ReadDateText(lines[lineNum].characterSprite);
            }
            yield return StartCoroutine(TypewriteDialog(lines[lineNum].speaker, lines[lineNum].dialog));
            lineNum++;
        }

        //dialogEvent.RemoveAllListeners();
        Cursor.lockState = CursorLockMode.None;
    }

    private IEnumerator TypewriteDialog(string speaker, string dialog)
    {
        float  delay = 0f;
        string hexColor;
        string eventKey = null;

        while (dialog.StartsWith("["))
        {
            int closeIndex = dialog.IndexOf(']');
            if (closeIndex == -1)
                break;

            string tag = dialog.Substring(0, closeIndex + 1);
            dialog = dialog.Substring(closeIndex + 1);

            if (tag.StartsWith("[TOPIC:"))
            {
                hexColor = tag.Replace("[TOPIC:", "").Replace("]", "");

                // Safety checks
                if (!ColorUtility.TryParseHtmlString(hexColor, out _))
                hexColor = "#FFFFFF";

                // Apply formatting
                string topicLabel = $"<b><u><align=center><color={hexColor}>{dialog}</color></align></u></b>";
                dialogText.text += topicLabel;
                break;
            }
            else if (tag.StartsWith("[DELAY:"))
            {
                float.TryParse(
                    tag.Replace("[DELAY:", "").Replace("]", ""),
                    out delay
                );
            }
            else if (tag.StartsWith("[EVENT:"))
            {
                eventKey = tag.Replace("[EVENT:", "").Replace("]", "");
            }
        }

        dialog = dialog.Trim();

        // Apply delay downstream (example)
        yield return new WaitForSeconds(delay);

        if(!string.IsNullOrEmpty(eventKey))
        {
            dialogEvent.Invoke(eventKey);
            eventKey = null;
            Debug.Log("Dialog event invoked.");
        }


        isTypewriting = true;
        skipRequested = false;
        currentSession.isWriting = true;

        // Store the original position of the dateCharacter
        Vector3 originalPosition = currentSession.dateCharacter.transform.localPosition;

        // If the speaker is player, we will set their name to playerName. If for whatever reason the playerName variable is empty or null, we won't set it
        if (speaker == "PC")
        {
            if (playerName != "" && playerName != null)
            {
                speaker = playerName;
            }
        }

        dialog = dialog.Replace("[Player]", playerName);
        dialog = dialog.Replace("[PlayerName]", playerName);
        dialog = dialog.Replace("[Player Name]", playerName);

        // Prepare the speaker's portion in bold (appears immediately)
        string speakerPortion = $"<b>{speaker}:</b> ";

        // Store the current dialog text to preserve history
        string previousText = dialogText.text;

        // Add spacing between previous and new text if there is existing dialog
        if (!string.IsNullOrEmpty(previousText))
        {
            previousText += "\n\n";  // Add space between previous and new lines
        }

        // Set the initial text with the speaker's portion
        string initialText = $"{previousText}{speakerPortion}";
        dialogText.text = initialText;  // Display the speaker portion immediately
        AdjustTextBoxHeight();  // Ensure the text box resizes

        // Track the message as it is being typed
        string currentMessage = "";

        // Typewriter effect: Display the message one letter at a time
        foreach (char letter in dialog.ToCharArray())
        {
            gameManager.UpdateEndTurnButton(false); // Disable end turn button

            if (skipRequested)
            {
                // If skip is requested, instantly complete the message
                currentMessage = dialog;
                break;
            }

            currentMessage += letter;
            dialogText.text = initialText + currentMessage;  // Update dialog with each letter

            AdjustTextBoxHeight();  // Adjust the text box height with each letter
            ScrollToBottom();  // Keep the scroll at the bottom

            yield return new WaitForSeconds(typewriterSpeed);  // Control typing speed
        }

        // Finalize the message and ensure layout updates
        dialogText.text = initialText + currentMessage;
        AdjustTextBoxHeight();  // Ensure the text box is fully adjusted
        ScrollToBottom();  // Keep the scroll at the bottom

        // Add a new line after the conversation ends to distinguish it from the next one
        if (lineNum >= maxLineNum - 1)
        {
            dialogText.text += "\n\n";  // Add spacing to separate conversations
            AdjustTextBoxHeight();
            ScrollToBottom();
        }

        // Reset typewriter and writing state
        isTypewriting = false;
        currentSession.isWriting = false;
        gameManager.UpdateEndTurnButton(true); // Enable end turn button
        currentSession.dateCharacter.transform.localPosition = originalPosition;
    }

    #region Helper Methods
    private Color DarkenColor(Color color, float factor)
    {
        return new Color(
            color.r * factor,
            color.g * factor,
            color.b * factor,
            color.a
        );
    }

    // Method to ensure the Scroll Rect stays at the bottom
    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();  // Ensure the layout is updated
        scrollRect.verticalNormalizedPosition = 0f;  // Scroll to the bottom
    }

    // Adjust the height of the text box based on the content size
    private void AdjustTextBoxHeight()
    {
        // Force the TextMeshProUGUI to update its layout
        dialogText.ForceMeshUpdate();

        // Get the preferred height of the text content
        float newHeight = dialogText.preferredHeight;

        // Apply the new height to the RectTransform
        textRectTransform.sizeDelta = new Vector2(textRectTransform.sizeDelta.x, newHeight);
    }
    #endregion
}

[System.Serializable]
public class DialogueLines
{
    public string speaker;          // speakers corresponding to each line
    [TextArea(3, 10)]
    public string dialog;           // main dialogue lines
    public Sprite characterSprite;  // character sprite for the dialogue line
}

