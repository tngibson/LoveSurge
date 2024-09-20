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
        // Sets the initial power of the convo topic that is selected
        if (!dialogPlayedAtFullPower)
        {
            initialPower = selectedConvoTopic.PowerNum;
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

        // Update the selected conversation topic's power and the associated UI
        selectedConvoTopic.PowerNum -= score;
        selectedConvoTopic.numText.text = selectedConvoTopic.PowerNum.ToString();

        // Update the score display in the UI
        scoreText.text = "Round Score: " + score.ToString();

        // Clear the played cards and reset the score for the next round
        playedCards.Clear();
        score = 0;

        // Checks if Dialog should be played
        CheckDialogTriggers();

        // If the conversation topic has been completed 
        if (selectedConvoTopic.PowerNum <= 0)
        {
            selectedConvoTopic.isClicked = false;
            topicContainer.EnableButtons(); // Re-enable topic buttons
            selectedConvoTopic.gameObject.SetActive(false); // Hide the completed topic

            // Move the completed topic to the 'done' list and remove it from the active list
            topicContainer.doneConvos.Add(selectedConvoTopic);

            topicContainer.convoTopics.Remove(selectedConvoTopic); 
            selectedConvoTopic = null; // Clear the selected topic
        }
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
        else
        {
            StartCoroutine(PlayDialog());
            dialogPlayedAtFullPower = true;
        }
    }

    // Coroutine to play the dialog 
    private IEnumerator PlayDialog()
    {
        List<string> lines;
        List<Sprite> sprites;

        //Clear the text
        playerText.text = "";
        dateText.text = "";

        // Depending on which convo topic we have selected, we change which lines and sprites we use
        switch (selectedConvoTopic.ConvoAttribute)
        {
            case "Cha":
                lines = currentSession.chaLines;
                sprites = currentSession.chaSprites;
                break;
            case "Cou":
                lines = currentSession.couLines;
                sprites = currentSession.couSprites;
                break;
            case "Cle":
                lines = currentSession.cleLines;
                sprites = currentSession.cleSprites;
                break;
            default:
                lines = currentSession.creLines;
                sprites = currentSession.creSprites;
                break;
        }

        // Reads the first line (either PC or Date)
        currentSession.ReadText(sprites[lineNum]);
        yield return StartCoroutine(TypewriteDialog(playerText, lines[lineNum], sprites[lineNum]));
        lineNum++;

        // Reads the second line (whichever didn't go)
        currentSession.ReadText(sprites[lineNum]);
        yield return StartCoroutine(TypewriteDialog(dateText, lines[lineNum], sprites[lineNum]));
        lineNum++;
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
        // Check for skip input only when typewriting is active
        if (isTypewriting && Input.GetButtonDown("Skip"))
        {
            skipRequested = true;
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