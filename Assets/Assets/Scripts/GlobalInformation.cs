using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalInformation : MonoBehaviour
{
    public static GlobalInformation instance;
    public TimeOfDay timeOfDay;

    public List<int> statlist = new List<int>();

    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        statlist.Add(0);
        statlist.Add(0);
        statlist.Add(0);
        statlist.Add(0);
    }

    public void NextDay()
    {
        timeOfDay = TimeOfDay.Morning;
    }

    public void ProgressTimeOfDay()
    {
        if (timeOfDay == TimeOfDay.Morning)
        {
            timeOfDay = TimeOfDay.Afternoon;
        }
        else if (timeOfDay == TimeOfDay.Afternoon)
        {
            timeOfDay = TimeOfDay.Night;
        }
        else if (timeOfDay == TimeOfDay.Night)
        {
            timeOfDay = TimeOfDay.Morning;
        }
    }
}

public enum TimeOfDay
{
    Morning,
    Afternoon,
    Night
}

public enum Stats
{
    Charisma,
    Courage,
    Cleverness,
    Creativity
}
