using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton instance for global access

    // UI elements for displaying game information
    [SerializeField] private TextMeshProUGUI scoreText;  // UI for displaying the score
    [SerializeField] private TextMeshProUGUI deckCountText;
    [SerializeField] private GameObject endGameText;
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
    [SerializeField] private ReserveManager reserveManager;

    public List<string> categories = new List<string> { "Cha", "Cou", "Cle", "Cre" }; // List of categories for conversation topics

    // Buttons
    [SerializeField] private GameObject endTurnButton;
    [SerializeField] private GameObject mapButton;

    [SerializeField] private int CurrentCharacterIndex;
    private int currentScore = 0;  // Tracks the player's score
    private bool isTopicSelected;

    public bool IsTopicSelected { get; set; }

    [SerializeField] private int handSize = 4;  // Max hand size the player can have

    private bool isHandPlayable = false;

    [SerializeField] private MapScript mapButtonScript;

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
        MusicManager.SetParameterByName("dateProgress", 0);
        Debug.Log("Start Music");

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

            deckCountText.text = deckContainer.Deck.Count.ToString();
        }
        else if (deckContainer.Deck.Count <= 0)
        {
            fullHandText.SetActive(true);
            fullHandText.GetComponentInChildren<TextMeshProUGUI>().text = "Your Deck is Empty!";
        }
        else
        {
            // Show "full hand" warning if applicable
            fullHandText.SetActive(true);
            fullHandText.GetComponentInChildren<TextMeshProUGUI>().text = "Your Hand is Full!";
        }

        // Score the cards in the dropzone
        if (currentConvoTopic != null)
        {
            dropzone.ScoreCards();
        }

        // Reset the dropzone for the next turn
        dropzone.ResetForNewTurn();

        // Check for card game win conditions (no more convo topics to choose from)
        if (topicContainer.convoTopics.Count <= 0)
        {
            EndGameFullWin();
        }
        else if (deckContainer.Deck.Count <= 0 && (playerArea.CardsInHand.Count == 0 || checkHandPlayable() == false))
        {
            EndGameLoss();
        }

        //UpdateEndTurnButton(false); // Disable the end turn button
    }

    public void EndGameHalfWin()
    {
        mapButtonScript.locName = "NokiDate2SkillCheck1"; // Hard coded for date 2 demo, will be changed later
        endGameText.GetComponentInChildren<TextMeshProUGUI>().text = "Noki want's to talk more closely with you...";
        endGameText.SetActive(true);
        endTurnButton.SetActive(false);
        discardBin.SetActive(false);
        mapButton.SetActive(true); // Enable the map button at game win
        MusicManager.SetParameterByName("dateProgress", 1);
    }

    private void EndGameFullWin()
    {
        mapButtonScript.locName = "NokiDate2SkillCheck2"; // Hard coded for date 2 demo, will be changed later
        endGameText.GetComponentInChildren<TextMeshProUGUI>().text = "You Win, Congratulations!";
        endGameText.SetActive(true);
        endTurnButton.SetActive(false);
        discardBin.SetActive(false);
        mapButton.SetActive(true); // Enable the map button at game win
        MusicManager.SetParameterByName("dateProgress", 3);
        Debug.Log("Good End");
    }

    // Ends the game and displays the game over message
    private void EndGameLoss()
    {
        endGameText.SetActive(true);
        endTurnButton.SetActive(false);
        discardBin.SetActive(false);
        mapButton.SetActive(true); // Enable the map button at game over
        MusicManager.SetParameterByName("dateProgress", 4);
        Debug.Log("Bad End");




        // Hard coded for date 2 demo, will be changed later
        if (!LocationManager.Instance.GetDateState(1))
        {
            LocationManager.Instance.SetDateState(1, true);
        }
        LocationManager.Instance.isPlayable = false;
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
        if (endTurnButton.GetComponent<Button>().interactable != state)
        {
            endTurnButton.GetComponent<Button>().interactable = state;
        }
    }

    private bool checkHandPlayable()
    {
        isHandPlayable = false;
        foreach (Card card in playerArea.CardsInHand)
        {
            if (dropzone.CanPlaceCard(card))
            {
                isHandPlayable = true;
            }
        }

        if (dropzone.CanPlaceCard(reserveManager.GetCurrentPlayableCard()))
        {
            isHandPlayable = true;
        }

        return isHandPlayable;
    }
}
