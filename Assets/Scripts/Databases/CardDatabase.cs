using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    public static CardDatabase Instance;

    [SerializeField] private List<Card> cardPrefabs;

    private Dictionary<string, Card> lookup;

    void Awake()
    {
        Instance = this;

        lookup = new Dictionary<string, Card>();
        foreach (var card in cardPrefabs)
        {
            lookup[card.CardID] = card;
        }
    }

    public Card Spawn(string id)
    {
        if (!lookup.ContainsKey(id)) return null;
        return Instantiate(lookup[id]);
    }
}