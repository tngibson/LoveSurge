using UnityEngine;

public class Location : MonoBehaviour
{
    // Increases the specified stat and progresses the time of day
    public void IncreaseStat(Stats stat)
    {
        GlobalInformation.instance.statlist[(int)stat] += 1;  // Increase the selected stat
        GlobalInformation.instance.ProgressTimeOfDay();       // Progress time of day after increasing the stat
        Leave();                                              // Close the location
    }

    // Leaves the location and deactivates the object
    public void Leave()
    {
        gameObject.SetActive(false);
    }

    // Methods for increasing specific stats
    public void IncreaseCharStat() { IncreaseStat(Stats.Charisma); }
    public void IncreaseCourStat() { IncreaseStat(Stats.Courage); }
    public void IncreaseClevStat() { IncreaseStat(Stats.Cleverness); }
    public void IncreaseCreaStat() { IncreaseStat(Stats.Creativity); }
}