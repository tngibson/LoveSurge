using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocationManager : MonoBehaviour
{
    public static LocationManager Instance { get; private set; }

    private List<bool> dateStates = new List<bool> { false, false, false };
    public bool dateStarted = false;

    [SerializeField] private MapScript targetMapScript;

    public bool isPlayable = true;
    public DayPhase phaseEnteredDate = DayPhase.None;
    public bool isFirstTime = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Subscribe to sceneLoaded event
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        targetMapScript = GameObject.Find("House")?.GetComponent<MapScript>();
        for (int i = 0; i < dateStates.Count; i++)
        {
            if (GetDateState(i))
            {
                UpdateMapLocation();
            }
        }

        if (isPlayable && targetMapScript != null && isFirstTime)
        {
            targetMapScript.UpdateLocationText("Go on a date with Noki!");
            targetMapScript.SetEnabled(true);
            phaseEnteredDate = CalendarManager.instance.currentPhase;
            isFirstTime = false;
        }
        else if (isPlayable && targetMapScript != null && !isFirstTime)
        {
            targetMapScript.UpdateLocationText("Continue your date with Noki!");
            targetMapScript.SetEnabled(true);
            phaseEnteredDate = CalendarManager.instance.currentPhase;
        }
        else if ((phaseEnteredDate != DayPhase.None && phaseEnteredDate == CalendarManager.instance.currentPhase) && targetMapScript != null)
        {
            targetMapScript.UpdateLocationText("Continue your date with Noki!");
            targetMapScript.SetEnabled(true);
            isPlayable = true;
            phaseEnteredDate = DayPhase.None;
        }
        else if (targetMapScript != null)
        {
            targetMapScript.UpdateLocationText("You must wait a full day to continue your date!");
            targetMapScript.SetEnabled(false);
        }
    }

    public bool GetDateState(int dateIndex)
    {
        if (dateIndex >= 0 && dateIndex < dateStates.Count)
        {
            return dateStates[dateIndex];
        }
        return false;
    }

    public void SetDateState(int dateIndex, bool state)
    {
        if (dateIndex >= 0 && dateIndex < dateStates.Count)
        {
            dateStates[dateIndex] = state;
            dateStarted = state; // Update dateStarted if any date is set

            if (dateStarted)
            {
                UpdateMapLocation();
            }
        }
    }

    private void UpdateMapLocation()
    {
        if (targetMapScript != null)
        {
            targetMapScript.locName = "NokiDate2CardGame1";
            Debug.Log("Location updated to: " + targetMapScript.locName);
        }
        else
        {
            Debug.LogWarning("Target MapScript not assigned or found!");
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

