using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocationManager : MonoBehaviour, ISaveable
{
    public static LocationManager Instance { get; private set; }

    [System.Serializable]
    public class DateData
    {
        public string name; // e.g. "Noki", "Celci", "Lotte"
        public MapScript mapScript;
        public bool isPlayable = true;
        public bool dateStarted = false;
        public bool isFirstTime = true;
        public bool allDatesDone = false;
        public DayPhase phaseEnteredDate = DayPhase.None;

        public DateNum currentDate = DateNum.Date1;
        public Date1Stage date1Stage = Date1Stage.Intro;
        public Date2Stage date2Stage = Date2Stage.Intro;
        public Date3Stage date3Stage = Date3Stage.Intro;
    }

    public enum DateNum { Date1, Date2, Date3 }
    public enum Date1Stage { Intro, CardGame, Done }
    public enum Date2Stage { Intro, CardGame, Done }
    public enum Date3Stage { Intro, CardGame, Done }

    [Header("Character Date Data")]
    public List<DateData> characterDates = new List<DateData>
    {
        new DateData { name = "Noki" },
        new DateData { name = "Celci" },
        new DateData { name = "Lotte" }
    };

    private bool mainEvent17Triggered = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (var data in characterDates)
        {
            data.mapScript = GameObject.Find(data.name + "Date")?.GetComponent<MapScript>();
            if (data.mapScript == null) continue;

            if (data.allDatesDone)
            {
                // Hide or disable the button entirely if all dates are done
                data.mapScript.UpdateLocationText($"{data.name}'s story is complete!");
                data.mapScript.SetEnabled(false);
                data.mapScript.gameObject.SetActive(false);
                continue;
            }

            UpdateMapLocation(data);

            if (data.isPlayable && data.isFirstTime)
            {
                data.mapScript.UpdateLocationText($"Go on a date with {data.name}!");
                data.mapScript.SetEnabled(true);
                data.phaseEnteredDate = CalendarManager.instance.currentPhase;
                data.isFirstTime = false;
            }
            else if (data.isPlayable && !data.isFirstTime)
            {
                data.mapScript.UpdateLocationText($"Continue your date with {data.name}!");
                data.mapScript.SetEnabled(true);
                data.phaseEnteredDate = CalendarManager.instance.currentPhase;
            }
            else if (data.phaseEnteredDate != DayPhase.None &&
                     data.phaseEnteredDate == CalendarManager.instance.currentPhase)
            {
                data.mapScript.UpdateLocationText($"Continue your date with {data.name}!");
                data.mapScript.SetEnabled(true);
                data.isPlayable = true;
                data.phaseEnteredDate = DayPhase.None;
            }
            else
            {
                data.mapScript.UpdateLocationText("You must wait a full day to continue your date!");
                data.mapScript.SetEnabled(false);
            }
        }
    }

    public void SetDateState(string charName, bool started)
    {
        var data = characterDates.Find(d => d.name == charName);
        if (data == null) return;

        data.dateStarted = started;
        if (started)
        {
            UpdateMapLocation(data);
        }
    }

    private void UpdateMapLocation(DateData data)
    {
        if (data.mapScript == null) return;

        switch (data.currentDate)
        {
            case DateNum.Date1:
                switch (data.date1Stage)
                {
                    case Date1Stage.Intro:
                        data.mapScript.locName = $"{data.name}Date1Intro";
                        break;
                    case Date1Stage.CardGame:
                        data.mapScript.locName = $"{data.name}Date1CardGame1";
                        break;
                    case Date1Stage.Done:
                        UnlockFirstDateCharacterAchievement(data);
                        UnlockDateAchievement(data, DateNum.Date1);
                        
                        CheckForMainEvent17();

                        data.currentDate = DateNum.Date2;
                        data.date2Stage = Date2Stage.Intro;
                        UpdateMapLocation(data);
                        ResetPlayerConvo();
                        ConnectionManager.instance.setConnection(0, 0);
                        return;
                }
                break;

            case DateNum.Date2:
                switch (data.date2Stage)
                {
                    case Date2Stage.Intro:
                        data.mapScript.locName = $"{data.name}Date2Intro";
                        break;
                    case Date2Stage.CardGame:
                        data.mapScript.locName = $"{data.name}Date2CardGame1";
                        break;
                    case Date2Stage.Done:
                        UnlockDateAchievement(data, DateNum.Date2);

                        CheckForMainEvent17();

                        data.currentDate = DateNum.Date3;
                        data.date3Stage = Date3Stage.Intro;
                        UpdateMapLocation(data);
                        ResetPlayerConvo();
                        ConnectionManager.instance.setConnection(0, 0);
                        return;
                }
                break;

            case DateNum.Date3:
                switch (data.date3Stage)
                {
                    case Date3Stage.Intro:
                        data.mapScript.locName = $"{data.name}Date3Intro";
                        break;
                    case Date3Stage.CardGame:
                        data.mapScript.locName = $"{data.name}Date3CardGame1";
                        break;
                    case Date3Stage.Done:
                        UnlockDateAchievement(data, DateNum.Date3);

                        switch (data.name)
                        {
                            case "Celci":
                                Player.instance.lastCharacterCompleted = 1;
                                break;

                            case "Lotte":
                                Player.instance.lastCharacterCompleted = 2;
                                break;

                            case "Noki":
                                Player.instance.lastCharacterCompleted = 0;
                                break;

                            default:
                                return;
                        }

                        Debug.Log($"All {data.name} dates completed.");
                        data.allDatesDone = true;

                        // Disable and hide the button when all dates are done
                        if (data.mapScript != null)
                        {
                            data.mapScript.UpdateLocationText($"{data.name}'s story is complete!");
                            data.mapScript.SetEnabled(false);
                            data.mapScript.gameObject.SetActive(false);
                        }
                        return;
                }
                break;
        }

        data.mapScript.SetEnabled(data.isPlayable);
    }

    private void ResetPlayerConvo()
    {
        for (int i = 0; i < Player.instance.convoTiers.Count; i++)
        {
            Player.instance.convoTiers[i] = 1;
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void TryBindMapScript(MapScript map)
    {
        if (map == null) return;

        foreach (var data in characterDates)
        {
            if (map.name.Contains(data.name + "Date"))
            {
                data.mapScript = map;
                //Debug.Log($"Bound {data.name} map button.");
                UpdateMapLocation(data);

                if (data.allDatesDone)
                {
                    map.UpdateLocationText($"{data.name}'s story is complete!");
                    map.SetEnabled(false);
                    map.gameObject.SetActive(false);
                    return;
                }

                if (data.isPlayable && data.isFirstTime)
                {
                    map.UpdateLocationText($"Go on a date with {data.name}!");
                    map.SetEnabled(true);
                    data.phaseEnteredDate = CalendarManager.instance.currentPhase;
                    data.isFirstTime = false;
                }
                else if (data.isPlayable)
                {
                    map.UpdateLocationText($"Continue your date with {data.name}!");
                    map.SetEnabled(true);
                    data.phaseEnteredDate = CalendarManager.instance.currentPhase;
                }
                else
                {
                    map.UpdateLocationText("You must wait a full day to continue your date!");
                    map.SetEnabled(false);
                }

                return;
            }
        }
    }

    public void SetPhaseEnteredDate(string charName, DayPhase phase)
    {
        var data = characterDates.Find(d => d.name == charName);
        if (data != null)
        {
            data.phaseEnteredDate = phase;
        }
    }

    private void CheckForMainEvent17()
    {
        if (mainEvent17Triggered)
            return;

        bool allDate1Complete = true;
        bool atLeastOneDate2Complete = false;

        foreach (var data in characterDates)
        {
            // Date1 complete if we have progressed beyond it
            if (data.currentDate == DateNum.Date1)
                allDate1Complete = false;

            // Date2 complete if we have progressed beyond it
            if (data.currentDate == DateNum.Date3 ||
               (data.currentDate == DateNum.Date2 && data.date2Stage == Date2Stage.Done))
            {
                atLeastOneDate2Complete = true;
            }
        }

        if (allDate1Complete && atLeastOneDate2Complete)
        {
            mainEvent17Triggered = true;
            CalendarManager.instance.NotifyMainEvent17Completed();
            SceneManager.LoadScene("MainEvent1.7");
        }
    }

    public bool HasAtLeastOneDate2Completed()
    {
        foreach (var data in characterDates)
        {
            if (data.currentDate == DateNum.Date3 ||
               (data.currentDate == DateNum.Date2 && data.date2Stage == Date2Stage.Done))
            {
                return true;
            }
        }

        return false;
    }

    private void UnlockFirstDateCharacterAchievement(DateData data)
    {
        if (AchievementComponent.AchievementSystem == null)
            return;

        AchievementID id;

        switch (data.name)
        {
            case "Celci":
                id = AchievementID.NEW_ACHIEVEMENT_1_1;
                break;

            case "Lotte":
                id = AchievementID.NEW_ACHIEVEMENT_1_2;
                break;

            case "Noki":
                id = AchievementID.NEW_ACHIEVEMENT_1_3;
                break;

            default:
                return;
        }

        AchievementComponent.AchievementSystem.UnlockAchievement(id);
    }

    private void UnlockDateAchievement(DateData data, DateNum completedDate)
    {
        if (AchievementComponent.AchievementSystem == null)
            return;

        int baseIndex = 0;

        switch (data.name)
        {
            case "Noki":
                baseIndex = 9;
                break;
            case "Celci":
                baseIndex = 12;
                break;
            case "Lotte":
                baseIndex = 15;
                break;
            default:
                return;
        }

        int dateOffset = (int)completedDate; // Date1=0, Date2=1, Date3=2
        int achievementIndex = baseIndex + dateOffset;

        AchievementID id = (AchievementID)achievementIndex;

        AchievementComponent.AchievementSystem.UnlockAchievement(id);
    }

    [System.Serializable]
    private struct LocationSaveData
    {
        public List<CharacterSaveData> characters;
    }

    [System.Serializable]
    private struct CharacterSaveData
    {
        public string name;
        public bool isPlayable;
        public bool dateStarted;
        public bool isFirstTime;
        public bool allDatesDone;
        public int phaseEnteredDate;
        public int currentDate;
        public int date1Stage;
        public int date2Stage;
        public int date3Stage;
    }

    private void OnEnable() => SaveLoadManager.Register(this);
    private void OnDisable() => SaveLoadManager.Unregister(this);

    public object CaptureState()
    {
        var list = new List<CharacterSaveData>();

        foreach (var d in characterDates)
        {
            list.Add(new CharacterSaveData
            {
                name = d.name,
                isPlayable = d.isPlayable,
                dateStarted = d.dateStarted,
                isFirstTime = d.isFirstTime,
                allDatesDone = d.allDatesDone,
                phaseEnteredDate = (int)d.phaseEnteredDate,
                currentDate = (int)d.currentDate,
                date1Stage = (int)d.date1Stage,
                date2Stage = (int)d.date2Stage,
                date3Stage = (int)d.date3Stage
            });
        }

        return JsonUtility.ToJson(new LocationSaveData { characters = list });
    }

    public void RestoreState(object state)
    {
        var json = state as string;
        var data = JsonUtility.FromJson<LocationSaveData>(json);

        foreach (var saved in data.characters)
        {
            var d = characterDates.Find(c => c.name == saved.name);
            if (d == null) continue;

            d.isPlayable = saved.isPlayable;
            d.dateStarted = saved.dateStarted;
            d.isFirstTime = saved.isFirstTime;
            d.allDatesDone = saved.allDatesDone;
            d.phaseEnteredDate = (DayPhase)saved.phaseEnteredDate;
            d.currentDate = (DateNum)saved.currentDate;
            d.date1Stage = (Date1Stage)saved.date1Stage;
            d.date2Stage = (Date2Stage)saved.date2Stage;
            d.date3Stage = (Date3Stage)saved.date3Stage;
        }

        if (SceneManager.GetActiveScene().isLoaded)
            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }
}