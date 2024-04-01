using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardPile
{
    List<CardInfo> discardedCards;

    public void AddToDiscard(CardInfo card)
    {
        discardedCards.Add(card);
    }

    public CardInfo GetFromDiscard(CardInfo card)
    {
        foreach(CardInfo cardi in discardedCards)
        {
            if (cardi.value == card.value & cardi.attribute == card.attribute)
            {
                discardedCards.Remove(cardi);
            }
        }

        return card;
    }
}
