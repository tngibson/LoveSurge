using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Vector3 itemOffset = new Vector3(0, -20f, 0);

    [Header("Sprites")]
    [SerializeField] private Item shopItem;
    [SerializeField] private Sprite unselected;
    [SerializeField] private Sprite selected;
    [SerializeField] private Sprite soldOut;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI purchaseText;
    [SerializeField] private const string PURCHASE = "Buy";
    [SerializeField] private const string SOLD_OUT = "Sold Out";
    private Image buttonImage;
    private GameObject itemInstance;
    private bool isSoldOut = false;
    public GameObject ItemInstance => itemInstance;
    public Item Item
    {
        get { return shopItem; }

        set 
        {
            itemInstance = Instantiate(value.itemPrefab, transform);
            itemInstance.transform.SetAsFirstSibling();
            itemInstance.TryGetComponent(out EventTrigger trigger);
            itemInstance.TryGetComponent(out DragDrop dragDrop);
            itemInstance.TryGetComponent(out GameItem gameItem);

            if (dragDrop != null) dragDrop.enabled = false;
            if (trigger != null) trigger.enabled = false;
            if (gameItem != null) itemInstance.GetComponentInChildren<Button>().enabled = false;


            itemInstance.transform.localScale = new Vector3(0.5f, 0.5f, 1);
            itemInstance.transform.localPosition = Vector3.zero + itemOffset;

            costText.text = $"${value.cost}";
            purchaseText.text = PURCHASE;
            shopItem = value; 
        }
    }
    private void Awake()
    {
        buttonImage = GetComponent<Image>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(!(Player.instance.cash - shopItem.cost >= 0) || isSoldOut) return;
        if(itemInstance.TryGetComponent(out GameItem gameItem) && Player.instance.collectedItems.Count >= 4)
        {
            //Debug.Log("Cannot purchase more items, inventory full.");
            return;
        }

        //Debug.Log("Purchased Item: " + shopItem.itemPrefab.name);
        Player.instance.cash -= shopItem.cost;
        isSoldOut = true;
        buttonImage.sprite = soldOut;
        itemInstance.SetActive(false);
        purchaseText.text = SOLD_OUT;
        purchaseText.color = Color.white;

        if(itemInstance.TryGetComponent(out Card card))
        {
            Card cardInstance = Instantiate(shopItem.itemPrefab.GetComponent<Card>());
            cardInstance.gameObject.SetActive(false);
            Player.instance.CollectCard(cardInstance);
        }
        else if(gameItem != null)
        {
            if(Player.instance.collectedItems.Count > 4) return;
            GameItem gameItemInstance = Instantiate(shopItem.itemPrefab.GetComponent<GameItem>());
            Player.instance.CollectItem(gameItemInstance);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isSoldOut) return;
        buttonImage.sprite = unselected;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSoldOut) return;
        buttonImage.sprite = selected;
    }
}
