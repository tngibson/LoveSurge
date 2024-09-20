using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class Dropzone : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI playerText;
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private List<Card> playedCards;
    [SerializeField] private DiscardPile discard;
    [SerializeField] private PlayerArea playerArea;
    [SerializeField] private TopicContainer topicContainer;
    [SerializeField] private Playtest currentSession;
    [SerializeField] public ConvoTopic selectedConvoTopic;

    /*
    private StreamReader playerReader;
    private StreamReader dateReader;
    */ 

    private int score = 0;
    private int lineNum = 0;
    private int initialPower;
    private bool dialogPlayedAtFullPower = false;
    private bool dialogPlayedAtHalfPower = false;
    private bool dialogPlayedAtZeroPower = false;

    // Typewriter settings
    [SerializeField] private float typewriterSpeed = 0.05f;  // Speed of the typewriter effect

    // Flag to indicate if the typewriter effect is running
    private bool isTypewriting = false;
    private bool skipRequested = false;

    // Coroutine reference for CountDownPower
    private int targetPowerNum;
    private Coroutine countDownPowerCoroutine;

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
        if (card != null)
        {
            playedCards.Add(card);
            playerArea.RemoveCards(card);
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
        }

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

            // Move the card to the discard pile and update its position
            discard.AddToDiscard(playedCards[i]);


            playedCards[i].transform.SetParent(discard.transform, false);
            playedCards[i].transform.position = discard.transform.position;
        }

        // Target PowerNum after subtracting the score
        targetPowerNum = selectedConvoTopic.PowerNum - score;
        selectedConvoTopic.PowerNum -= score;

        // Start the CountDownPower coroutine (store reference)
        if (countDownPowerCoroutine != null)
        {
            StopCoroutine(countDownPowerCoroutine);  // Stop any existing CountDownPower coroutine
        }
        countDownPowerCoroutine = StartCoroutine(CountDownPower(selectedConvoTopic.PowerNum + score, targetPowerNum));

        // Update the score display in the UI
        scoreText.text = "Round Score: " + score.ToString();

        // Checks if Dialog should be played
        CheckDialogTriggers();

        // If the conversation topic has been completed 
        if (selectedConvoTopic.PowerNum <= 0)
        {
            StopAllCoroutinesExceptCountDownPower();  // Stop all except CountDownPower
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
        }

        // Clear the played cards and reset the score for the next round
        playedCards.Clear();
        score = 0;
    }

    // Checks if Dialog should be played
    private void CheckDialogTriggers()
    {
        //If we complete the topic, play the final bit
        if (!dialogPlayedAtZeroPower && dialogPlayedAtHalfPower && selectedConvoTopic.PowerNum <= 0)
        {
            StartCoroutine(PlayDialog());
            dialogPlayedAtZeroPower = true;
        }
        // If we have played the first bit but not the second, and the current convo topic's power is below half, play the second bit
        else if (!dialogPlayedAtHalfPower && dialogPlayedAtFullPower && selectedConvoTopic.PowerNum <= initialPower / 2)
        {
            StartCoroutine(PlayDialog());
            dialogPlayedAtHalfPower = true;
        }
        else if (!dialogPlayedAtFullPower)
        {
            StartCoroutine(PlayDialog());
            dialogPlayedAtFullPower = true;
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
    }

    // Method to stop all coroutines except CountDownPower
    private void StopAllCoroutinesExceptCountDownPower()
    {
        // Store the CountDownPower coroutine
        Coroutine savedCoroutine = countDownPowerCoroutine;

        // Stop all coroutines
        StopAllCoroutines();

        // Restart the CountDownPower coroutine if it's still valid
        if (savedCoroutine != null)
        {
            countDownPowerCoroutine = StartCoroutine(CountDownPower(selectedConvoTopic.PowerNum + score, targetPowerNum));
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
    }
}