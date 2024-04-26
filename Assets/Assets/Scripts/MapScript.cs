using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScript : MonoBehaviour
{
    public string locInfo;
    public string locName;
    public MapLocationsManager manager;



    public void OnSelect()
    {
        manager.LocationSelect(locInfo, locName);
    }

}
