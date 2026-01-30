using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerDeckScript : MonoBehaviour
{
    // The deck of cards managed by this script
    [SerializeField] private List<Card> deck = new List<Card>();
    public List<Card> Deck => deck;

    // Serialized references to the different card prefabs
    [SerializeField] private ChaCard chaCard;
    [SerializeField] private CleCard cleCard;
    [SerializeField] private CreCard creCard;
    [SerializeField] private CouCard couCard;
    [SerializeField] private StressCard stressCard;
    // The container where cards are instantiated
    [SerializeField] private GameObject container;

    // Number of cards per type and power level
    [SerializeField] private int cardCount;

    [SerializeField] private int maxCardPower = 5;
    [SerializeField] private bool disableRandomDraw = false;

    private List<string> ignoredTags;

    public static readonly string STRESS_THRESH_2 = "StressThreshold2";
    public static readonly string STRESS_THRESH_3 = "StressThreshold3";

    [SerializeField] private TextMeshProUGUI deckCountText;

    [SerializeField] private GameObject deckBackground;

    private bool isShowing = false;

    private Vector2 initialPosition;

    // Initialize the deck on Awake
    protected void Start()
    {
        ignoredTags = new List<string>() { StatOffset.STRESS_FOUR };
        FillDeck();

        initialPosition = transform.position;

        deckCountText.text = deck.Count.ToString();
    }

    // Adds a card to the deck
    private void AddCard(Card card)
    {
        deck.Add(card);
    }

    // Instantiates a card, assigns its power, and adds it to the deck
    public Card MakeCard(Card prefab, int power)
    {
        Card finishedCard = Instantiate(prefab, container.transform);
        finishedCard.SetVisibility(false);
        finishedCard.Power = power;
        AddCard(finishedCard);

        return finishedCard;
    }

    // Removes a card from the deck
    public void RemoveCard(Card card)
    {
        deck.Remove(card);
    }

    // Fills the deck with cards of each type and power level
    protected virtual void FillDeck()
    {
        // Loop through the cards and create the deck based on `cardCount` for each power level
        for (int i = 1; i <= maxCardPower; i++) // Powers from 1 to maxCardPower
        {
            for (int j = 0; j < cardCount; j++)
            {
                Card card1 = MakeCard(chaCard, i);
                Card card2 = MakeCard(cleCard, i);
                Card card3 = MakeCard(creCard, i);
                Card card4 = MakeCard(couCard, i);

                // Set cards that had their power decreased so the UI can reflect this
                if (Player.GetSafeOffsets()[0].GetAmount(ignoredTags) < 0) card1.Debuffed = true;
                if (Player.GetSafeOffsets()[1].GetAmount(ignoredTags) < 0) card2.Debuffed = true;
                if (Player.GetSafeOffsets()[3].GetAmount(ignoredTags) < 0) card3.Debuffed = true;
                if (Player.GetSafeOffsets()[2].GetAmount(ignoredTags) < 0) card4.Debuffed = true;
            }
        }

        AddStressCards();
        AddCollectedCards();
    }

    // Draw a random card from the deck
    public Card Draw()
    {
        if (deck.Count > 0)
        {
            // Makes testing hands easier
            if (disableRandomDraw)
            {
                Card cardToDraw = deck[0];
                cardToDraw.SetVisibility(true);
                RemoveCard(cardToDraw);
                return cardToDraw;
            }

            // Randomly select a card and remove it from the deck
            int cardChosen = Random.Range(0, deck.Count);
            Card drawnCard = deck[cardChosen];
            drawnCard.SetVisibility(true);
            RemoveCard(drawnCard);
            return drawnCard;
        }
        else
        {
            Debug.LogWarning("Deck is empty");
            return null;
        }
    }

    public void DrawCards(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Card card = Draw();
            if (card == null)
            {
                Debug.LogWarning("No more cards to draw.");
                break; // Exit the loop if no more cards are available
            }

            PlayerArea playerArea = GameManager.instance?.PlayerArea;
            card.transform.SetParent(playerArea.transform);  // Set the parent of the card to the player's area
            playerArea.AddCards(card);  // Add the card to the player's area
        }
    }

    private void AddStressCards()
    {
        // Checks if player has stress
        if (StressManager.instance.currentStressAmt > 0)
        { 
            // Get the number of filled bars and sum that number with n-1 repeatedly, to get the number of cards to spawn
            for (int i = 0; i < ReverseSum(StressManager.GetStressBarsFilled(StressManager.instance.currentStressAmt)); i++)
            {
                MakeCard(stressCard, 0);
            }
        }
    }

    private int ReverseSum(int num)
    {
        int sum = 0;
        for (int i = num; i >= 0; i--)
        {
            sum += i;
        }

        return sum;
    }

    public void ShowDeck()
    {
        if (isShowing == false)
        {
            isShowing = true;

            deckBackground.SetActive(true);

            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            foreach (Card card in deck)
            {
                card.SetVisibility(true);
            }
        }
        else
        {
            isShowing = false;

            deckBackground.SetActive(false);

            transform.position = initialPosition;

            foreach (Card card in deck)
            {
                card.SetVisibility(false);
            }
        }
    }

    public void AddCollectedCards()
    {
        foreach (Card card in Player.instance.collectedCards)
        {
            Debug.Log("Adding collected card to deck: " + card.name);
            Card newCard = MakeCard(card, card.Power);
            newCard.gameObject.SetActive(true);
            newCard.transform.localScale = Vector3.one;
            newCard.Debuffed = false;
        }
    }
}