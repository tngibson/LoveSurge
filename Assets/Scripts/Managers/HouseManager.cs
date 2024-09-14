using UnityEngine;

public class HouseManager : MonoBehaviour
{
    // References to different containers
    [SerializeField] private GameObject houseContainer;
    [SerializeField] private GameObject bedroomContainer;
    [SerializeField] private GameObject lotteContainer;

    // Handles when the front door is interacted with
    public void OnFrontDoor()
    {
        Clear();  // Ensure all containers are hidden before performing actions
        houseContainer.SetActive(false);
    }

    // Handles when the bedroom is selected
    public void OnBedroom()
    {
        Clear();  // Ensure all containers are hidden before activating the bedroom
        bedroomContainer.SetActive(true);
    }

    // Handles when the Lotte container is activated
    public void Lotte()
    {
        Clear();  // Ensure all containers are hidden before activating Lotte
        lotteContainer.SetActive(true);
    }

    // Clears all containers (sets them inactive)
    private void Clear()
    {
        houseContainer.SetActive(false);
        bedroomContainer.SetActive(false);
        lotteContainer.SetActive(false);
    }
}

