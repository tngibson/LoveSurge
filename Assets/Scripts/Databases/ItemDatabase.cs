using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    [SerializeField] private List<GameItem> itemPrefabs;

    private Dictionary<string, GameItem> lookup;

    void Awake()
    {
        Instance = this;

        lookup = new Dictionary<string, GameItem>();
        foreach (var item in itemPrefabs)
        {
            lookup[item.ItemID] = item;
        }
    }

    public GameItem Spawn(string id)
    {
        if (!lookup.ContainsKey(id)) return null;
        return Instantiate(lookup[id]);
    }
}