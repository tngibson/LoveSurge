using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour, ISaveable
{
    [SerializeField] private string playerName;

    // Define an enum to represent the stat names
    public enum StatType { Charisma, Cleverness, Courage, Creativity }

    // Use a list to store stat values
    public List<int> stats = new List<int>();
    public List<StatOffset> statOffsets = new List<StatOffset>();
    public static Player instance;
    public int cash = 0;
    private List<string> ignoredTags;

    public List<int> convoTiers = new List<int> { 1, 1, 1, 1 }; // Index 0 = Courage, 1 = Creativity, 2 = Cleverness, 3 = Charisma
    public List<Card> collectedCards = new();
    public List<GameItem> collectedItems = new();

    public event Action OnStatsChanged;

    public int lastCharacterCompleted;

    [Header("Choice Booleans")]
    public bool isHouseHot = true;
    public bool isCelciThreatened = false;
    public bool nokiRomanticRoute = false;
    public bool celciRomanticRoute = false;
    public bool lotteRomanticRoute = false;
    public bool MainEvent1dot5Branch1 = false;
    public bool MainEvent1dot5Branch2 = false;

    public string SaveID => "Player";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (stats.Count <= 0)
        {
            // Initialize the list with zero values for all stats
            for (int i = 0; i < System.Enum.GetValues(typeof(StatType)).Length; i++)
            {
                stats.Add(0);
                statOffsets.Add(new StatOffset(0));
            }
        }

        ignoredTags = new List<string>()
        {
            PlayerDeckScript.STRESS_THRESH_2, PlayerDeckScript.STRESS_THRESH_3
        };
    }

    private void Start()
    {
        StressManager.stressFilledEvent += OnStressFilled;
        StressManager.stressUnfilledEvent += OnStressUnfilled;
        StressManager.stressChangedEvent += OnStressChanged;
    }

    private void OnDestroy()
    {
        StressManager.stressFilledEvent -= OnStressFilled;
        StressManager.stressUnfilledEvent -= OnStressUnfilled;
        StressManager.stressChangedEvent -= OnStressChanged;
    }

    public void SetName(string name)
    {
        playerName = name;
    }

    public string GetName()
    {
        return playerName;
    }

    public void SetStat(StatType stat, int value)
    {
        stats[(int)stat] = value; // Use the enum as an index
        OnStatsChanged?.Invoke();
    }
    
    // Returns stat value with associated offset
    public int GetStat(StatType stat)
    {
        return stats[(int)stat] + statOffsets[(int)stat].GetAmount(ignoredTags); // Access the value using the enum index
    }

    // Returns stat value without offsets
    public int GetRawStat(StatType stat)
    {
        return stats[(int)stat]; // Access the value using the enum index
    }

    public List<int> GetStats()
    {
        return new List<int>(stats); // Return a copy of the list for safety
    }

    // Will always return a list of stat offsets, even if Player is null
    // (the list will still be the correct length even in that case)
    public static List<StatOffset> GetSafeOffsets()
    {
        if (instance == null)
        {
            List<StatOffset> newList = new List<StatOffset>(System.Enum.GetValues(typeof(StatType)).Length);
            for (int i = 0; i < System.Enum.GetValues(typeof(StatType)).Length; i++)
            {
                newList.Add(new StatOffset(0));
            }
            Debug.Log("Returning empty stat offset list because Player was null!");
            return newList;
        }

        return new List<StatOffset>(instance.statOffsets);
    }

    private void OnStressFilled(object sender, EventArgs args)
    {
        int index = Random.Range(0, System.Enum.GetValues(typeof(StatType)).Length);
        statOffsets[index].AddOffsetTag(StatOffset.STRESS_FOUR);
    }

    private void OnStressUnfilled(object sender, EventArgs args)
    {
        foreach (var offset in statOffsets)
        {
            if (offset.HasOffsetTag(StatOffset.STRESS_FOUR))
            {
                offset.RemoveOffsetTag(StatOffset.STRESS_FOUR);
                break;
            }
        }
    }

    private void OnStressChanged(object sender, StressEventArgs e)
    {
        int barsFilled = StressManager.GetStressBarsFilled(StressManager.instance.currentStressAmt);
        foreach (var statOffset in statOffsets)
        {
            if (statOffset.HasOffsetTag(PlayerDeckScript.STRESS_THRESH_2) && barsFilled < 2)
                statOffset.RemoveOffsetTag(PlayerDeckScript.STRESS_THRESH_2);
            if (statOffset.HasOffsetTag(PlayerDeckScript.STRESS_THRESH_3) && barsFilled < 3)
                statOffset.RemoveOffsetTag(PlayerDeckScript.STRESS_THRESH_3);
        }
    }

    public void SetConvoTiers(string convoAttribute, int tier)
    {
        switch (convoAttribute.ToLower())
        {
            case "cha":
            case "charisma":
                convoTiers[3] = tier;
                break;
            case "cre":
            case "creativity":
                convoTiers[1] = tier;
                break;
            case "cou":
            case "courage":
                convoTiers[0] = tier;
                break;
            case "cle":
            case "cleverness":
                convoTiers[2] = tier;
                break;
            default:
                Debug.LogWarning("Unknown convoAttribute: " + convoAttribute);
                break;
        }
    }

    public string CaptureState()
    {
        var offsetTags = new List<List<string>>();
        foreach (var offset in statOffsets)
        {
            offsetTags.Add(offset.GetTags());
        }

        var savedItems = new List<SavedItem>();
        foreach (var item in collectedItems)
        {
            savedItems.Add(new SavedItem
            {
                id = item.ItemID,
                socketIndex = item.SocketIndex
            });
        }

        var savedCards = new List<SavedCard>();
        foreach (var card in collectedCards)
        {
            savedCards.Add(new SavedCard
            {
                id = card.CardID,
                power = card.Power
            });
        }

        PlayerSaveData data = new PlayerSaveData
        {
            playerName = playerName,
            stats = new List<int>(stats),
            statOffsetTags = offsetTags,
            cash = cash,
            convoTiers = new List<int>(convoTiers),

            lastCharacterCompleted = lastCharacterCompleted,

            isHouseHot = isHouseHot,
            isCelciThreatened = isCelciThreatened,
            nokiRomanticRoute = nokiRomanticRoute,
            celciRomanticRoute = celciRomanticRoute,
            lotteRomanticRoute = lotteRomanticRoute,
            MainEvent1dot5Branch1 = MainEvent1dot5Branch1,
            MainEvent1dot5Branch2 = MainEvent1dot5Branch2,

            collectedItems = savedItems,
            collectedCards = savedCards
        };

        return JsonUtility.ToJson(data);
    }

    public void RestoreState(string json)
    {
        Debug.Log("Player RestoreState called");

        PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);
        if (data == null) return;

        playerName = data.playerName;

        stats = data.stats ?? new List<int>();
        cash = data.cash;
        convoTiers = data.convoTiers ?? new List<int>();

        lastCharacterCompleted = data.lastCharacterCompleted;

        isHouseHot = data.isHouseHot;
        isCelciThreatened = data.isCelciThreatened;
        nokiRomanticRoute = data.nokiRomanticRoute;
        celciRomanticRoute = data.celciRomanticRoute;
        lotteRomanticRoute = data.lotteRomanticRoute;
        MainEvent1dot5Branch1 = data.MainEvent1dot5Branch1;
        MainEvent1dot5Branch2 = data.MainEvent1dot5Branch2;

        // ---------- Restore Stat Offsets Safely ----------
        statOffsets = new List<StatOffset>();

        int statCount = Enum.GetValues(typeof(StatType)).Length;

        for (int i = 0; i < statCount; i++)
        {
            var offset = new StatOffset(0);

            if (data.statOffsetTags != null &&
                i < data.statOffsetTags.Count)
            {
                foreach (var tag in data.statOffsetTags[i])
                    offset.AddOffsetTag(tag);
            }

            statOffsets.Add(offset);
        }

        // ---------- Restore Items ----------
        collectedItems.Clear();

        if (data.collectedItems != null)
        {
            foreach (var saved in data.collectedItems)
            {
                var item = ItemDatabase.Instance.Spawn(saved.id);
                if (item == null) continue;

                item.SocketIndex = saved.socketIndex;
                CollectItem(item);
            }
        }

        // ---------- Restore Cards ----------
        collectedCards.Clear();

        if (data.collectedCards != null)
        {
            foreach (var saved in data.collectedCards)
            {
                var card = CardDatabase.Instance.Spawn(saved.id);
                if (card == null) continue;

                card.Power = saved.power;
                CollectCard(card);
            }
        }

        OnStatsChanged?.Invoke();
    }

    public void CollectItem(GameItem item)
    {
        if (collectedItems.Count > 4)
        {
            Debug.LogWarning("Cannot collect more than 5 items!");
            return;
        }
        item.transform.SetParent(this.transform);
        item.SocketIndex = collectedItems.Count;
        collectedItems.Add(item);
    }

    public void CollectCard(Card card)
    {
        card.transform.SetParent(this.transform);
        collectedCards.Add(card);
    }

    public void DeleteItem(GameItem item)
    {
        if (collectedItems.Contains(item))
        {
            collectedItems.Remove(item);
            Destroy(item.gameObject);
        }
    }

    public void DeleteCard(Card card)
    {
        if (collectedCards.Contains(card))
        {
            collectedCards.Remove(card);
            Destroy(card.gameObject);
        }
    }

    public void ReturnItem()
    {
        for(int i = 0; i < collectedItems.Count; i++)
        {
            collectedItems[i].transform.SetParent(this.transform);
            collectedItems[i].gameObject.SetActive(false);
        }
    }
}

