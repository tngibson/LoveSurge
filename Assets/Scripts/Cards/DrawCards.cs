using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCards : MonoBehaviour
{
    [SerializeField] private GameObject card1;
    [SerializeField] private GameObject card2;
    [SerializeField] private GameObject card3;
    [SerializeField] private GameObject card4;
    [SerializeField] private PlayerDeckScript deckContainer;  // Reference to the deck containing cards
    [SerializeField] private PlayerArea playerArea;  // Reference to the player's area where cards will be added

    public void OnClick()
    {
        // Draw 5 cards from the deck and add them to the player's area
        for (int i = 0; i < 5; i++)
        {
            Card card = deckContainer.Draw();
            if (card == null)
            {
                //Debug.LogWarning("No more cards to draw.");
                break; // Exit the loop if no more cards are available
            }

            deckContainer.RemoveCard(card);  // Remove the drawn card from the deck
            card.transform.SetParent(playerArea.transform);  // Set the parent of the card to the player's area
            playerArea.AddCards(card);  // Add the card to the player's area
        }
        AudioManager.instance.PlayOneShot(FMODEvents.instance.cardShuffle, this.transform.position);

        // Deactivate the GameObject to indicate that the draw process is complete
        gameObject.SetActive(false);
    }
}
