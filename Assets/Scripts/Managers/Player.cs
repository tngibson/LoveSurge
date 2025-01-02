using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    private string playerName;

    // Define an enum to represent the stat names
    public enum StatType { Charisma, Cleverness, Courage, Creativity }

    // Use a list to store stat values
    public List<int> stats = new List<int>();
    public List<StatOffset> statOffsets = new List<StatOffset>();

    public static Player instance;
    public int cash = 0;

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
    }

    private void Start()
    {
        if (stats.Count <= 0)
        {
            // Initialize the list with zero values for all stats
            for (int i = 0; i < System.Enum.GetValues(typeof(StatType)).Length; i++)
            {
                stats.Add(0);
                statOffsets.Add(new StatOffset(0));
            }
        }

        StressManager.stressFilledEvent += OnStressFilled;
        StressManager.stressUnfilledEvent += OnStressUnfilled;
    }

    private void OnDestroy()
    {
        StressManager.stressFilledEvent -= OnStressFilled;
        StressManager.stressUnfilledEvent -= OnStressUnfilled;
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
    }
    
    // Returns stat value with associated offset
    public int GetStat(StatType stat)
    {
        return stats[(int)stat] + statOffsets[(int)stat].GetAmount(); // Access the value using the enum index
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
}

[Serializable]
public struct StatOffset
{
    public static readonly string STRESS_FOUR = "Stress-4";

    [SerializeField] private List<string> affectingTags;
    private Dictionary<string, Func<int, int>> tagMapping;
    private int amount;
    private bool isDirty;

    public StatOffset(int _amount)
    {
        amount = _amount;
        affectingTags = new List<string>();
        isDirty = false;
        tagMapping = new Dictionary<string, Func<int, int>>()
        {
            {STRESS_FOUR, (input) => input - 4}
        };
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

    public int GetAmount()
    {
        if (!isDirty) return amount;

        int finalAmount = 0;

        foreach (var tag in affectingTags)
        {
            if(tagMapping.ContainsKey(tag)) finalAmount = tagMapping[tag](finalAmount);
        }

        amount = finalAmount;
        isDirty = false;
        return amount;
    }
}
