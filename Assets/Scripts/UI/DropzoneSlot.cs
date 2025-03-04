using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DropzoneSlot : MonoBehaviour
{
    [SerializeField] private List<Card> cardsInSlot = new List<Card>(); // List to hold multiple cards
    [SerializeField] private Card[] cardPrefabs; // Array of card prefabs to choose from

    public bool IsEmpty => cardsInSlot.Count == 0; // Check if the slot is empty
    public Card TopCard => cardsInSlot.Count > 0 ? cardsInSlot[cardsInSlot.Count - 1] : null; // Get the top card

    [SerializeField] private Dropzone dropzone;

    private void Awake()
    {
        // Add the starting card to the dropzone slot
        int randomAttribute = Random.Range(0, cardPrefabs.Length);
        int randomPower = Random.Range(1, 6);

        Card startingCard = Instantiate(cardPrefabs[randomAttribute]);
        startingCard.Power = randomPower;

        Destroy(startingCard.GetComponent<DragDrop>());
        dropzone.AddCardToDropzone(startingCard);
        dropzone.cardsToScore.Remove(startingCard);
        startingCard.isBottomCard = true;
    }

    public void AddCard(Card card)
    {
        cardsInSlot.Add(card); // Add card to the slot's list

        // Update the position of the card in the stack
        int index = cardsInSlot.Count - 1; // Determine stacking order
        card.transform.localPosition = new Vector3(0, index * 0.2f, 0); // Slightly offset
        card.transform.localScale = Vector3.one; // Ensure consistent scale
        card.transform.rotation = Quaternion.identity; // Reset rotation
    }

    public void RemoveTopCard()
    {
        if (cardsInSlot.Count > 0)
        {
            cardsInSlot.RemoveAt(cardsInSlot.Count - 1); // Remove only the top card
        }
    }

    public List<Card> GetCards() => new List<Card>(cardsInSlot); // Return a copy of the cards in the slot

    public void ClearAllCards()
    {
        cardsInSlot.Clear(); // Remove all cards from the slot
    }
}