using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialButtonManager : MonoBehaviour
{
    [SerializeField] GameObject radialButtonOn;
    [SerializeField] GameObject radialButtonOff;

    private bool isOn = false;

    public void OnButtonPress()
    {
        if (isOn)
        {
            radialButtonOff.SetActive(true);
            radialButtonOn.SetActive(false);
            isOn = false;
        }
        else
        {
            radialButtonOn.SetActive(true);
            radialButtonOff.SetActive(false);
            isOn = true;
        }
    }
}
