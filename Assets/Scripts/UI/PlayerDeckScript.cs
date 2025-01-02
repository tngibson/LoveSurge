using System.Collections.Generic;
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

    // Initialize the deck on Awake
    private void Awake()
    {
        //if (deckFilled == false)
        //{
        FillDeck();
        //deckFilled = true;
        //}
    }

    // Adds a card to the deck
    private void AddCard(Card card)
    {
        deck.Add(card);
    }

    // Instantiates a card, assigns its power, and adds it to the deck
    private Card MakeCard(Card prefab, int power)
    {
        Card finishedCard = Instantiate(prefab, container.transform);
        finishedCard.Power = power;
        finishedCard.SetVisiblity(false);
        AddCard(finishedCard);

        return finishedCard;
    }

    // Removes a card from the deck
    public void RemoveCard(Card card)
    {
        deck.Remove(card);
    }

    // Fills the deck with cards of each type and power level
    private void FillDeck()
    {
        int[] offsets = CalculateStressPowerOffsets();
        // Loop through the cards and create the deck based on `cardCount` for each power level
        for (int i = 1; i <= maxCardPower; i++) // Powers from 1 to maxCardPower
        {
            for (int j = 0; j < cardCount; j++)
            {
                Card card1 = MakeCard(chaCard, i + offsets[0]);
                Card card2 = MakeCard(cleCard, i + offsets[1]);
                Card card3 = MakeCard(creCard, i + offsets[2]);
                Card card4 = MakeCard(couCard, i + offsets[3]);

                // Set cards that had their power decreased so the UI can reflect this
                if (offsets[0] < 0) card1.Debuffed = true;
                if (offsets[1] < 0) card2.Debuffed = true;
                if (offsets[2] < 0) card3.Debuffed = true;
                if (offsets[3] < 0) card4.Debuffed = true;
            }
        }
        AddStressCards();
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
                cardToDraw.SetVisiblity(true);
                RemoveCard(cardToDraw);
                return cardToDraw;
            }

            // Randomly select a card and remove it from the deck
            int cardChosen = Random.Range(0, deck.Count);
            Card drawnCard = deck[cardChosen];
            drawnCard.SetVisiblity(true);
            RemoveCard(drawnCard);
            return drawnCard;
        }
        else
        {
            Debug.LogWarning("Deck is empty");
            return null;
        }
    }

    private void AddStressCards()
    {
        // Checks if player has stress
        if (StressManager.instance.currentStressAmt > 0)
        {
            // since stress amount is a decimal between 0 and 1, we multiply  the amount by 10 to get a whole number and add that many stress cards, eventually need to figure out a better formula for this
            for (int i = 0; i <= StressManager.instance.numStressBars - 
                 StressManager.GetStressBarsFilled(StressManager.instance.currentStressAmt); i++)
            {
                MakeCard(stressCard, i);
            }
        }
    }

    private int[] CalculateStressPowerOffsets()
    {
        int[] offsets = new[] { 0, 0, 0, 0 };

        if (StressManager.instance.currentStressAmt <= 0) return offsets;

        if (StressManager.GetStressBarsFilled(StressManager.instance.currentStressAmt) == 2)
        {
            int index = Random.Range(0, 4);
            offsets[index] = -1;
            return offsets;
        }

        if (StressManager.GetStressBarsFilled(StressManager.instance.currentStressAmt) == 3)
        {
            int index = Random.Range(0, 4);
            offsets[index] = -1;

            int secondIndex = index;
            while (secondIndex == index)
                secondIndex = Random.Range(0, 4);

            offsets[secondIndex] = -1;
            return offsets;
        }

        return offsets;
    }
}