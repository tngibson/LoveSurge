using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedroomManager : MonoBehaviour
{
    public void YesSleep()
    {
        gameObject.SetActive(false);
        GlobalInformation.instance.ProgressTimeOfDay();
    }

    public void NoSleep()
    {
        gameObject.SetActive(false);
    }

}