[Serializable]
public class StatOffset
{
    public static readonly string STRESS_FOUR = "Stress-4";

    [SerializeField] private List<string> affectingTags;

    private static Dictionary<string, Func<int, int>> tagMapping = new Dictionary<string, Func<int, int>>()
    {
        {STRESS_FOUR, input => input - 4},
        {PlayerDeckScript.STRESS_THRESH_2, input => input - 1},
        {PlayerDeckScript.STRESS_THRESH_3, input => input - 1}
    };
    private int amount;
    private bool isDirty;

    public StatOffset(int _amount)
    {
        amount = _amount;
        affectingTags = new List<string>();
        isDirty = false;
    }

    public void AddOffsetTag(string tag)
    {
        affectingTags.Add(tag);
        isDirty = true;
    }

    public bool RemoveOffsetTag(string tag)
    {
        bool success = affectingTags.Remove(tag);
        if (success) isDirty = true;

        return success;
    }

    public bool HasOffsetTag(string tag)
    {
        return affectingTags.Contains(tag);
    }

    public int GetAmount(List<string> ignoreTags = null)
    {
        // TODO: This makes it always return 0 for some reason
        //if (!isDirty) return amount;

        int finalAmount = 0;

        foreach (var tag in affectingTags)
        {
            // ignoreTags allows you to conditionally ignore certain tags
            bool ignoreTag = ignoreTags?.Contains(tag) ?? false;
            if (tagMapping.ContainsKey(tag) && !ignoreTag) 
                finalAmount = tagMapping[tag](finalAmount);
        }

        amount = finalAmount;
        isDirty = false;
        return amount;
    }

    public List<string> GetTags()
    {
        return new List<string>(affectingTags);
    }
}

[System.Serializable]
public class PlayerSaveData
{
    public string playerName;

    public List<int> stats;
    public List<List<string>> statOffsetTags;

    public int cash;
    public List<int> convoTiers;

    public int lastCharacterCompleted;

    // Choice flags
    public bool isHouseHot;
    public bool isCelciThreatened;
    public bool nokiRomanticRoute;
    public bool celciRomanticRoute;
    public bool lotteRomanticRoute;
    public bool MainEvent1dot5Branch1;
    public bool MainEvent1dot5Branch2;

    // Inventory
    public List<string> collectedItemIDs;
    public List<string> collectedCardIDs;
    public List<SavedItem> collectedItems;
    public List<SavedCard> collectedCards;
}

[System.Serializable]
public class SavedItem
{
    public string id;
    public int socketIndex;
}

[System.Serializable]
public class SavedCard
{
    public string id;
    public int power;
}
