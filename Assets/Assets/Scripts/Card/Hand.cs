using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand
{
    public List<CardInfo> cardsInHand = new List<CardInfo>();

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

    public CardInfo DiscardFromHand(int cardIndex)
    {
        CardInfo cardInfo = cardsInHand[cardIndex];
        cardsInHand.RemoveAt(cardIndex);
        return cardInfo;
    }
}
