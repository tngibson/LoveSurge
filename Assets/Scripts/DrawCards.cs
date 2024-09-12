using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCards : MonoBehaviour
{
    [SerializeField] GameObject card1;
    [SerializeField] GameObject card2;
    [SerializeField] GameObject card3;
    [SerializeField] GameObject card4;
    [SerializeField] PlayerDeckScript deckContainer;
    [SerializeField] PlayerArea playerArea;
    private void Start()
    {
    }
    public void onClick()
    {
        for (int i = 0; i < 5 ; i++)
        {
            Card card = deckContainer.draw();
            deckContainer.removeCard(card);
            card.transform.SetParent(playerArea.transform);
            playerArea.AddCards(card);

        }
        this.gameObject.SetActive(false);
    }
}
