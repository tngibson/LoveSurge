using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private string playerName;

    // order of stats is Charisma, Cleverness, Creativity, Courage
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

       for (int i = 0; i <4; i++)
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

    public void setStats(int id, int stat)
    {
        stats[id] = stat;
    }
    
    public List<int> GetStats()
    {
        return stats;
    }
}
