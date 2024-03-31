using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand
{
    List<CardInfo> CardsInHand;

    public void DrawToHand(CardInfo card)
    {
        CardsInHand.Add(card);
    }

    public CardInfo DiscardFromHand(CardInfo card)
    {
        foreach(CardInfo cardi in CardsInHand)
        {
            if (cardi.value == card.value & cardi.attribute == card.attribute)
            {
                CardsInHand.Remove(cardi);
            }
        }

        return card;
    }
}
