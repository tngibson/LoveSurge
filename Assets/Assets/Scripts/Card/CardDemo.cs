using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
