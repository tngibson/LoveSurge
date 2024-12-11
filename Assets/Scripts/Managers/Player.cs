using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private string playerName;

    // Define an enum to represent the stat names
    public enum StatType { Charisma, Cleverness, Courage, Creativity }

    // Use a list to store stat values
    public List<int> stats = new List<int>();

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
        // Initialize the list with zero values for all stats
        for (int i = 0; i < System.Enum.GetValues(typeof(StatType)).Length; i++)
        {
            stats.Add(0);
        }
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

    public int GetStat(StatType stat)
    {
        return stats[(int)stat]; // Access the value using the enum index
    }

    public List<int> GetStats()
    {
        return new List<int>(stats); // Return a copy of the list for safety
    }
}
