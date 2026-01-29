using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private ShopButton[] shopButtons;
    [SerializeField] private CardShopPool cardShopPool;
    [SerializeField] private ItemShopPool itemShopPool;
    private List<string> currentShopItems = new List<string>();

    private void Start()
    {
        PopulateShop();
    }

    private void PopulateShop()
    {
        currentShopItems.Clear();
        shopButtons[0].Item = GetRandomItemFromPool(cardShopPool.shopItems);
        shopButtons[1].Item = GetRandomItemFromPool(cardShopPool.shopItems);
        shopButtons[2].Item = GetRandomItemFromPool(itemShopPool.shopItems);
        shopButtons[3].Item = GetRandomItemFromPool(itemShopPool.shopItems);
    }

    private Item GetRandomItemFromPool(List<Item> pool)
    {
        Item selectedItem;
        selectedItem = pool[Random.Range(0, pool.Count)]; 

        // Ensure the selected item is not already in the shop
        while (currentShopItems.Contains(selectedItem.itemPrefab.name))
        {
            selectedItem = pool[Random.Range(0, pool.Count)];
        }

        currentShopItems.Add(selectedItem.itemPrefab.name);
        return selectedItem;
    }
}

[CreateAssetMenu(fileName = "CardShopPool", menuName = "CardShopPool")]
public class CardShopPool:ScriptableObject
{
    public List<Item> shopItems;
}

[CreateAssetMenu(fileName = "ItemShopPool", menuName = "ItemShopPool")]
public class ItemShopPool:ScriptableObject
{
    public List<Item> shopItems;
}

[System.Serializable]
public class Item
{
    public GameObject itemPrefab;
    public int cost;
}
