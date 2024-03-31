using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum Cards
{
    Charisma,
    Courage,
    Clever
}

public struct CardInfo
{
    public int value;
    public Cards cards;

    public CardInfo(Cards cards, int value)
    {
        this.value = value;
        this.cards = cards;
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
