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

    //temp dummy convo info for testing purposes
    public void Start()
    {
        List<string> dummyTopics = new List<string>();
        List<CardInfo> dummyDeck = new List<CardInfo>();

        dummyTopics.Add("supercalifragilisticexpialidocius");
        dummyDeck.Add(new CardInfo(CardAttribute.Charisma, 1));
        dummyTopics.Add("supercalifragilisticexpialidocius");
        dummyDeck.Add(new CardInfo(CardAttribute.Charisma, 2));
        dummyTopics.Add("supercalifragilisticexpialidocius");
        dummyDeck.Add(new CardInfo(CardAttribute.Charisma, 3));
        dummyTopics.Add("supercalifragilisticexpialidocius");
        dummyDeck.Add(new CardInfo(CardAttribute.Charisma, 4));

        play = new PlayField(dummyDeck, dummyTopics);
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
        debugText.text = play.toString();
    }
}
