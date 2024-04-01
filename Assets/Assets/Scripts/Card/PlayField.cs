using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayField
{
    public Hand playerHand;
    public DiscardPile discardPile;
    public Deck deck;

    List<CardInfo> playedCards = new List<CardInfo>();

    public void DrawDeckToHand()
    {
        playerHand.AddToHand(deck.DrawFromDeck());
    }

    public void PlayCard(CardInfo card)
    {
        playedCards.Add(playerHand.DiscardFromHand(card));
    }

}
