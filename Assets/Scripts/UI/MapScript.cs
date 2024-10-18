using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MapScript : MonoBehaviour
{
    // Information and name for the map location
    public string locInfo;
    public string locName;

    public MapLocationsManager manager; // Reference to the MapLocationsManager
    private StressBar SB;
    private void Start()
    {
        SB = GetComponent<StressBar>();
    }
    // OnSelect is triggered when the map location is selected
    public void OnSelect()
    {
        SB.addToCurrentStress();
        SB.updateStressBar();
        // Delegate the selection logic to the manager, passing location info and name
        if (manager != null)  // Check if the manager reference is assigned
        {
            SceneManager.LoadScene(sceneName:locName);
            this.gameObject.SetActive(false);
            manager.LocationSelect(locInfo, locName);
        }
        else
        {
            Debug.LogWarning("MapLocationsManager reference is missing on " + gameObject.name);
        }
    }
}