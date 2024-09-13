using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArea : MonoBehaviour
{
    [SerializeField] List<Card> cardsInHand;
    public List<Card> CardsInHand { get { return cardsInHand; }  }
    private void Awake()
    {
        cardsInHand = new List<Card>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddCards(Card value)
    {
        CardsInHand.Add(value);
    }
    public void RemoveCards(Card value)
    {
        CardsInHand.Remove(value);
    }
}
