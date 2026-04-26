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

        // NEW: prevents re-processing transitions
        public bool progressionResolved = false;
    }

    public enum DateNum { Date1, Date2, Date3 }
    public enum Date1Stage { Intro, CardGame, Done }
    public enum Date2Stage { Intro, CardGame, Done }
    public enum Date3Stage { Intro, CardGame, Done }

    public string SaveID => "LocationManager";

    [Header("Character Date Data")]
    public List<DateData> characterDates = new List<DateData>
    {
        new DateData { name = "Noki" },
        new DateData { name = "Celci" },
        new DateData { name = "Lotte" }
    };

    private bool mainEvent17Triggered = false;

    private bool tutorialEnded = false;

    private string activeDateCharacter = null;

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
        if (scene.name == "Map" && tutorialEnded == false)
        {
            tutorialEnded = true;
            ResetPlayerConvo();
        }

        foreach (var data in characterDates)
        {
            data.mapScript = GameObject.Find(data.name + "Date")?.GetComponent<MapScript>();
            if (data.mapScript == null) continue;

            // NEW: resolve progression safely
            ResolveProgression(data);

            if (data.allDatesDone)
            {
                data.mapScript.UpdateLocationText($"{data.name}'s story is complete!");
                data.mapScript.SetEnabled(false);
                data.mapScript.gameObject.SetActive(false);
                continue;
            }

            UpdateMapUI(data);
        }
    }

    // NEW: handles progression ONLY
    private void ResolveProgression(DateData data)
    {
        if (data.progressionResolved) return;

        switch (data.currentDate)
        {
            case DateNum.Date1:
                if (data.date1Stage == Date1Stage.Done)
                {
                    data.progressionResolved = true;
                    UnlockFirstDateCharacterAchievement(data);
                    UnlockDateAchievement(data, DateNum.Date1);
                    CheckForMainEvent17();
                    AdvanceToNextDate(data);
                }
                break;

            case DateNum.Date2:
                if (data.date2Stage == Date2Stage.Done)
                {
                    data.progressionResolved = true;
                    UnlockDateAchievement(data, DateNum.Date2);
                    CheckForMainEvent17();
                }
                break;
        }
    }

    // NEW: handles ONLY UI + locName
    private void UpdateMapUI(DateData data)
    {
        if (data.mapScript == null) return;

        switch (data.currentDate)
        {
            case DateNum.Date1:
                data.mapScript.locName = data.date1Stage == Date1Stage.CardGame
                    ? $"{data.name}Date1CardGame1"
                    : $"{data.name}Date1Intro";
                break;

            case DateNum.Date2:
                data.mapScript.locName = data.date2Stage == Date2Stage.CardGame
                    ? $"{data.name}Date2CardGame1"
                    : $"{data.name}Date2Intro";
                break;

            case DateNum.Date3:
                data.mapScript.locName = data.date3Stage == Date3Stage.CardGame
                    ? $"{data.name}Date3CardGame1"
                    : $"{data.name}Date3Intro";
                break;
        }

        if (data.isPlayable && data.isFirstTime)
        {
            data.mapScript.UpdateLocationText($"Go on a date with {data.name}!");
            data.mapScript.SetEnabled(true);
            data.phaseEnteredDate = CalendarManager.instance.currentPhase;
            data.isFirstTime = false;
        }
        else if (data.isPlayable)
        {
            data.mapScript.UpdateLocationText($"Continue your date with {data.name}!");
            data.mapScript.SetEnabled(true);
            data.phaseEnteredDate = CalendarManager.instance.currentPhase;
        }
        else
        {
            data.mapScript.UpdateLocationText("You must wait a full day to continue your date!");
            data.mapScript.SetEnabled(false);
        }
    }

    // NEW: clean transition
    private void AdvanceToNextDate(DateData data)
    {
        data.progressionResolved = false;

        if (data.currentDate == DateNum.Date1)
        {
            data.currentDate = DateNum.Date2;
            data.date2Stage = Date2Stage.Intro;
        }
        else if (data.currentDate == DateNum.Date2)
        {
            data.currentDate = DateNum.Date3;
            data.date3Stage = Date3Stage.Intro;
        }

        ResetPlayerConvo();
        ConnectionManager.instance.setConnection(0, 0);
    }



    public void SetDateState(string charName, bool started)
    {
        var data = characterDates.Find(d => d.name == charName);
        if (data == null) return;

        data.dateStarted = started;
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

    public bool TryStartDate(string charName)
    {
        // If no active date, lock it in
        if (string.IsNullOrEmpty(activeDateCharacter))
        {
            activeDateCharacter = charName;
            return true;
        }

        // If trying to switch mid-date -> reject
        if (activeDateCharacter != charName)
        {
            Debug.Log("Cannot switch dates mid-progress.");
            return false;
        }

        return true;
    }

    private void UnlockFirstDateCharacterAchievement(DateData data)
    {
        if (AchievementComponent.AchievementSystem == null) return;

        AchievementID id;

        switch (data.name)
        {
            case "Celci": id = AchievementID.NEW_ACHIEVEMENT_1_0; break;
            case "Lotte": id = AchievementID.NEW_ACHIEVEMENT_1_1; break;
            case "Noki": id = AchievementID.NEW_ACHIEVEMENT_1_2; break;
            default: return;
        }

        AchievementComponent.AchievementSystem.UnlockAchievement(id);
    }

    private void UnlockDateAchievement(DateData data, DateNum completedDate)
    {
        if (AchievementComponent.AchievementSystem == null) return;

        int baseIndex = 0;

        switch (data.name)
        {
            case "Noki": baseIndex = 9; break;
            case "Celci": baseIndex = 12; break;
            case "Lotte": baseIndex = 15; break;
            default: return;
        }

        int achievementIndex = baseIndex + (int)completedDate;
        AchievementID id = (AchievementID)achievementIndex;

        AchievementComponent.AchievementSystem.UnlockAchievement(id);
    }

    public string CaptureState()
    {
        SaveData data = new SaveData { dates = new List<CharacterDateSave>() };

        foreach (var d in characterDates)
        {
            data.dates.Add(new CharacterDateSave
            {
                name = d.name,
                isPlayable = d.isPlayable,
                dateStarted = d.dateStarted,
                isFirstTime = d.isFirstTime,
                allDatesDone = d.allDatesDone,
                phase = (int)d.phaseEnteredDate,
                currentDate = (int)d.currentDate,
                d1 = (int)d.date1Stage,
                d2 = (int)d.date2Stage,
                d3 = (int)d.date3Stage
            });
        }

        return JsonUtility.ToJson(data);
    }

    public void RestoreState(string json)
    {
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        foreach (var s in data.dates)
        {
            var d = characterDates.Find(x => x.name == s.name);
            if (d == null) continue;

            d.isPlayable = s.isPlayable;
            d.dateStarted = s.dateStarted;
            d.isFirstTime = s.isFirstTime;
            d.allDatesDone = s.allDatesDone;
            d.phaseEnteredDate = (DayPhase)s.phase;
            d.currentDate = (DateNum)s.currentDate;
            d.date1Stage = (Date1Stage)s.d1;
            d.date2Stage = (Date2Stage)s.d2;
            d.date3Stage = (Date3Stage)s.d3;

            d.progressionResolved = false; // ensure safe re-evaluation
        }
    }

    [System.Serializable]
    class SaveData
    {
        public List<CharacterDateSave> dates;
    }

    [System.Serializable]
    class CharacterDateSave
    {
        public string name;
        public bool isPlayable;
        public bool dateStarted;
        public bool isFirstTime;
        public bool allDatesDone;
        public int phase;
        public int currentDate;
        public int d1;
        public int d2;
        public int d3;
    }
}