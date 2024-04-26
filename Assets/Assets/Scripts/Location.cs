using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour
{
    public void IncreaseCharStat()
    {
        GlobalInformation.instance.statlist[(int)Stats.Charisma] +=1;
        GlobalInformation.instance.ProgressTimeOfDay();
        Leave();
    }

    public void IncreaseCourStat()
    {
        GlobalInformation.instance.statlist[(int)Stats.Courage] +=1;
        GlobalInformation.instance.ProgressTimeOfDay();
        Leave();
    }

    public void IncreaseClevStat()
    {
        GlobalInformation.instance.statlist[(int)Stats.Cleverness] +=1;
        GlobalInformation.instance.ProgressTimeOfDay();
        Leave();
    }

    public void IncreaseCreaStat()
    {
        GlobalInformation.instance.statlist[(int)Stats.Creativity] +=1;
        GlobalInformation.instance.ProgressTimeOfDay();
        Leave();
    }

    public void Leave()
    {
        gameObject.SetActive(false);
    }

}
