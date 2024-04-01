using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    // handles player input and displays the board information
}
