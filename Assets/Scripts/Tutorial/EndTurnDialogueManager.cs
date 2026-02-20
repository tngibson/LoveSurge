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
    [SerializeField] private Card StressCardPrefab;
    
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
            if (playerArea.CardsInHand.Count > 0)
            {
                DiscardHand();
                firstTurnFailureDialog.PlayDialog();
                return;
            }

            firstTurnDialog.PlayDialog();
        }
        else if (turnCount == 2)
        {
            if (playerArea.CardsInHand.Count > 0)
            {
                DiscardHand();
                secondTurnFailureDialog.PlayDialog();
                return;
            }

            secondTurnDialog.PlayDialog();
        }
        else if (turnCount == 3)
        {
            DiscardHand();
            thirdTurnDialog.PlayDialog();
            PlayerDeckScriptTutorial playerDeck = FindObjectOfType<PlayerDeckScriptTutorial>();
            for (int i = 0; i < 2; i++)
            {
                Card stresscard = playerDeck.MakeCard(StressCardPrefab,0);
                playerDeck.Deck.Insert(0, stresscard);
            }
        }
    }

    private void DiscardHand()
    {
        PlayerArea playerArea = FindObjectOfType<PlayerArea>();
        DiscardPile discardPile = FindObjectOfType<DiscardPile>();
        for(int i = 0; i < playerArea.CardsInHand.Count; i++)
        {
            playerArea.CardsInHand[i].GetComponent<DragDrop>().CurrentDiscardPile = discardPile;
            playerArea.CardsInHand[i].GetComponent<DragDrop>().DiscardCard();
        }
    }
}
