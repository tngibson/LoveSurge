using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscardPile : MonoBehaviour
{
    List<Card> pile;
    public List<Card> Pile { get { return pile; } }
    // Start is called before the first frame update
    void Start()
    {
        pile = new List<Card>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void addToDiscard(Card played)
    {
        pile.Add(played);

        // This makes it so that the card containers are no longer see-through
        played.SetImageAlpha(1f);
        // This makes it so that the cards can no longer be moved or swapped when they've been discarded
        Destroy(played.GetComponent<DragDrop>());
        Destroy(played.GetComponent<GridElementSwapper>());
    }
}
