using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardPile
{
    List<CardInfo> DiscardedCards;

    
    public void AddToDiscard(CardInfo card)
    {
        DiscardedCards.Add(card);
    }

    public CardInfo GetFromDiscard(CardInfo card)
    {
        foreach(CardInfo cardi in DiscardedCards)
        {
            if (cardi.value == card.value & cardi.attribute == card.attribute)
            {
                DiscardedCards.Remove(cardi);
            }
        }

        return card;
    }

}
