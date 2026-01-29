using UnityEngine;
public class GameItem : MonoBehaviour
{
    [SerializeField] private string description;
    [SerializeField] private int drawAmount;
    [SerializeField] private int discardAmount;
    [SerializeField] private bool redrawHand;

    [Header("Score Boost")]
    [SerializeField] private string type;
    [SerializeField] private string operation;
    [SerializeField] private int scoreboost;

    // Add properties and methods relevant to game items here
    public virtual void UseItem()
    {
        Player.instance.DeleteItem(this);
        if (redrawHand) RedrawHand();
        Discard(discardAmount);

        PlayerDeckScript deck = Player.instance.GetComponent<PlayerDeckScript>();
        if(deck != null) deck.DrawCards(drawAmount);

        Dropzone dropzone = FindObjectOfType<Dropzone>();
        if(dropzone != null) dropzone.ApplyBonus(type, scoreboost, operation);
    }

    public void Discard(int amount)
    {
        PlayerArea playerArea = FindObjectOfType<PlayerArea>();
        DiscardPile discardPile = FindObjectOfType<DiscardPile>();
        for(int i = amount; i > 0; i--)
        {
            if(i >= playerArea.CardsInHand.Count) break;
            playerArea.CardsInHand[i].GetComponent<DragDrop>().CurrentDiscardPile = discardPile;
            playerArea.CardsInHand[i].GetComponent<DragDrop>().DiscardCard();
        }
    }

    public void RedrawHand()
    {
        PlayerArea playerArea = FindObjectOfType<PlayerArea>();
        DiscardPile discardPile = FindObjectOfType<DiscardPile>();
        for(int i = 0; i < playerArea.CardsInHand.Count; i++)
        {
            playerArea.CardsInHand[i].GetComponent<DragDrop>().CurrentDiscardPile = discardPile;
            playerArea.CardsInHand[i].GetComponent<DragDrop>().DiscardCard();
        }

        PlayerDeckScript deck = Player.instance.GetComponent<PlayerDeckScript>();
        if(deck != null) deck.DrawCards(playerArea.CardsInHand.Count);
    } 
}