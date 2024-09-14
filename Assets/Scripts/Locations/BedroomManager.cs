using UnityEngine;

public class BedroomManager : MonoBehaviour
{
    // Handles the action for when the player chooses to sleep
    public void YesSleep()
    {
        CloseBedroom();
        GlobalInformation.instance.ProgressTimeOfDay(); // Progress time of day when sleeping
    }

    // Handles the action for when the player chooses not to sleep
    public void NoSleep()
    {
        CloseBedroom();
    }

    // Method to close the bedroom UI
    private void CloseBedroom()
    {
        gameObject.SetActive(false); // Deactivates the bedroom UI
    }
}
