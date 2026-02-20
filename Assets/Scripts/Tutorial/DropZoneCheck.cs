using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropZoneCheck : MonoBehaviour
{
    [SerializeField] private Dropzone dropzone;
    [SerializeField] private PlayTutorialDialog playTutorialDialogSuccess;
    [SerializeField] private PlayTutorialDialog playTutorialDialogFailure;

    void Update()
    {
        if (dropzone.GetCardsToScore().Count == 1 && 
            dropzone.GetCardsToScore()[0].Power == 1 && 
            dropzone.GetCardsToScore()[0].Type.Equals("Cou"))
        {
            playTutorialDialogSuccess.PlayDialog();
            enabled = false;
        }
        else if (dropzone.GetCardsToScore().Count == 1)
        {
            playTutorialDialogFailure.PlayDialog();
            dropzone.RemoveCardFromDropzone();
        }
    }
}
