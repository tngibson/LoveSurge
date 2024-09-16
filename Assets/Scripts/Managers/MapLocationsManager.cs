using UnityEngine;
using TMPro;

public class MapLocationsManager : MonoBehaviour
{
    // Reference to the UI element for displaying location info
    [SerializeField] private TextMeshProUGUI infoDisplay;

    // Reference to the LocationWindowManager script
    [SerializeField] private LocationWindowManager windowObject;

    // Initializes the info display when the script starts
    private void Start()
    {
        infoDisplay.text = "Stats: \n";  // Initialize the info display with default text
    }

    // Method to handle selecting a location
    public void LocationSelect(string text, string location)
    {
        if (windowObject != null)
        {
            // Activate the location window and update the relevant information
            windowObject.gameObject.SetActive(true);
            infoDisplay.text = text;
            windowObject.activeName = location;
        }
        else
        {
            Debug.LogWarning("LocationWindowManager is not assigned.");
        }
    }
}