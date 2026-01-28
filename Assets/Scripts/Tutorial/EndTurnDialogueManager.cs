using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EndTurnDialogueManager : MonoBehaviour
{
    private int turnCount = 0;
    [SerializeField] private PlayTutorialDialog firstTurnDialog;
    [SerializeField] private PlayTutorialDialog firstTurnFailureDialog;
    [SerializeField] private PlayTutorialDialog secondTurnDialog;
    [SerializeField] private PlayTutorialDialog secondTurnFailureDialog;
    [SerializeField] private PlayTutorialDialog thirdTurnDialog;
    
    private void Start()
    {
        turnCount = 0;
    }

    public void AdvanceTurn()
    {
        PlayerArea playerArea = FindObjectOfType<PlayerArea>();


        turnCount++;
        if (turnCount == 1)
        {
            if (playerArea.CardsInHand.Count != 0)
            {
                firstTurnFailureDialog.PlayDialog();
                return;
            }

            firstTurnFailureDialog.PlayDialog();
        }
        else if (turnCount == 2)
        {
            if (playerArea.CardsInHand.Count != 0)
            {
                secondTurnFailureDialog.PlayDialog();
                return;
            }

            secondTurnDialog.PlayDialog();
        }
        else if (turnCount == 3)
        {
            thirdTurnDialog.PlayDialog();
        }
    }
}
