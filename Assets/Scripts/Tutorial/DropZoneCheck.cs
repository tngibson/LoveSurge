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
}
