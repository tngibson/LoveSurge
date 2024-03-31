using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    Stack<CardInfo> deck;

    public CardInfo DrawFromDeck()
    {
        return deck.Pop();
    }

    public void ShuffleDeck()
    {
        Debug.Log("shufflelupagus");
    }
}
