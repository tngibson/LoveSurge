using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class PlayField
{
    public Hand playerHand;
    public DiscardPile discardPile;
    public Deck deck;

    //create a fillhand function and figure out how hand sizes work

    List<CardInfo> playedCards = new List<CardInfo>();
    List<DialogueOption> dialogueOptions;

    public void DrawDeckToHand()
    {
        playerHand.AddToHand(deck.DrawFromDeck());
    }

    public void PlayCard(CardInfo card)
    {
        playedCards.Add(playerHand.DiscardFromHand(card));
    }

    public void GenerateDialogueOption(List<string> Topics)
    {
        dialogueOptions = new List<DialogueOption>();

        for (int i = 0; i < 4; i++)
        {
            //smallNum & bigNum are hard coded here, but will not be later on
            //topics hard coded here, but will be randomly selected from list of topics later on
            dialogueOptions.Add(new DialogueOption(Topics[i], 3, 6));
        }
    }
}


public class DialogueOption
{
    string topic;
    public int smallNum;
    public int bigNum;

    List<CardInfo> stackedCards;

     public DialogueOption(string topic, int smallNum, int bigNum)
    {
        this.topic = topic;
        this.smallNum = smallNum;
        this.bigNum = bigNum;
    }

    public void AddToStack(CardInfo card)
    {
        stackedCards.Add(card);
    }

    public int CheckTurnSum()
    {
        int total = 0;

        foreach(CardInfo card in stackedCards)
        {
            total += card.value;
        }
        return total;
    }
}