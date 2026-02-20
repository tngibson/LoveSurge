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

    private int socketIndex;
    public string Description => description;
    public int SocketIndex { get { return socketIndex; } set { socketIndex = value; } }

    [SerializeField] public string itemID;

    public string ItemID => itemID;

    // Add properties and methods relevant to game items here
    public virtual void UseItem()
    {
        Player.instance.DeleteItem(this);
        if (redrawHand) RedrawHand();
        Discard(discardAmount);

        PlayerDeckScript deck = GameManager.instance?.DeckContainer;
        deck?.DrawCards(drawAmount);

        Dropzone dropzone = GameManager.instance?.Dropzone;
        dropzone?.ApplyBonus(type, scoreboost, operation);

        GameManager.instance.ItemCanvasInstance.TryGetComponent(out Socket socket);
        socket.ClearSocket(socketIndex);

        UnlockAchievement(AchievementID.NEW_ACHIEVEMENT_1_5);
    }

    public void Discard(int amount)
    {
        PlayerArea playerArea = GameManager.instance?.PlayerArea;
        DiscardPile discardPile = GameManager.instance?.DiscardPile;
        
        for(int i = amount; i > 0; i--)
        {
            if(i > playerArea.CardsInHand.Count) break;
            playerArea.CardsInHand[i - 1].GetComponent<DragDrop>().CurrentDiscardPile = discardPile;
            playerArea.CardsInHand[i - 1].GetComponent<DragDrop>().DiscardCard();
        }
    }

    public void RedrawHand()
    {
        PlayerArea playerArea = GameManager.instance?.PlayerArea;

        Discard(playerArea.CardsInHand.Count);

        PlayerDeckScript deck = GameManager.instance?.DeckContainer;
        deck?.DrawCards(4);
    }

    private void UnlockAchievement(AchievementID id)
    {
        if (AchievementComponent.AchievementSystem == null)
            return;

        AchievementComponent.AchievementSystem.UnlockAchievement(id);
    }
}