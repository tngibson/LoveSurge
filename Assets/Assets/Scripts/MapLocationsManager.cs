using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class MapLocationsManager : MonoBehaviour
{
    public TextMeshProUGUI infoDisplay;
    public LocationWindowManager windowObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LocationSelect(string text, string location)
    {
        windowObject.gameObject.SetActive(true);
        infoDisplay.text = text;
        windowObject.activeName = location;
    }
}
