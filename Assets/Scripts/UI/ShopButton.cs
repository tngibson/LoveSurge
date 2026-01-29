using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Item shopItem;
    [SerializeField] private Sprite unselected;
    [SerializeField] private Sprite selected;
    [SerializeField] private Sprite soldOut;
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
            itemInstance.transform.localScale = new Vector3(0.5f, 0.5f, 1);
            itemInstance.transform.localPosition = Vector3.zero;
            shopItem = value; 
        }
    }
    private void Awake()
    {
        buttonImage = GetComponent<Image>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Player.instance.cash -= shopItem.cost;
        isSoldOut = true;
        buttonImage.sprite = soldOut;
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
