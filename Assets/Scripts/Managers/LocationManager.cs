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

    public enum DateNum { Date1, Date2, Date3 };
    public enum Date1Stage { Intro, CardGame, Done };
    public enum Date2Stage { Intro, CardGame, Done };
    public enum Date3Stage { Intro, CardGame, Done };

    public DateNum date = DateNum.Date1;
    public Date1Stage date1Stage = Date1Stage.Intro;
    public Date2Stage date2Stage = Date2Stage.Intro;
    public Date3Stage date3Stage = Date3Stage.Intro;

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
        UpdateMapLocation();
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
        if (targetMapScript == null) return;

        switch (date)
        {
            case DateNum.Date1:
                switch (date1Stage)
                {
                    case Date1Stage.Intro:
                        targetMapScript.locName = "NokiDate1Intro";
                        break;
                    case Date1Stage.CardGame:
                        targetMapScript.locName = "NokiDate1CardGame1";
                        break;
                    case Date1Stage.Done:
                        date = DateNum.Date2;
                        date2Stage = Date2Stage.Intro;
                        UpdateMapLocation();
                        for (int i = 0; i < Player.instance.convoTiers.Count; i++)
                        {
                            Player.instance.convoTiers[i] = 1;
                        }
                        ConnectionManager.instance.setConnection(0, 0);
                        return;
                }
                break;

            case DateNum.Date2:
                switch (date2Stage)
                {
                    case Date2Stage.Intro:
                        targetMapScript.locName = "NokiDate2Intro";
                        break;
                    case Date2Stage.CardGame:
                        targetMapScript.locName = "NokiDate2CardGame1";
                        break;
                    case Date2Stage.Done:
                        date = DateNum.Date3;
                        date3Stage = Date3Stage.Intro;
                        UpdateMapLocation();
                        for (int i = 0; i < Player.instance.convoTiers.Count; i++)
                        {
                            Player.instance.convoTiers[i] = 1;
                        }
                        ConnectionManager.instance.setConnection(0, 0);
                        return;
                }
                break;

            case DateNum.Date3:
                switch (date3Stage)
                {
                    case Date3Stage.Intro:
                        targetMapScript.locName = "NokiDate3Intro";
                        break;
                    case Date3Stage.CardGame:
                        targetMapScript.locName = "NokiDate3CardGame1";
                        break;
                    case Date3Stage.Done:
                        Debug.Log("All dates completed.");
                        break;
                }
                break;
        }

        targetMapScript.SetEnabled(isPlayable);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

