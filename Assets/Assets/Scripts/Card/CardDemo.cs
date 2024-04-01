using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using JetBrains.Annotations;
using UnityEngine;
using TMPro; 
using UnityEngine.UI;

public enum CardAttribute
{
    Charisma,
    Courage,
    Cleverness, 
    Creativity
}


public struct CardInfo
{
    public int value;
    public CardAttribute attribute;

    public CardInfo(CardAttribute attribute, int value)
    {
        this.value = value;
        this.attribute = attribute;
    }
}


public class CardDemo : MonoBehaviour
{
    public PlayField play;
    public int handIndex; //temp variable to represent selected card in hand
    public int dialogueIndex; //temp also
    public TextMeshProUGUI debugText;
    public bool debugActive;

    //temp dummy convo info for testing purposes
    public void Start()
    {
        List<string> dummyTopics = new List<string>();
        List<CardInfo> dummyDeck = new List<CardInfo>();

        dummyTopics.Add("char1");
        dummyDeck.Add(new CardInfo(CardAttribute.Charisma, 1));
        dummyTopics.Add("cour2");
        dummyDeck.Add(new CardInfo(CardAttribute.Courage, 2));
        dummyTopics.Add("clev3");
        dummyDeck.Add(new CardInfo(CardAttribute.Cleverness, 3));
        dummyTopics.Add("crea4");
        dummyDeck.Add(new CardInfo(CardAttribute.Creativity, 4));
        dummyTopics.Add("char5");
        dummyDeck.Add(new CardInfo(CardAttribute.Charisma, 5));
        dummyTopics.Add("cour6");
        dummyDeck.Add(new CardInfo(CardAttribute.Courage, 6));
        dummyTopics.Add("clev7");
        dummyDeck.Add(new CardInfo(CardAttribute.Cleverness, 7));
        dummyTopics.Add("crea8");
        dummyDeck.Add(new CardInfo(CardAttribute.Creativity, 8));

        play = new PlayField(dummyDeck, dummyTopics);

        debugActive = true;
    }

    //function to handles player input and display the board information
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            handIndex -= 1;

            if (handIndex < 0)
            {
                play.GetHandSize();

                handIndex = play.GetHandSize() - 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            handIndex += 1;

            if (handIndex >= play.GetHandSize())
            {
                handIndex = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            play.PlayCard(handIndex, dialogueIndex);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            dialogueIndex += 1;

            if (dialogueIndex >= play.dialogueOptions.Count)
            {
                dialogueIndex = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            dialogueIndex -= 1;

            if (dialogueIndex < 0)
            {
                dialogueIndex = play.dialogueOptions.Count - 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            play.SelectDialogueOption(dialogueIndex);
            play.DrawDeckToHand();
        }

        if (debugActive == true)
        {
            debugText.text = play.toString() + "\n handIndex " + handIndex + "\n dialogueIndex " + dialogueIndex;
        }
    }
}
