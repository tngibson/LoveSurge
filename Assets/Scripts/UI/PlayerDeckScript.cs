using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeckScript : MonoBehaviour
{
    [SerializeField] List<Card> deck = new List<Card>();
    public List<Card> Deck { get { return deck; } private set { } }
    [SerializeField] ChaCard chaCard;
    [SerializeField] CleCard cleCard;
    [SerializeField] CreCard creCard;
    [SerializeField] CouCard couCard;
    [SerializeField] GameObject container;
    [SerializeField] int cardCount;
    private void Awake()
    {
        fillDeck();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void addCard(Card card)
    {
        deck.Add(card);
    }

    void makeCard(Card prefab, int power)
    {
        Card finishedCard = Instantiate(prefab);
        finishedCard.Power = power;
        finishedCard.transform.SetParent(container.transform);
        addCard(finishedCard);
    }
    public void removeCard(Card card)
    {
        deck.Remove(card);
    }
    void fillDeck()
    {
        // fills deck with 3 charisma cards each of powers 1,2,3 (9 total)
        for (int i = 0; i < cardCount; i++)
        {
            makeCard(chaCard, 1);
        }
        for (int i = 0; i < cardCount; i++)
        {
            makeCard(chaCard, 2);
        }
        for (int i = 0; i < cardCount; i++)
        {
            makeCard(chaCard, 3);
        }
        // fills deck with 3 cleverness cards each of powers 1,2,3 (9 total)
        for (int i = 0; i < cardCount; i++)
        {
            makeCard(cleCard, 1);
        }
        for (int i = 0; i < cardCount; i++)
        {
            makeCard(cleCard, 2);
        }
        for (int i = 0; i < cardCount; i++)
        {
            makeCard(cleCard, 3);
        }
        // fills deck with 3 creativity cards each of powers 1,2,3 (9 total)
        for (int i = 0; i < cardCount; i++)
        {   
            makeCard(creCard, 1);
        }
        for (int i = 0; i < cardCount; i++)
        {
            makeCard(creCard, 2);
        }
        for (int i = 0; i < cardCount; i++)
        {
            makeCard(creCard, 3);
        }
        // fills deck with 3 courage cards each of powers 1,2,3 (9 total)
        for (int i = 0; i < cardCount; i++)
        {
            makeCard(couCard, 1);
        }
        for (int i = 0; i < cardCount; i++)
        {
            makeCard(couCard, 2);
        }
        for (int i = 0; i < cardCount; i++)
        {
            makeCard(couCard, 3);
        }
    }

    public Card draw()
    {
        if (deck.Count > 0)
        {
            int length = deck.Count;
            int CardChosen = Random.Range(0, length--);
            return deck[CardChosen];
        }
        else
        {
            print("deck is empty");
            return null;
        }


    }
}
