using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalInformation : MonoBehaviour
{
    public static GlobalInformation instance; // Singleton instance to allow global access to this class

    public TimeOfDay timeOfDay; // Holds the current time of day, represented by the TimeOfDay enum

    public List<int> statlist = new List<int>(); // List of stats, where each index corresponds to a stat (e.g., Charisma, Courage)

    [SerializeField] private int numStats = 4; // The number of stats used in the game

    void Awake()
    {
        // Ensure only one instance of GlobalInformation exists
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        // Initialize stats to 0
        for (int i = 0; i < numStats; i++)
        {
            statlist.Add(0);
        }
    }

    // Resets the time of day to Morning
    public void NextDay()
    {
        timeOfDay = TimeOfDay.Morning;
    }

    // Progresses through time (Morning -> Afternoon -> Night -> Morning)
    public void ProgressTimeOfDay()
    {
        switch (timeOfDay)
        {
            case TimeOfDay.Morning:
                timeOfDay = TimeOfDay.Afternoon;
                break;
            case TimeOfDay.Afternoon:
                timeOfDay = TimeOfDay.Night;
                break;
            case TimeOfDay.Night:
                timeOfDay = TimeOfDay.Morning;
                break;
        }
    }
}

// Enum representing the different times of day
public enum TimeOfDay
{
    Morning,
    Afternoon,
    Night
}

// Enum representing different stats (though it's not currently used in this script)
public enum Stats
{
    Charisma,
    Courage,
    Cleverness,
    Creativity
}

