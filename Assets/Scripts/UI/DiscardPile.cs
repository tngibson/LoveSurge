using System.Collections.Generic;
using UnityEngine;

public class DiscardPile : MonoBehaviour
{
    // A list to store the discarded cards
    private List<Card> pile;

    // Property to access the discard pile
    public List<Card> Pile { get { return pile; } }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the pile as an empty list
        pile = new List<Card>();
    }

    // Adds a card to the discard pile
    public void AddToDiscard(Card played)
    {
        // Add the card to the pile
        pile.Add(played);

        // Set the card's image to fully opaque
        played.SetImageAlpha(1f);

        // Remove components to prevent the card from being moved or swapped
        RemoveCardInteractions(played);
    }

    // Method to remove DragDrop and GridElementSwapper components from the card
    private void RemoveCardInteractions(Card played)
    {
        var dragDrop = played.GetComponent<DragDrop>();
        if (dragDrop != null)
        {
            Destroy(dragDrop);
        }

        var gridSwapper = played.GetComponent<GridElementSwapper>();
        if (gridSwapper != null)
        {
            Destroy(gridSwapper);
        }
    }
}