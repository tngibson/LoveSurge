using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropZoneCheck : MonoBehaviour
{
    [SerializeField] private DropzoneSlot dropzoneSlot;
    [SerializeField] private PlayTutorialDialog playTutorialDialogSuccess;
    [SerializeField] private PlayTutorialDialog playTutorialDialogFailure;

    void Update()
    {
        if (dropzoneSlot.CardsInSlot.Count == 2 && 
            dropzoneSlot.CardsInSlot[1].Power == 1 && 
            dropzoneSlot.CardsInSlot[1].Type.Equals("Cou"))
        {
            playTutorialDialogSuccess.PlayDialog();
            enabled = false;
        }
        else if (dropzoneSlot.CardsInSlot.Count == 2)
        {
            playTutorialDialogFailure.PlayDialog();
            dropzoneSlot.RemoveTopCard();
        }
    }


    // public void ReturnCard(Card card)
    // {
    //     if (card != null)
    //     {
    //         // Remove the top card from the dropzone and add it back to the player's hand (if not a reserve card)
    //         if (!card.isReserveCard)
    //         {
    //             playerArea.AddCards(card);
    //         }
    //         cardsToScore.Remove(card);
    //         dropzone.RemoveTopCard();

    //         // Update the last placed card to the new top card
    //         lastPlacedCard = dropzone.TopCard;
    //         GameManager.instance.ComboSurge --;

    //         // Reset UI or other visual elements if necessary
    //         CalculateScore();
    //     }
    // }
}
