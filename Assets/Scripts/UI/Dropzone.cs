using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dropzone : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI scoreCalculationText;
    [SerializeField] private TextMeshProUGUI dialogText;

    [SerializeField] private List<Card> playedCards;
    [SerializeField] private int maxCards = 4;
    [SerializeField] public List<Card> cardsToScore = new List<Card>(); // Cards to be scored this turn
    [SerializeField] private Card lastPlacedCard; // Most recently placed card

    [SerializeField] private DiscardPile discard;
    [SerializeField] private PlayerArea playerArea;
    [SerializeField] private TopicContainer topicContainer;
    [SerializeField] private Playtest currentSession;
    [SerializeField] public ConvoTopic selectedConvoTopic;
    [SerializeField] private GameManager gameManager;
    [SerializeField] ReserveManager reserveManager;

    // Array to hold the dropzone
    [SerializeField] private DropzoneSlot dropzone; 

    public Dictionary<string, int> multipyboost = new()
    {
        {"Cha", 1},
        {"Cou", 1},
        {"Cle", 1},
        {"Cre", 1}
    };
    public Dictionary<string, int> addboost = new()
    {
        {"Cha", 0},
        {"Cou", 0},
        {"Cle", 0},
        {"Cre", 0}
    };

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
    private float typewriterSpeed = 0.025f;  // Speed of the typewriter effect

    // Flag to indicate if the typewriter effect is running
    public bool isTypewriting = false;
    private bool skipRequested = false;

    // Coroutine reference for CountDownPower
    private int targetPowerNum;
    public bool isCountingDown = false;

    [SerializeField] private float discardDuration = 1.0f; // Duration of the discard animation

    // Public getters for various properties
    public List<Card> GetPlayedCards() => playedCards;
    public List<Card> GetCardsToScore() => cardsToScore;
    public int GetMaxCards() => maxCards;

    [SerializeField] private ScrollRect scrollRect; // Reference to the Scroll Rect and Content Transform
    [SerializeField] private RectTransform textRectTransform;  // RectTransform of the TextMeshProUGUI

    private Player playerManager;
    private string playerName;

    private bool addExtraLine;

    private bool halfwayPointDone = false;
    private int addCha;
    private int addCou;
    private int addCle;
    private int addCre;
    private int multCha;
    private int multCou;
    private int multCle;
    private int multCre;

    // Start is called before the first frame update
    void Start()
    {
        // Set the playerManager and get the player's preferred name
        if (FindObjectsOfType<Player>() != null)
        {
            playerManager = FindObjectOfType<Player>();
            playerName = playerManager.GetName();
        }
        else
        {
            Debug.LogWarning("Player Manager was null!");
        }
    }

    private void Update()
    {
        // Allow only skip input when dialog is playing or when power is counting down
        if ((isTypewriting && Input.GetButtonDown("Skip")) || (isCountingDown && Input.GetButtonDown("Skip")))
        {
            skipRequested = true;
        }

        addCha = addboost["Cha"];
        addCou = addboost["Cou"];
        addCle = addboost["Cle"];
        addCre = addboost["Cre"];

        multCha = multipyboost["Cha"];
        multCou = multipyboost["Cou"];
        multCle = multipyboost["Cle"];
        multCre = multipyboost["Cre"];
    }

    // Adds a card to the played cards list and removes it from the player's area
    public void AddCardToDropzone(Card card)
    {
        if (card == null) return;

        // Check placement rules (using CanPlaceCard method or similar logic)
        if (CanPlaceCard(card))
        {
            // Add card to the dropzone
            dropzone.AddCard(card); // Add to the internal list
            cardsToScore.Add(card); // Add to the scoring list
            card.transform.SetParent(dropzone.transform, false); // Parent it to the DropzoneSlot

            // Disable collider to prevent interactions while in dropzone
            card.GetComponent<Collider2D>().enabled = false;

            // Update the card's position based on its stacking order
            int index = dropzone.GetCards().Count - 1; // Get current index in the stack
            card.transform.localPosition = new Vector3(0, index * 0.2f, 0); // Slightly staggered stacking
            card.transform.localScale = Vector3.one; // Reset scale
            card.transform.rotation = Quaternion.identity; // Reset rotation

            // Update the last placed card
            lastPlacedCard = card;

            //Debug.Log($"Card {card.name} placed in DropzoneSlot.");
            CalculateScore();
        }
        else
        {
            Debug.LogError("Cannot place card: Attributes do not match.");
        }
    }

    // Method to handle special "Str" type cards separately for modularity
    private void HandleStressCard()
    {
        StressManager.instance?.RemoveFromCurrentStress(0.1f);
    }

    // Removes a card from the dropzone and adds it to the player's area
    public void RemoveCardFromDropzone()
    {
        Card topCard = dropzone.TopCard; // Get the current top card
        if (topCard != null)
        {
            // Remove the top card from the dropzone and add it back to the player's hand (if not a reserve card)
            if (!topCard.isReserveCard)
            {
                playerArea.AddCards(topCard);
                topCard.transform.SetParent(playerArea.transform, false); // Reset card hierarchy visually
            }
            cardsToScore.Remove(topCard);
            dropzone.RemoveTopCard();

            // Update the last placed card to the new top card
            lastPlacedCard = dropzone.TopCard;
            GameManager.instance.ComboSurge --;

            // Reset UI or other visual elements if necessary
            CalculateScore();
        }
    }

    public void ReturnCards()
    {
        List<Card> cards = dropzone.GetCards(); // Get all cards in the dropzone
        Card bottomCard = null;
        foreach (Card card in cards)
        {
            if (!card.isBottomCard && !card.isPlayed)
            {
                if (!card.isReserveCard)
                {
                    // Add each card back to the player's hand
                    playerArea.AddCards(card);
                    card.transform.SetParent(playerArea.transform, false); // Reset card hierarchy visually
                }
                else
                {
                    card.transform.SetParent(reserveManager.currentOpenSlot.transform, false); // Set as a child of the current open Reserve Slot in hierarchy

                    reserveManager.CardReturned(card);
                }
                card.isInDropzone = false;
            }
            else
            {
                bottomCard = card;
            }
        }

        // Clear the dropzone
        dropzone.ClearAllCards();

        // Reset last placed card
        lastPlacedCard = bottomCard;

        // Clear the cards to score
        cardsToScore.Clear();
        //Debug.Log(cardsToScore.Count);

        CalculateScore();
    }

    // Scores the played cards, moves them to the discard pile, and updates the UI
    public void ScoreCards()
    {
        gameManager.UpdateEndTurnButton(false); // Disables end turn button
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
            initialPower = selectedConvoTopic.TierPower;   // Store the initial power of the topic
            selectedConvoTopic.isLocked = true;           // Lock the topic to prevent changes during scoring
        }

        // Recalculate the current score based on cards in dropzones
        CalculateScore(true);

        // Move all cards to the discard pile asynchronously
        //Currently removed for obsolescence
        // StartCoroutine(DiscardCards());

        // Calculate the new power level after subtracting the score
        int startPowerNum = selectedConvoTopic.TierPower;
        int targetPowerNum = selectedConvoTopic.TierPower - score;
        selectedConvoTopic.tierPower = targetPowerNum;  // Update the power level of the topic

        // Start the countdown of the power value (smooth UI animation)
        StartCoroutine(CountDownPower(startPowerNum, targetPowerNum));

        // If the topic's power is depleted, complete the conversation topic
        if (selectedConvoTopic.TierPower <= 0)
        {
            CompleteConvo();
        }

        if (!completedConvo && !failedConvo)
        {
            // Check if dialog triggers are needed based on the new power state
            CheckDialogTriggers();
        }

        // Reset the state of the dropzones after scoring
        ResetAfterScoring();
    }

    public void ResetForNewTurn()
    {
        // Remove the DragDrop script from all of the cards that were scored
        foreach (Card card in cardsToScore)
        {
            Destroy(card.GetComponent<DragDrop>());
        }

        // Clear cardsToScore for a fresh scoring list
        cardsToScore.Clear();
        // Maintain cards in dropzone but reset lastPlacedCard
        lastPlacedCard = dropzone.TopCard;
        lastPlacedCard.isBottomCard = true;

        foreach (Card card in dropzone.GetCards())
        {
            card.isPlayed = true;
            
            // On the off chance DragDrop is not destroyed, we make it so that it can't be dragged anyway
            if (card.GetComponent<DragDrop>() != null)
            {
                card.GetComponent<DragDrop>().isDraggable = false;
            }

            if (card.isReserveCard)
            {
                card.isReserveCard = false;
            }

            if (card != dropzone.TopCard)
            {
                card.transform.gameObject.SetActive(false);
            }
        }
    }

    // Method to calculate score (call anytime score may be changed)
    // EndOfTurnTally should only be set when calculating the score at the end of the turn,
    // for effects that only accumulate at the end of the turn
    public void CalculateScore(bool endOfTurnTally = false)
    {
        // Reset score to recalculate
        score = 0;

        // Calculate total power and find the highest power
        int totalPower = 0;
        int highestPower = 0;

        // String to hold the calculation breakdown
        string calculationBreakdown = "";

        if (cardsToScore.Count != 0 && gameManager.IsTopicSelected)
        {
            // Iterate through all played cards to calculate the score
            foreach (Card card in cardsToScore)
            {
                // If this card is a stress card, reduce stress instead of scoring
                if (endOfTurnTally && card.GetType() == typeof(StressCard))
                {
                    HandleStressCard();
                    continue;
                }
                int boostedPower = (card.Power + addboost[card.Type]) * 
                                    multipyboost[card.Type];

                totalPower += boostedPower;
                if (boostedPower > highestPower)
                {
                    highestPower = boostedPower;
                }
            }

            // Apply scoring formula: (sum of powers) * (highest power value)
            score = totalPower * highestPower;

            // Update the current score text field with the recalculated score
            currentScoreText.text = "Current Score: " + score.ToString();

            // Construct the breakdown string
            calculationBreakdown = $"Total Power: {totalPower}\nHighest Power: {highestPower}\nScore Calculation: {totalPower} * {highestPower} = {score}";
        }
        else
        {
            // Set score text to zero if no cards are played or no topic is selected
            currentScoreText.text = "Current Score: " + score.ToString();
            calculationBreakdown = "No cards played or no topic selected.\nScore: 0";
        }

        // Update the TextMeshProUGUI object with the calculation breakdown
        scoreCalculationText.text = calculationBreakdown;
    }

    // Checks if the appropriate dialogs should be triggered based on the topic's power state.
    private void CheckDialogTriggers()
    {
        // Check each threshold and enqueue dialogs if necessary
        if (!dialogPlayedAtFullPower && selectedConvoTopic.TierPower <= initialPower - 1)
        {
            dialogPlayedAtFullPower = true;
            dialogQueue.Enqueue(PlayDialog());  // Enqueue dialog for full power
        }

        if (!dialogPlayedAtHalfPower && selectedConvoTopic.TierPower <= initialPower / 2)
        {
            Player.instance.SetConvoTiers(selectedConvoTopic.ConvoAttribute, 2);
            dialogPlayedAtHalfPower = true;
            dialogQueue.Enqueue(PlayDialog());  // Enqueue dialog for half power
        }

        if (!dialogPlayedAtZeroPower && selectedConvoTopic.TierPower <= 0)
        {
            Player.instance.SetConvoTiers(selectedConvoTopic.ConvoAttribute, 3);
            dialogPlayedAtZeroPower = true;
            dialogQueue.Enqueue(PlayDialog());  // Enqueue dialog for zero power
            if (addExtraLine)
            {
                dialogQueue.Enqueue(PlayDialog());
            }
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
        increaseConnection(0);
        CheckDialogTriggers();

        // Move the topic to the "done" list and remove it from active topics
        topicContainer.doneConvos.Add(selectedConvoTopic);
        topicContainer.convoTopics.Remove(selectedConvoTopic);

        // Reset the convoText
        selectedConvoTopic.convoText.text = "Awaiting Topic...";

        // Reset topic selection state for the next round
        selectedConvoTopic.isClicked = false;
        gameManager.IsTopicSelected = false;

        // Re-enable topic buttons for the next selection
        topicContainer.EnableButtons();
    }

    // Clears all cards from dropzones and resets the current score to 0.
    private void ResetAfterScoring()
    {
        score = 0;  // Reset score to 0
        CalculateScore();
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
                sprites = currentSession.chaSprites;
                speakers = currentSession.chaSpeaker;
                addExtraLine = currentSession.addExtraType4Line;
                break;
            case "cou":
            case "courage":
                lines = currentSession.couLines;
                sprites = currentSession.couSprites;
                speakers = currentSession.couSpeaker;
                addExtraLine = currentSession.addExtraType1Line;
                break;
            case "cle":
            case "cleverness":
                lines = currentSession.cleLines;
                sprites = currentSession.cleSprites;
                speakers = currentSession.cleSpeaker;
                addExtraLine = currentSession.addExtraType3Line;
                break;
            case "cre":
            case "creativity":
                lines = currentSession.creLines;
                sprites = currentSession.creSprites;
                speakers = currentSession.creSpeaker;
                addExtraLine = currentSession.addExtraType2Line;
                break;
            default:
                Debug.LogWarning("Unknown conversation attribute: " + selectedConvoTopic.ConvoAttribute);
                yield break;
        }

        maxLineNum = lines.Count;

        if (lineNum >= maxLineNum) yield break; // Ensure we don't go out of bounds

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
        yield return StartCoroutine(TypewriteDialog(speakers[lineNum], lines[lineNum]));
        lineNum++;

        // Check if there is a next line before proceeding
        if (lineNum < maxLineNum)
        {
            isPCSpeaker = speakers[lineNum] == "PC";

            if (isPCSpeaker)
            {
                currentSession.ReadPlayerText();
            }
            else
            {
                currentSession.ReadDateText(sprites[lineNum]);
            }
            yield return StartCoroutine(TypewriteDialog(speakers[lineNum], lines[lineNum]));
            lineNum++;
        }

        Cursor.lockState = CursorLockMode.None;
    }

    private IEnumerator TypewriteDialog(string speaker, string message)
    {
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

        message = message.Replace("[Player]", playerName);
        message = message.Replace("[PlayerName]", playerName);
        message = message.Replace("[Player Name]", playerName);

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
        dialogText.text = initialText;      // Display the speaker portion immediately
        AdjustTextBoxHeight();              // Ensure the text box resizes

        // Track the message as it is being typed
        string currentMessage = "";

        // Typewriter effect: Display the message one letter at a time
        foreach (char letter in message.ToCharArray())
        {
            gameManager.UpdateEndTurnButton(false); // Disable end turn button

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
        gameManager.UpdateEndTurnButton(true); // Enable end turn button
        currentSession.dateCharacter.transform.localPosition = originalPosition;
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
        if (!isTypewriting)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        isCountingDown = true;
        skipRequested = false;
        while (startValue > endValue)
        {
            gameManager.UpdateEndTurnButton(false); // Disable end turn button
            if (skipRequested)
            {
                startValue = endValue; // Instantly set to final value
                break;
            }

            startValue--;  // Decrement the power by 1
            selectedConvoTopic.numText.text = startValue.ToString();  // Update the UI
            yield return new WaitForSeconds(0.05f);  // Small delay for the countdown effect
        }

        // Ensure the final value is set correctly
        selectedConvoTopic.tierPower = endValue;
        selectedConvoTopic.numText.text = endValue.ToString();

        if (selectedConvoTopic.TierPower <= 0)
        {
            selectedConvoTopic.numText.text = ""; // Hide the num text
            selectedConvoTopic.finishedText.SetActive(true); // Show the finished text
            selectedConvoTopic.background.color = new Color(0.68f, 0.85f, 0.90f, 1); // Pastel blue
            selectedConvoTopic.isLocked = false;
            selectedConvoTopic.ToggleClick(true);
            gameManager.ResetConvoTopic();
            if (topicContainer.doneConvos.Count == 2 && !halfwayPointDone)
            {
                gameManager.EndGameHalfWin();
            }
        }

        isCountingDown = false;
        gameManager.UpdateEndTurnButton(true); // Enable end turn button

        if (!isTypewriting)
        {
            Cursor.lockState = CursorLockMode.None;
        }

        currentScoreText.text = "Current Score: " + 0;
        scoreCalculationText.text = "No cards played or no topic selected.\nScore: 0";
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

    public void ApplyBonus(string type, int value, string operation)
    {
        switch(operation)
        {
            case "+":
                addboost[type] = Math.Clamp(addboost[type] + value, 0, 999);
                break;
            case "x":
                multipyboost[type] = Math.Clamp(multipyboost[type] * value, 1, 999);
                break;
            case "-":
                addboost[type] = Math.Clamp(addboost[type] - value, 0, 999);
                break;
            case "/":
                multipyboost[type] = Math.Clamp(multipyboost[type] / value, 1, 999);
                break;
            // default:
            //     Debug.LogWarning("Unknown operation: " + operation);
            //     break;
        }
        CalculateScore();
    }

    // Checks if all dropzones are empty.
    // Currently removed for obsolescence
    /*
    private bool AllDropzonesEmpty()
    {
        foreach (var dropzone in dropzones)
        {
            if (!dropzone.IsEmpty) return false;
        }
        return true;
    }
    */

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
        // Ensure the dropzone is properly managed
        if (dropzone.TopCard == null && dropzone.GetCards().Count > 0)
        {
            dropzone.ClearAllCards(); // Clear invalid state
        }
    }

    public bool CanPlaceCard(Card card)
    {
        // If the dropzone wants to prevent a card from being placed, that logic
        // should go here, not in Card.CanPlaceOnCard()
        return card.CanPlaceOnCard(lastPlacedCard);
    }

    public DropzoneSlot GetDropzone()
    {
        return dropzone;
    }

    public void increaseConnection(int index)
    {
        ConnectionManager.instance.increaseConnection(index, 1);
    }

    internal void ResetBoosts()
    {
        addboost = new()
        {
            {"Cha", 0},
            {"Cou", 0},
            {"Cle", 0},
            {"Cre", 0}
        };

        multipyboost = new()
        {
            {"Cha", 1},
            {"Cou", 1},
            {"Cle", 1},
            {"Cre", 1}
        };
    }
}