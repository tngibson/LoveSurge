using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton instance for global access

    // UI elements for displaying game information
    [SerializeField] private TextMeshProUGUI scoreText;  // UI for displaying the score
    [SerializeField] private TextMeshProUGUI endGameText;
    [SerializeField] private GameObject fullHandText;

    // Stress values
    [SerializeField] private float currentStressAmt = 0f;
    [SerializeField] private float maxStressAmt = 10f;

    // References to card prefabs
    [SerializeField] private GameObject chaCard;
    [SerializeField] private GameObject couCard;
    [SerializeField] private GameObject cleCard;
    [SerializeField] private GameObject creCard;

    // References to gameplay elements
    [SerializeField] private PlayerArea playerArea;
    [SerializeField] public ConvoTopic currentConvoTopic;
    [SerializeField] private TopicContainer topicContainer;
    [SerializeField] private PlayerDeckScript deckContainer;
    [SerializeField] private Dropzone dropzone;
    [SerializeField] private DiscardPile discard;
    [SerializeField] private GameObject discardBin;

    public List<string> categories = new List<string> { "Cha", "Cou", "Cle", "Cre" }; // List of categories for conversation topics

    // Buttons
    [SerializeField] private GameObject endTurnButton;
    [SerializeField] private GameObject mapButton;

    private int currentScore = 0;  // Tracks the player's score
    private bool isTopicSelected;

    public bool IsTopicSelected { get; set; }

    [SerializeField] private int handSize = 4;  // Max hand size the player can have

    // Initial game setup
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);  // Ensures only one instance of GameManager
        }
        else
        {
            instance = this;
        }

        StressBar.instance?.UpdateStressBar();
        SetConvoStart();
    }

    private void Start()
    {
        fullHandText.SetActive(false); // Hide the "full hand" warning initially
        scoreText.text = "Score: 0";   // Initialize the score UI
    }

    // Setup the conversation at the start of the game
    public void SetConvoStart()
    {
        if (isTopicSelected)
        {
            isTopicSelected = false;
        }
    }

    // Handles end of turn logic
    public void OnEndTurn()
    {
        // Ensure the player has up to (handSize) cards in their hand
        if (playerArea.CardsInHand.Count < handSize && deckContainer.Deck.Count > 0)
        {
            int missingCards = handSize - playerArea.CardsInHand.Count;
            fullHandText.SetActive(false);

            // Draw cards to fill the player's hand
            for (int i = 0; i < missingCards; i++)
            {
                Card card = deckContainer.Draw();
                if (card != null)
                {
                    deckContainer.RemoveCard(card);
                    card.transform.SetParent(playerArea.transform);
                    playerArea.AddCards(card);
                }
            }
        }
        else
        {
            // Show "full hand" warning if applicable
            fullHandText.SetActive(true);
        }

        // Score the cards in the dropzone
        if (currentConvoTopic != null)
        {
            dropzone.ScoreCards();
        }

        // Reset the dropzone for the next turn
        dropzone.ResetForNewTurn();

        // Check for game over conditions (empty deck and hand)
        if (deckContainer.Deck.Count <= 0 && playerArea.CardsInHand.Count == 0)
        {
            EndGame();
        }

        //UpdateEndTurnButton(false); // Disable the end turn button
    }

    // Ends the game and displays the game over message
    private void EndGame()
    {
        endGameText.text = "Game Over! You ran out of cards!";
        endTurnButton.SetActive(false);
        discardBin.SetActive(false);
        mapButton.SetActive(true); // Enable the map button at game over
    }

    // Updates the score and refreshes the UI
    public void UpdateScore(int score)
    {
        currentScore += score; // Add to the current score
        scoreText.text = $"Score: {currentScore}"; // Update the UI
    }

    // Resets the conversation topic selection state
    public void ResetConvoTopic()
    {
        currentConvoTopic = null;
        isTopicSelected = false;
        topicContainer.EnableButtons(); // Re-enable topic buttonss
    }

    // Toggles the interactivity of the end turn button
    public void UpdateEndTurnButton(bool state)
    {
        endTurnButton.GetComponent<Button>().interactable = state;
    }


}
