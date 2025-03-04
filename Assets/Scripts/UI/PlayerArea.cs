using System.Collections.Generic;
using UnityEngine;

public class PlayerArea : MonoBehaviour
{
    // List to store the player's hand of cards
    [SerializeField] private List<Card> cardsInHand;
    public List<Card> CardsInHand { get { return cardsInHand; } }

    // Initialize the cardsInHand list
    private void Awake()
    {
        cardsInHand = new List<Card>();
    }

    // Add a card to the player's hand
    public void AddCards(Card card)
    {
        cardsInHand.Add(card); 
    }

    // Remove a card from the player's hand
    public void RemoveCards(Card card)
    {
        cardsInHand.Remove(card);
    }
}