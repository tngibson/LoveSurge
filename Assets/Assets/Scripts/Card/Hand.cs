using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand
{
    List<CardInfo> cardsInHand;

    public void AddToHand(CardInfo card)
    {
        cardsInHand.Add(card);
    }

    public CardInfo DiscardFromHand(CardInfo card)
    {
        foreach(CardInfo cardi in cardsInHand)
        {
            if (cardi.value == card.value & cardi.attribute == card.attribute)
            {
                cardsInHand.Remove(cardi);
            }
        }
        return card;
    }
}
