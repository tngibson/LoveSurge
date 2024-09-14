using UnityEngine;
using TMPro;

public class LocationWindowManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI infoDisplay; // Reference to the UI element displaying location information

    [SerializeField] public string activeName; // Name of the currently active location

    [SerializeField] private LocationScene locationScene; // Reference to the LocationScene

    // Handles the close action for the location window
    public void OnClose()
    {
        gameObject.SetActive(false);  // Deactivates the location window
    }

    // Handles the action of entering a new location
    public void OnEnter()
    {
        if (!string.IsNullOrEmpty(activeName) && locationScene != null)
        {
            // Changes the scene to the location specified by activeName
            locationScene.ChangeScene(activeName);
        }
        OnClose();  // Close the window after entering the location
    }
}
