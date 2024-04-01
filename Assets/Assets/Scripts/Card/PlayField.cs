using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayField
{
    public Hand playerHand;
    public DiscardPile discardPile;
    public Deck deck;

    //create a fillhand function and figure out how hand sizes work

    public List<DialogueOption> dialogueOptions;

    public PlayField(List<CardInfo> newDeck, List<string> Topics)
    {
        playerHand = new Hand();
        discardPile = new DiscardPile();
        deck = new Deck(newDeck);

        //for now four cards are put into the hand, this is not determined yet
        DrawDeckToHand();
        DrawDeckToHand();
        DrawDeckToHand();
        DrawDeckToHand();

        GenerateDialogueOption(Topics);
    }

    public void DrawDeckToHand()
    {
        playerHand.AddToHand(deck.DrawFromDeck());
    }

    public void PlayCard(CardInfo card, DialogueOption dialogueOption)
    {
        dialogueOption.AddToStack(playerHand.DiscardFromHand(card));
    }

    public void PlayCard(int cardIndex, DialogueOption dialogueOption)
    {
        dialogueOption.AddToStack(playerHand.DiscardFromHand(cardIndex));
    }

    public void PlayCard(int cardIndex, int dialogueOption)
    {
        dialogueOptions[dialogueOption].AddToStack(playerHand.DiscardFromHand(cardIndex));
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

    public void ReplaceDialogueOption(DialogueOption dialogueOption, List<string> Topics)
    {
        dialogueOptions.Remove(dialogueOption);

        //hard coded, but will not be later
        dialogueOptions.Add(new DialogueOption(Topics[0], 3, 6));
    }

    public int GetHandSize()
    {
        return playerHand.cardsInHand.Count;
    }

    public string toString()
    {
        string text = "";

        foreach(DialogueOption dialogueOption in dialogueOptions)
        {
            text += "DIALOGUEOPTION: " + dialogueOption.topic + " " + dialogueOption.smallNum + " " + dialogueOption.bigNum + " total value: " + dialogueOption.CheckTurnSum() + "\n";
        }
        text += "\n\n\n";

        foreach(CardInfo card in playerHand.cardsInHand)
        {
            text += "HAND: " + card.value + " " + card.attribute + "\n";
        }
        return text;
    }
}


public class DialogueOption
{
    public string topic;
    public int smallNum;
    public int bigNum;
    public bool isUnlocked;

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
        
        if (CheckTurnSum() >= smallNum)
        {
            isUnlocked = true;
        }
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