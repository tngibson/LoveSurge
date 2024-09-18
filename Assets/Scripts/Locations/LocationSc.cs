using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LocationScene : MonoBehaviour
{
    [SerializeField] private List<string> randomLocations; // List of possible random event locations

    private bool dialogCompleted = false; // Flag to track if dialog has been completed

    [SerializeField] private float randomEventChance = 0.1f; // Chance for random event to occur

    // Changes to the chosen location, possibly after a random event
    public void ChangeScene(string sceneName)
    {
        // Check if a random event should occur
        if (RandomEventTrigger())
        {
            // Choose a random location and start the dialog sequence
            string randomLocation = GetRandomLocation();
            if (randomLocation != null)
            {
                StartCoroutine(RandomEventSequence(randomLocation, sceneName));
            }
            else
            {
                // No random locations left, go directly to the chosen location
                ActivateLocation(sceneName);
            }
        }
        else
        {
            // No random event, go directly to the selected location
            ActivateLocation(sceneName);
        }
    }

    // Coroutine for the random event sequence and wait for dialog completion
    private IEnumerator RandomEventSequence(string randomLocation, string finalLocation)
    {
        // Go to the random event location first
        ActivateLocation(randomLocation);

        // Wait until the dialog is completed
        yield return new WaitUntil(() => dialogCompleted);

        // After dialog is finished, increase a random stat
        IncreaseRandomStat();

        // Then, go to the final chosen location
        ActivateLocation(finalLocation);
    }

    // Activates the specified location by name
    private void ActivateLocation(string locationName)
    {
        // Deactivate all other locations before activating the new one
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        // Activate the specified location
        GameObject sceneObject = transform.Find(locationName)?.gameObject;
        if (sceneObject != null)
        {
            sceneObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"Location '{locationName}' not found.");
        }
    }

    // Determines whether to trigger a random event (only if random locations are available)
    private bool RandomEventTrigger()
    {
        // Check if there are any random locations left
        if (randomLocations != null && randomLocations.Count > 0)
        {
            return Random.value > randomEventChance;  // 50% chance of a random event occurring
        }
        else
        {
            // No random locations left, disable random events
            return false;
        }
    }

    // Gets a random location from the list of random event locations, removes it from the list
    private string GetRandomLocation()
    {
        if (randomLocations != null && randomLocations.Count > 0)
        {
            int randomIndex = Random.Range(0, randomLocations.Count);
            string randomLocation = randomLocations[randomIndex];

            // Remove the selected location from the list to prevent it from being chosen again
            randomLocations.RemoveAt(randomIndex);

            return randomLocation;
        }
        else
        {
            Debug.LogWarning("No random locations left.");
            return null;  // Return null if no random locations are available
        }
    }

    // Call this method when all dialog is finished to proceed to the final location
    public void CompleteDialog()
    {
        dialogCompleted = true;  // Mark dialog as completed
    }

    // Reset the dialog completion flag (should be called at the start of each new event)
    public void ResetDialog()
    {
        dialogCompleted = false;  // Reset the flag before starting a new dialog sequence
    }

    // Increases a random stat from the available stats (Charisma, Courage, Cleverness, Creativity)
    private void IncreaseRandomStat()
    {
        Stats[] availableStats = { Stats.Charisma, Stats.Courage, Stats.Cleverness, Stats.Creativity };
        Stats randomStat = availableStats[Random.Range(0, availableStats.Length)];

        // Increase the selected stat
        GlobalInformation.instance.statlist[(int)randomStat] += 1;

        Debug.Log($"{randomStat} stat has been increased.");
    }
}
