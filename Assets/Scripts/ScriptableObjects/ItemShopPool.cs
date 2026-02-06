using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemShopPool", menuName = "ItemShopPool")]
public class ItemShopPool:ScriptableObject
{
    public List<Item> shopItems;
}
