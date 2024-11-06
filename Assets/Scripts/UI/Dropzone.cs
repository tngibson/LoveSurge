using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dropzone : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI dialogText;

    [SerializeField] private List<Card> playedCards;
    [SerializeField] private int maxCards = 4;

    [SerializeField] private DiscardPile discard;
    [SerializeField] private PlayerArea playerArea;
    [SerializeField] private TopicContainer topicContainer;
    [SerializeField] private Playtest currentSession;
    [SerializeField] public ConvoTopic selectedConvoTopic;
    [SerializeField] private GameManager gameManager;

    // Array to hold four individual dropzones, each capable of storing one card
    [SerializeField] private DropzoneSlot[] dropzones = new DropzoneSlot[4];

    private int score = 0;
    private int lineNum = 0;
    private int maxLineNum;
    private int initialPower;
    private bool dialogPlayedAtFullPower = false;
    private bool dialogPlayedAtHalfPower = false;
    private bool dialogPlayedAtZeroPower = false;

    // Queue to manage multiple dialogs for sequential playback
    private Queue<IEnumerator> dialogQueue = new Queue<IEnumerator>();

    private bool completedConvo = false;
    private bool failedConvo = false;

    // Typewriter settings
    [SerializeField] private float typewriterSpeed = 0.05f;  // Speed of the typewriter effect

    // Flag to indicate if the typewriter effect is running
    public bool isTypewriting = false;
    private bool skipRequested = false;

    // Coroutine reference for CountDownPower
    private int targetPowerNum;

    [SerializeField] private float discardDuration = 1.0f; // Duration of the discard animation

    // Public getters for various properties
    public List<Card> GetPlayedCards() => playedCards;
    public int GetMaxCards() => maxCards;

    [SerializeField] private ScrollRect scrollRect; // Reference to the Scroll Rect and Content Transform
    [SerializeField] private RectTransform textRectTransform;  // RectTransform of the TextMeshProUGUI

    private Player playerManager;
    private string playerName;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        int maxSlots = dropzones.Length;  // Assuming dropzones is an array of DropzoneSlot components
        playedCards = new List<Card>(new Card[maxSlots]); // Initializes with nulls up to maxSlots
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize all dropzones and link them to this Dropzone manager
        foreach (var dropzone in dropzones)
        {
            dropzone.Initialize(this);
        }

        // Set the playerManager and get the player's preferred name
        if (GameObject.Find("PlayerManager") != null)
        {
            playerManager = GameObject.Find("PlayerManager").GetComponent<Player>();
            playerName = playerManager.GetName();
        }
    }

    private void Update()
    {
        // Allow only skip input when dialog is playing
        if (isTypewriting && Input.GetButtonDown("Skip"))
        {
            skipRequested = true;
        }
    }

    // Adds a card to the played cards list and removes it from the player's area
    public void AddCardToDropzone(Card card, int dropzoneIndex)
    {
        if (card != null && dropzones[dropzoneIndex].IsEmpty && card.Type != "Str")
        {
            dropzones[dropzoneIndex].SetCard(card);  // Place card in the dropzone

            // Directly place the card at the specified dropzoneIndex in playedCards
            playedCards[dropzoneIndex] = card;

            // Remove card from player's area
            playerArea.RemoveCards(card);

            // Update UI and calculate score
            gameManager.UpdateEndTurnButton(true);
            CalculateScore();
        }
        else if (card != null && card.Type == "Str")
        {
            HandleStressCard(card);
        }
        else
        {
            Debug.LogError("There is no card to add.");
        }
    }

    // Method to handle special "Str" type cards separately for modularity
    private void HandleStressCard(Card card)
    {
        if (StressManager.instance.currentStressAmt > 0)
        {
            StressManager.instance.removeFromCurrentStress(card.Power * 0.1f);
            StressBar.instance.updateStressBar();
        }
        else
        {
            Debug.Log("Cannot add stress card when stress is at 0.");
        }
    }

    // Removes a card from the dropzone and adds it to the player's area
    public void RemoveCardFromDropzone(int dropzoneIndex)
    {
        Card card = dropzones[dropzoneIndex].GetCard();
        if (card != null)
        {
            // Return card to player area
            playerArea.AddCards(card);

            // Clear the dropzone slot
            dropzones[dropzoneIndex].ClearCard();

            // Set the corresponding index in playedCards to null instead of removing
            playedCards[dropzoneIndex] = null;

            // Check if all dropzones are empty to update the end turn button state
            if (AllDropzonesEmpty())
            {
                gameManager.UpdateEndTurnButton(false);  // Disable end turn button if no cards are placed
            }

            CalculateScore();  // Recalculate the score
        }
    }

    public void ReturnCards()
    {
        for (int i = 0; i < dropzones.Length; i++)
        {
            Card card = dropzones[i].GetCard();
            if (card != null)
            {
                // Add the card back to the player's hand
                playerArea.AddCards(card);

                // Reset the card's parent to the player area to maintain correct hierarchy
                card.transform.SetParent(playerArea.transform, false);

                // Clear the dropzone slot
                dropzones[i].ClearCard();

                // Set the corresponding playedCards slot to null instead of removing it
                playedCards[i] = null;
            }
        }

        // Reset the score since no cards are left in the dropzones
        CalculateScore();
    }

    // Scores the played cards, moves them to the discard pile, and updates the UI
    public void ScoreCards()
    {
        if (failedConvo)
        {
            dialogText.text += "\n\n";
        }

        if (completedConvo || failedConvo || !dialogPlayedAtFullPower)
        {
            completedConvo = false;
            failedConvo = false;
            ResetDialogFlags();
            lineNum = 0;
            initialPower = selectedConvoTopic.PowerNum;   // Store the initial power of the topic
            selectedConvoTopic.isLocked = true;           // Lock the topic to prevent changes during scoring
        }

        // Recalculate the current score based on cards in dropzones
        CalculateScore();

        // Move all cards to the discard pile asynchronously
        StartCoroutine(DiscardCards());

        // Calculate the new power level after subtracting the score
        int targetPowerNum = selectedConvoTopic.PowerNum - score;
        selectedConvoTopic.PowerNum = targetPowerNum;  // Update the power level of the topic

        // Start the countdown of the power value (smooth UI animation)
        StartCoroutine(CountDownPower(initialPower, targetPowerNum));

        // If the topic's power is depleted, complete the conversation topic
        if (selectedConvoTopic.PowerNum <= 0)
        {
            CompleteConvo();
        }
        // If the maximum turn count is reached without depleting power, the topic fails
        else if (gameManager.turnCount >= gameManager.maxTurnCount)
        {
            FailConvo();
        }

        if (!completedConvo && !failedConvo)
        {
            // Check if dialog triggers are needed based on the new power state
            CheckDialogTriggers();
        }

        // Reset the state of the dropzones after scoring
        ResetAfterScoring();
    }

    // Method to calculate score (call anytime score may be changed)
    public void CalculateScore()
    {
        // Reset score to recalculate
        score = 0;

        if (playedCards.Count != 0 && gameManager.IsTopicSelected)
        {
            // Iterate through all played cards to calculate the score
            for (int i = 0; i < playedCards.Count; i++)
            {
                Card card1 = playedCards[i];

                // Skip null entries in the playedCards list
                if (card1 == null) continue;

                // Add the card's power to the score
                score += card1.Power;

                // If there's a next card, apply bonuses based on matching attributes
                if (i + 1 < playedCards.Count)
                {
                    Card card2 = playedCards[i + 1];

                    // Skip null comparisons
                    if (card2 != null)
                    {
                        // Bonus points for matching types or similar power levels
                        if (card1.Type == card2.Type) { score++; }
                        if (card1.Power == card2.Power) { score++; }
                        if (card1.Power == card2.Power - 1) { score++; }
                    }
                }

                // Bonus points for matching the selected conversation topic
                if (card1.Type == selectedConvoTopic.ConvoAttribute.Substring(0, 3)) { score++; }
            }

            // Update the current score text field with the recalculated score
            currentScoreText.text = "Current Score: " + score.ToString();
        }
        else
        {
            // Set score text to zero if no cards are played or no topic is selected
            currentScoreText.text = "Current Score: " + score.ToString();
        }
    }

    // Coroutine that moves all cards from the dropzones to the discard pile at once.
    private IEnumerator DiscardCards()
    {
        List<Coroutine> discardCoroutines = new List<Coroutine>();

        for (int i = 0; i < dropzones.Length; i++)
        {
            Card card = dropzones[i].GetCard();
            if (card != null)
            {
                // Start discarding each card without waiting for completion
                Coroutine discardCoroutine = StartCoroutine(MoveCardToDiscardPile(card));
                discardCoroutines.Add(discardCoroutine);

                // Clear each dropzone and set playedCards[i] to null instead of removing
                dropzones[i].ClearCard();
                playedCards[i] = null;  // Set the corresponding slot in playedCards to null
                discard.AddToDiscard(card);  // Add card to discard pile
            }
        }

        // Wait until all discard coroutines are complete
        foreach (Coroutine coroutine in discardCoroutines)
        {
            yield return coroutine;
        }
    }

    // Modified to support simultaneous movement
    private IEnumerator MoveCardToDiscardPile(Card card)
    {
        Vector3 startPosition = card.transform.position;
        Vector3 discardPosition = discard.transform.position;

        Vector3 startScale = card.transform.localScale;
        Vector3 targetScale = Vector3.zero;

        float duration = discardDuration;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float progress = elapsedTime / duration;

            card.transform.position = Vector3.Lerp(startPosition, discardPosition, Mathf.SmoothStep(0, 1, progress));
            card.transform.localScale = Vector3.Lerp(startScale, targetScale, progress);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        card.transform.position = discardPosition;
        card.transform.localScale = targetScale;
        card.transform.SetParent(discard.transform, true);
    }



    // Checks if the appropriate dialogs should be triggered based on the topic's power state.
    private void CheckDialogTriggers()
    {
        // Check each threshold and enqueue dialogs if necessary
        if (!dialogPlayedAtFullPower)
        {
            dialogPlayedAtFullPower = true;
            dialogQueue.Enqueue(PlayDialog());  // Enqueue dialog for full power
        }

        if (!dialogPlayedAtHalfPower && selectedConvoTopic.PowerNum <= initialPower / 2)
        {
            dialogPlayedAtHalfPower = true;
            dialogQueue.Enqueue(PlayDialog());  // Enqueue dialog for half power
        }

        if (!dialogPlayedAtZeroPower && selectedConvoTopic.PowerNum <= 0)
        {
            dialogPlayedAtZeroPower = true;
            dialogQueue.Enqueue(PlayDialog());  // Enqueue dialog for zero power
        }

        // Play all queued dialogs sequentially
        if (dialogQueue.Count > 0)
        {
            StartCoroutine(PlayQueuedDialogs());
        }
    }

    // Handles the completion of a conversation topic by moving it to the "done" list and resetting relevant states for the next round.
    private void CompleteConvo()
    {
        completedConvo = true;

        CheckDialogTriggers();

        // Move the topic to the "done" list and remove it from active topics
        topicContainer.doneConvos.Add(selectedConvoTopic);
        topicContainer.convoTopics.Remove(selectedConvoTopic);

        // Reset the convoText
        selectedConvoTopic.convoText.text = "Awaiting Topic...";

        // Reset topic selection state for the next round
        selectedConvoTopic.isClicked = false;
        gameManager.IsTopicSelected = false;
        gameManager.turnCount = 0;

        // Re-enable topic buttons for the next selection
        topicContainer.EnableButtons();
    }

    // Handles the failure of a conversation topic by moving it to the "failed" list and resetting relevant states for the next round.
    private void FailConvo()
    {
        failedConvo = true;

        CheckDialogTriggers();

        // Move the topic to the "failed" list and remove it from active topics
        topicContainer.failedConvos.Add(selectedConvoTopic);
        topicContainer.convoTopics.Remove(selectedConvoTopic);

        // Reset the convoText
        selectedConvoTopic.convoText.text = "Awaiting Topic...";

        // Mark the topic as failed and reset its state
        selectedConvoTopic.isFailed = true;
        selectedConvoTopic.isClicked = false;

        // Reset topic selection state
        gameManager.IsTopicSelected = false;

        // Re-enable topic buttons for the next selection
        topicContainer.EnableButtons();
    }

    // Clears all cards from dropzones and resets the current score to 0.
    private void ResetAfterScoring()
    {
        score = 0;  // Reset score to 0
        currentScoreText.text = "Current Score: 0";  // Update UI
    }

    // Coroutine to play the dialog 
    private IEnumerator PlayDialog()
    {
        Cursor.lockState = CursorLockMode.Locked;

        List<string> lines;
        List<Sprite> sprites;
        List<string> speakers;

        // Depending on the selected conversation topic, we choose the relevant lines and speakers
        switch (selectedConvoTopic.ConvoAttribute.ToLower())
        {
            case "cha":
            case "charisma":
                lines = currentSession.chaLines;
                maxLineNum = lines.Count;
                sprites = currentSession.chaSprites;
                speakers = currentSession.chaSpeaker;
                break;
            case "cou":
            case "courage":
                lines = currentSession.couLines;
                maxLineNum = lines.Count;
                sprites = currentSession.couSprites;
                speakers = currentSession.couSpeaker;
                break;
            case "cle":
            case "cleverness":
                lines = currentSession.cleLines;
                maxLineNum = lines.Count;
                sprites = currentSession.cleSprites;
                speakers = currentSession.cleSpeaker;
                break;
            case "cre":
            case "creativity":
                lines = currentSession.creLines;
                maxLineNum = lines.Count;
                sprites = currentSession.creSprites;
                speakers = currentSession.creSpeaker;
                break;
            default:
                Debug.LogWarning("Unknown conversation attribute: " + selectedConvoTopic.ConvoAttribute);
                lines = null;
                maxLineNum = lines.Count;
                sprites = null;
                speakers = null;
                break;
        }

        bool isPCSpeaker = speakers[lineNum] == "PC";

        // Read and typewrite the first speaker's line
        if (isPCSpeaker)
        {
            currentSession.ReadPlayerText(); // Call the method for PC speaker
        }
        else
        {
            currentSession.ReadDateText(sprites[lineNum]); // Call the method for Date speaker
        }
        // Read and typewrite the first speaker's line
        yield return StartCoroutine(TypewriteDialog(speakers[lineNum], lines[lineNum]));
        lineNum++;

        // Read and typewrite the second speaker's line
        if (isPCSpeaker)
        {
            currentSession.ReadDateText(sprites[lineNum]); // Call the method for Date speaker
        }
        else
        {
            currentSession.ReadPlayerText(); // Call the method for PC speaker
        }
        // Read and typewrite the second speaker's line
        yield return StartCoroutine(TypewriteDialog(speakers[lineNum], lines[lineNum]));
        lineNum++;

        Cursor.lockState = CursorLockMode.None;
    }

    private IEnumerator TypewriteDialog(string speaker, string message)
    {
        isTypewriting = true;
        skipRequested = false;
        currentSession.isWriting = true;

        // If the speaker is player, we will set their name to playerName. If for whatever reason the playerName variable is empty or null, we won't set it
        if (speaker == "PC")
        {
            if (playerName != "" && playerName != null)
            {
                speaker = playerName;
            }
        }

        message = message.Replace("[Player]", playerName);

        // Prepare the speaker's portion in bold (appears immediately)
        string speakerPortion = $"<b>{speaker}:</b> ";

        // Check if it's the first line of the conversation
        if (lineNum == 0)
        {
            Color topicColor = selectedConvoTopic.topicColor;  // topic color of the selected convo topic
            Color darkenedColor = DarkenColor(topicColor, 0.75f);  // Darken given color
            string hexColor = ColorUtility.ToHtmlStringRGB(darkenedColor);  // Convert to the color to a hex string

            // Set the convo topic label for the current topic convo
            string topicLabel = $"<b><u><align=center><color=#{hexColor}>{selectedConvoTopic.topicLabelText.text}</color></align></u></b>";
            dialogText.text += topicLabel;  // Display the topic label
        }

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
        foreach (char letter in message.ToCharArray())
        {
            if (skipRequested)
            {
                // If skip is requested, instantly complete the message
                currentMessage = message;
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
    }

    // Helper method to darken the convo topic color
    private Color DarkenColor(Color color, float factor)
    {
        return new Color(
            color.r * factor,
            color.g * factor,
            color.b * factor,
            color.a 
        );
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

    // Method to ensure the Scroll Rect stays at the bottom
    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();  // Ensure the layout is updated
        scrollRect.verticalNormalizedPosition = 0f;  // Scroll to the bottom
    }

    // Coroutine to count down PowerNum smoothly
    private IEnumerator CountDownPower(int startValue, int endValue)
    {
        while (startValue > endValue)
        {
            startValue--;  // Decrement the power by 1
            selectedConvoTopic.numText.text = startValue.ToString();  // Update the UI
            yield return new WaitForSeconds(0.05f);  // Small delay for the countdown effect
        }

        // Ensure the final value is set correctly
        selectedConvoTopic.PowerNum = endValue;
        selectedConvoTopic.numText.text = endValue.ToString();

        if (selectedConvoTopic.PowerNum <= 0)
        {
            selectedConvoTopic.numText.text = ""; // Hide the num text
            selectedConvoTopic.finishedText.SetActive(true); // Show the finished text
            selectedConvoTopic.background.color = new Color(0.68f, 0.85f, 0.90f, 1); // Pastel blue
            selectedConvoTopic.isLocked = false;
            selectedConvoTopic.ToggleClick(true);
            gameManager.ResetConvoTopic();
        }
        else if (gameManager.turnCount >= gameManager.maxTurnCount && selectedConvoTopic.PowerNum > 0)
        {
            selectedConvoTopic.numText.text = ""; // Hide the num text
            selectedConvoTopic.bustedText.SetActive(true); // Show the busted text
            selectedConvoTopic.background.color = new Color(1f, 0.5f, 0.5f, 1f); // Pastel red
            selectedConvoTopic.isLocked = false;
            selectedConvoTopic.ToggleClick(true);
            gameManager.turnCount = 0;
            gameManager.ResetConvoTopic();
        }
    }

    // Swaps two cards in the played cards list by index
    public void SwapCards(int cardIndex1, int cardIndex2)
    {
        Card temp = playedCards[cardIndex1];
        playedCards[cardIndex1] = playedCards[cardIndex2];
        playedCards[cardIndex2] = temp;
        CalculateScore();
    }

    public bool HasCard(Card card)
    {
        return playedCards.Contains(card);
    }

    public int GetCardIndex(Card card)
    {
        // Use IndexOf to find the index of the specified card in the playedCards list
        int index = playedCards.IndexOf(card);

        // Optional: Check if the card was found
        if (index == -1)
        {
            Debug.LogWarning("Card not found in playedCards list.");
        }

        return index;
    }

    // Checks if all dropzones are empty.
    private bool AllDropzonesEmpty()
    {
        foreach (var dropzone in dropzones)
        {
            if (!dropzone.IsEmpty) return false;
        }
        return true;
    }

    // Coroutine to play all dialogs in the queue sequentially
    private IEnumerator PlayQueuedDialogs()
    {
        while (dialogQueue.Count > 0)
        {
            yield return StartCoroutine(dialogQueue.Dequeue());
        }

        // Clears the queue for the dialog
        dialogQueue.Clear();
    }

    // Clears the dialog queue and resets all dialog flags
    public void ResetDialogFlags()
    {
        dialogQueue.Clear();
        dialogPlayedAtZeroPower = false;
        dialogPlayedAtHalfPower = false;
        dialogPlayedAtFullPower = false;
    }

    public void ValidateDropzoneState()
    {
        foreach (var slot in dropzones) 
        {
            if (slot.transform.childCount == 0)
                slot.ClearCard();  // Clear the card if there is no child object
        }
    }
}