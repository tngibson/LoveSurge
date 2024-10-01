using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class Dropzone : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private TextMeshProUGUI playerText;
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private List<Card> playedCards;
    [SerializeField] private DiscardPile discard;
    [SerializeField] private PlayerArea playerArea;
    [SerializeField] private TopicContainer topicContainer;
    [SerializeField] private Playtest currentSession;
    [SerializeField] public ConvoTopic selectedConvoTopic;
    [SerializeField] private GameManager gameManager;

    /*
    private StreamReader playerReader;
    private StreamReader dateReader;
    */ 

    private int score = 0;
    private int totalScore = 0;
    private int lineNum = 0;
    private int initialPower;
    private bool dialogPlayedAtFullPower = false;
    private bool dialogPlayedAtHalfPower = false;
    private bool dialogPlayedAtZeroPower = false;

    // Typewriter settings
    [SerializeField] private float typewriterSpeed = 0.05f;  // Speed of the typewriter effect

    // Flag to indicate if the typewriter effect is running
    public bool isTypewriting = false;
    private bool skipRequested = false;

    // Coroutine reference for CountDownPower
    private int targetPowerNum;
    private Coroutine countDownPowerCoroutine;

    // Boolean for if a topic has been selected or not
    private bool isTopicSelected = false;
    public bool IsTopicSelected { get; set; }

    [SerializeField] private float discardDuration = 1.0f; // Duration of the discard animation
    private Coroutine discardCardsCoroutine;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        playedCards = new List<Card>(); // Initialize the list of played cards
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the StreamReader objects using the current session's playerReader and dateReader
        /*
        playerReader = currentSession.GetPlayerReader();
        dateReader = currentSession.GetReader();
        */
    }

    // Adds a card to the played cards list and removes it from the player's area
    public void AddCard(Card card)
    {
        if (card != null && !isTopicSelected)
        {
            playedCards.Add(card);
            playerArea.RemoveCards(card);
            CalculateScore();
        }
        else
        {
            Debug.LogError("There is no card to add.");
        }
    }

    // Scores the played cards, moves them to the discard pile, and updates the UI
    public void ScoreCards()
    {
        // Sets the initial power and line number of the convo topic that is selected
        if (!dialogPlayedAtFullPower)
        {
            initialPower = selectedConvoTopic.PowerNum;
            lineNum = 0;
            gameManager.turnCount = 1; // Resets the turn count
        }

        // Calculates our current score
        CalculateScore();

        // Moves all played cards to the discard pile
        discardCardsCoroutine = StartCoroutine(DiscardCards());

        // Target PowerNum after subtracting the score
        targetPowerNum = selectedConvoTopic.PowerNum - score;
        selectedConvoTopic.PowerNum -= score;

        // Start the CountDownPower coroutine (store reference)
        if (countDownPowerCoroutine != null)
        {
            StopCoroutine(countDownPowerCoroutine);  // Stop any existing CountDownPower coroutine
        }
        countDownPowerCoroutine = StartCoroutine(CountDownPower(selectedConvoTopic.PowerNum + score, targetPowerNum));

        // Checks if Dialog should be played
        CheckDialogTriggers();

        // If the conversation topic has been completed 
        if (selectedConvoTopic.PowerNum <= 0)
        {
            StopMultipleCoroutines();  // Stop all except CountDownPower
            StartCoroutine(PlayAllDialogsSequentially());

            selectedConvoTopic.isClicked = false;
            topicContainer.EnableButtons(); // Re-enable topic buttons

            // Move the completed topic to the 'done' list and remove it from the active list
            topicContainer.doneConvos.Add(selectedConvoTopic);

            topicContainer.convoTopics.Remove(selectedConvoTopic); 

            // Makes the dialog available for the next topic
            dialogPlayedAtZeroPower = false;
            dialogPlayedAtHalfPower = false;
            dialogPlayedAtFullPower = false;

            // Resets the bool for whether a topic is selected or not
            isTopicSelected = false;
        }

        // If the conversation topic has not been completed in time
        if (gameManager.turnCount >= gameManager.maxTurnCount && selectedConvoTopic.PowerNum > 0)
        {
            //StopAllCoroutinesExceptCountDownPower();  // Stop all except CountDownPower

            // Make the convo topic unclicked
            selectedConvoTopic.isClicked = false;
            topicContainer.EnableButtons(); // Re-enable topic buttons

            // Add the current convo topic to failed convos and remove it from the list of topics
            topicContainer.failedConvos.Add(selectedConvoTopic);
            topicContainer.convoTopics.Remove(selectedConvoTopic);
            selectedConvoTopic.isFailed = true;

            // Makes the dialog available for the next topic
            dialogPlayedAtZeroPower = false;
            dialogPlayedAtHalfPower = false;
            dialogPlayedAtFullPower = false;

            // Resets the bool for whether a topic is selected or not
            isTopicSelected = false;
        }

        // Clear the played cards, reset the score for the next round, and updates the total score
        playedCards.Clear();
        totalScore += score;
        totalScoreText.text = "Total Score: " + totalScore.ToString();
        currentScoreText.text = "Current Score: 0";
        score = 0;
    }

    // Checks if Dialog should be played
    private void CheckDialogTriggers()
    {
        //If we complete the topic, play the final bit
        if (!dialogPlayedAtZeroPower && dialogPlayedAtHalfPower && selectedConvoTopic.PowerNum <= 0)
        {
            dialogPlayedAtZeroPower = true;
            StartCoroutine(PlayDialog());
        }
        // If we have played the first bit but not the second, and the current convo topic's power is below half, play the second bit
        else if (!dialogPlayedAtHalfPower && dialogPlayedAtFullPower && selectedConvoTopic.PowerNum <= initialPower / 2)
        {
            dialogPlayedAtHalfPower = true;
            StartCoroutine(PlayDialog());
        }
        else if (!dialogPlayedAtFullPower)
        {
            dialogPlayedAtFullPower = true;
            StartCoroutine(PlayDialog());
        }
    }

    // Coroutine to play the dialog 
    private IEnumerator PlayDialog()
    {
        Cursor.lockState = CursorLockMode.Locked;

        List<string> lines;
        List<Sprite> sprites;
        List<string> speaker;

        //Clear the text
        playerText.text = "";
        dateText.text = "";

        // Depending on which convo topic we have selected, we change which lines and sprites we use
        switch (selectedConvoTopic.ConvoAttribute)
        {
            case "Cha":
                lines = currentSession.chaLines;
                sprites = currentSession.chaSprites;
                speaker = currentSession.chaSpeaker;
                break;
            case "Cou":
                lines = currentSession.couLines;
                sprites = currentSession.couSprites;
                speaker = currentSession.couSpeaker;
                break;
            case "Cle":
                lines = currentSession.cleLines;
                sprites = currentSession.cleSprites;
                speaker = currentSession.cleSpeaker;
                break;
            default:
                lines = currentSession.creLines;
                sprites = currentSession.creSprites;
                speaker = currentSession.creSpeaker;
                break;
        }

        // Determine the order of speakers based on whether "PC" is the speaker
        bool isPCSpeaker = speaker[lineNum] == "PC";

        // First speaker (either PC or Date)
        TextMeshProUGUI firstText = isPCSpeaker ? playerText : dateText;
        TextMeshProUGUI secondText = isPCSpeaker ? dateText : playerText;

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
        yield return StartCoroutine(TypewriteDialog(firstText, lines[lineNum], sprites[lineNum]));
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
        yield return StartCoroutine(TypewriteDialog(secondText, lines[lineNum], sprites[lineNum]));
        lineNum++;

        Cursor.lockState = CursorLockMode.None;
    }

    private IEnumerator TypewriteDialog(TextMeshProUGUI textComponent, string message, Sprite sprite)
    {
        isTypewriting = true;     // Typewriting is now active
        skipRequested = false;    // Reset the skip request
        currentSession.isWriting = true;

        foreach (char letter in message.ToCharArray())
        {
            if (skipRequested)
            {
                // If skip is requested, instantly complete the dialog
                textComponent.text = message;
                break;
            }

            textComponent.text += letter;
            yield return new WaitForSeconds(typewriterSpeed);  // Control typing speed
        }

        isTypewriting = false;  // Typewriting is complete
        currentSession.isWriting = false; 
    }

    void Update()
    {
        // Allow only skip input when dialog is playing
        if (isTypewriting && Input.GetButtonDown("Skip"))
        {
            skipRequested = true;
        }
    }

    // Coroutine that plays all dialogs sequentially
    private IEnumerator PlayAllDialogsSequentially()
    {
        // Play the dialog for full power if not yet played
        if (!dialogPlayedAtFullPower || lineNum == 0)
        {
            yield return StartCoroutine(PlayDialog());
            dialogPlayedAtFullPower = true;

            // Wait for the player to press the skip button before continuing
            yield return StartCoroutine(WaitForSkipButton());
        }

        // Play the dialog for half power if not yet played
        if (!dialogPlayedAtHalfPower || lineNum == 2)
        {
            yield return StartCoroutine(PlayDialog());
            dialogPlayedAtHalfPower = true;

            // Wait for the player to press the skip button before continuing
            yield return StartCoroutine(WaitForSkipButton());
        }

        // Play the dialog for zero power if not yet played
        if (!dialogPlayedAtZeroPower || lineNum == 4)
        {
            yield return StartCoroutine(PlayDialog());
            dialogPlayedAtZeroPower = true;

            // Wait for the player to press the skip button before continuing
            yield return StartCoroutine(WaitForSkipButton());
        }

        // Makes the dialog available for the next topic
        dialogPlayedAtZeroPower = false;
        dialogPlayedAtHalfPower = false;
        dialogPlayedAtFullPower = false;
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
        }

        if (gameManager.turnCount >= gameManager.maxTurnCount && selectedConvoTopic.PowerNum > 0)
        {
            selectedConvoTopic.numText.text = ""; // Hide the num text
            selectedConvoTopic.bustedText.SetActive(true); // Show the busted text
            selectedConvoTopic.background.color = new Color(1f, 0.5f, 0.5f, 1f); // Pastel red
            selectedConvoTopic = null;
        }
    }

    // Method to stop all coroutines except CountDownPower
    private void StopMultipleCoroutines()
    {
        // Store the CountDownPower coroutine
        Coroutine countdownCoroutine = countDownPowerCoroutine;
        Coroutine discardCoroutine = discardCardsCoroutine;

        // Stop all coroutines
        StopAllCoroutines();

        // Restart the coroutines if it's still valid
        if (countdownCoroutine != null)
        {
            countDownPowerCoroutine = StartCoroutine(CountDownPower(selectedConvoTopic.PowerNum + score, targetPowerNum));
        }
        if (discardCoroutine != null)
        {
            countDownPowerCoroutine = StartCoroutine(DiscardCards());
        }
    }

    private IEnumerator WaitForSkipButton()
    {
        skipRequested = false;
        while (!skipRequested)
        {
            if (Input.GetButtonDown("Skip"))
            {
                skipRequested = true;
            }
            yield return null; // Wait until the next frame before checking again
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

    // Method to calculate score (call anytime score may be changed)
    public void CalculateScore()
    {
        // Reset score to recalculate
        score = 0;

        // Iterate through all played cards to calculate the score
        for (int i = 0; i < playedCards.Count; i++)
        {
            // Add the card's power to the score
            score += playedCards[i].Power;

            // If there's a next card, apply bonuses based on matching attributes
            if (i + 1 < playedCards.Count)
            {
                Card card1 = playedCards[i];
                Card card2 = playedCards[i + 1];

                // Bonus points for matching types or similar power levels
                if (card1.Type == card2.Type) { score++; }
                if (card1.Power == card2.Power) { score++; }
                if (card1.Power == card2.Power - 1) { score++; }
            }

            // Bonus points for matching the selected conversation topic
            if (playedCards[i].Type == selectedConvoTopic.ConvoAttribute) { score++; }

            // Sets the current score text field to what the current score would be after recalculating
            currentScoreText.text = "Current Score: " + score.ToString();
        }
    }

    public IEnumerator DiscardCards()
    {
        // Get the world position of the discard pile directly
        Vector3 discardPilePosition = discard.transform.position;

        // Create a copy of the playedCards list to avoid modification errors
        List<Card> cardsToDiscard = new List<Card>(playedCards);

        // Start moving all cards simultaneously
        List<Coroutine> moveCoroutines = new List<Coroutine>();
        foreach (var card in cardsToDiscard)
        {
            // Ensure the card is in front of other UI elements
            card.transform.SetSiblingIndex(discard.transform.GetSiblingIndex() - 1);

            // Start a coroutine to move each card, and store the coroutine
            Coroutine moveCoroutine = StartCoroutine(MoveCardToDiscardPile(card, discardPilePosition, discardDuration));
            moveCoroutines.Add(moveCoroutine);
        }

        // Wait for all card move coroutines to complete
        foreach (var moveCoroutine in moveCoroutines)
        {
            yield return moveCoroutine;
        }

        // After all cards have been discarded, clear the playedCards list
        playedCards.Clear();
    }

    private IEnumerator MoveCardToDiscardPile(Card card, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = card.transform.position; // Starting position of the card
        float elapsedTime = 0f;

        // Move the card over time to the discard pile
        while (elapsedTime < duration)
        {
            float normalizedTime = elapsedTime / duration;
            float exponentialFactor = Mathf.Pow(normalizedTime, 3); // Exponential movement factor (cubic easing)

            // Apply the exponential factor to the movement
            card.transform.position = Vector3.Lerp(startPosition, targetPosition, exponentialFactor);

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure the card is exactly at the discard pile's position after the animation
        card.transform.position = targetPosition;

        // Parent the card to the discard pile AFTER the movement without affecting its position
        card.transform.SetParent(discard.transform, true); // 'true' ensures the world position stays the same

        // Optionally: Add to the discard pile in your logic
        discard.AddToDiscard(card);
    }

    public bool HasCard(Card card)
    {
        return playedCards.Contains(card);
    }
}