using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MapScript : MonoBehaviour
{
    // Information and name for the map location
    public string locInfo;
    public string locName;

    public bool useMapManager;

    public MapLocationsManager manager; // Reference to the MapLocationsManager

    private void Awake()
    {
        if (StressBar.instance != null)
        {
            StressBar.instance.updateStressBar();
        }
    }
    private void Start()
    {
        
    }
    // OnSelect is triggered when the map location is selected
    public void OnSelect()
    {
        // Delegate the selection logic to the manager, passing location info and name
        if (manager != null || !useMapManager)  // Check if the manager reference is assigned
        {
            if (StressManager.instance != null)
            {
                StressManager.instance.addToCurrentStress();
            }
            if (StressBar.instance != null)
            {
                StressBar.instance.updateStressBar();
            }

            SceneManager.LoadScene(locName);
            this.gameObject.SetActive(false);

            if (useMapManager)
            {
                manager.LocationSelect(locInfo, locName);
            }
        }
        else
        {
            Debug.LogWarning("MapLocationsManager reference is missing on " + gameObject.name);
        }
    }
}