using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour
{
    public void IncreaseStat()
    {
        GlobalInformation.instance.statlist[(int)Stats.Charisma] +=1;
        GlobalInformation.instance.ProgressTimeOfDay();
        Leave();
    }

    public void Leave()
    {
        gameObject.SetActive(false);
    }

}
