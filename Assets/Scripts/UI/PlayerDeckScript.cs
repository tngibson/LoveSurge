using System.Collections.Generic;
using UnityEngine;

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

    // Initialize the deck on Awake
    private void Awake()
    {
        FillDeck();
    }

    // Adds a card to the deck
    private void AddCard(Card card)
    {
        deck.Add(card);
    }

    // Instantiates a card, assigns its power, and adds it to the deck
    private void MakeCard(Card prefab, int power)
    {
        Card finishedCard = Instantiate(prefab, container.transform);
        finishedCard.Power = power;
        AddCard(finishedCard);
    }

    // Removes a card from the deck
    public void RemoveCard(Card card)
    {
        deck.Remove(card);
    }

    // Fills the deck with cards of each type and power level
    private void FillDeck()
    {
        // Loop through the cards and create the deck based on `cardCount` for each power level
        for (int i = 1; i <= 3; i++) // Powers 1, 2, 3
        {
            for (int j = 0; j < cardCount; j++)
            {
                MakeCard(chaCard, i);
                MakeCard(cleCard, i);
                MakeCard(creCard, i);
                MakeCard(couCard, i);
            }
        }
        addStressCards();
    }

    // Draw a random card from the deck
    public Card Draw()
    {
        if (deck.Count > 0)
        {
            // Randomly select a card and remove it from the deck
            int cardChosen = Random.Range(0, deck.Count);
            Card drawnCard = deck[cardChosen];
            RemoveCard(drawnCard);
            return drawnCard;
        }
        else
        {
            Debug.LogWarning("Deck is empty");
            return null;
        }
    }

    private void addStressCards()
    {
        // Checks if player has stress
        if (StressManager.instance.currentStressAmt > 0)
        {
            // since stress amount is a decimal between 0 and 1, we multiply  the amount by 10 to get a whole number and add that many stress cards, eventually need to figure out a better formula for this
            for (int i = 0; i < (StressManager.instance.currentStressAmt * 10); i++)
            {
                MakeCard(stressCard, i);
            }
        }   
    }
}

