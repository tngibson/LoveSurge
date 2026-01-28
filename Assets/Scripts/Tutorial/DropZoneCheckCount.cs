using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropZoneCheckCount : MonoBehaviour
{
    [SerializeField] private DropzoneSlot dropzoneSlot;
    [SerializeField] private PlayerArea playerArea;
    [SerializeField] private int requiredCardCount = 4;
    [SerializeField] private PlayTutorialDialog playTutorialDialogSuccess;
    [SerializeField] private PlayTutorialDialog playTutorialDialogFailure;

    void Update()
    {
        if (dropzoneSlot.GetCards().Count == requiredCardCount)
        {
            playTutorialDialogSuccess.PlayDialog();
            enabled = false;
        }
    }

    public void PlayFailureDialog()
    {
        if (dropzoneSlot.GetCards().Count != requiredCardCount)
        {
            playTutorialDialogFailure.PlayDialog();
            for(int i = 0; i < playerArea.CardsInHand.Count; i++)
            {
                playerArea.CardsInHand[i].GetComponent<DragDrop>().DiscardCard();
            }
            
        }
    }   
}
