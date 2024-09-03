using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class MapLocationsManager : MonoBehaviour
{
    public TextMeshProUGUI infoDisplay;
    public LocationWindowManager windowObject;

    public void Start()
    {
        infoDisplay.text = "Stats: \n ";
    }

    public void LocationSelect(string text, string location)
    {
        windowObject.gameObject.SetActive(true);
        infoDisplay.text = text;
        windowObject.activeName = location;
    }
}
