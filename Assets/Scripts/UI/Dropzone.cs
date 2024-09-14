using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class Dropzone : MonoBehaviour
{
    // UI elements for displaying score, player, and date
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI playerText;
    [SerializeField] private TextMeshProUGUI dateText;

    // List to store the cards played in the dropzone
    [SerializeField] private List<Card> playedCards;

    // References to other gameplay objects
    [SerializeField] private DiscardPile discard;
    [SerializeField] private PlayerArea playerArea;
    [SerializeField] private TopicContainer topicContainer;
    [SerializeField] private Playtest currentSession;

    // The currently selected conversation topic and its associated card
    [SerializeField] public ConvoTopic selectedConvoTopic;

    // Stream readers for reading player and date information
    private StreamReader playerReader;
    private StreamReader dateReader;

    // Keeps track of the score for the current round
    private int score = 0;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        playedCards = new List<Card>(); // Initialize the list of played cards
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the StreamReader objects using the current session's playerReader and dateReader
        playerReader = currentSession.GetPlayerReader();
        dateReader = currentSession.GetReader();
    }

    // Adds a card to the played cards list and removes it from the player's area
    public void AddCard(Card card)
    {
        playedCards.Add(card);
        playerArea.RemoveCards(card);
    }

    // Scores the played cards, moves them to the discard pile, and updates the UI
    public void ScoreCards()
    {
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

        // If the conversation topic has been completed (PowerNum is <= 0)
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

        // Read the next date and player info from the session and update the UI
        currentSession.ReadText(dateText, dateReader);
        currentSession.ReadText(playerText, playerReader);
    }

    // Swaps two cards in the played cards list by index
    public void SwapCards(int cardIndex1, int cardIndex2)
    {
        Card temp = playedCards[cardIndex1];
        playedCards[cardIndex1] = playedCards[cardIndex2];
        playedCards[cardIndex2] = temp;
    }
}