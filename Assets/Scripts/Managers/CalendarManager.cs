using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static LocationManager;

public class CalendarManager : MonoBehaviour, ISaveable
{
    public static CalendarManager instance { get; private set; }
    public DayManager currentDate { get; private set; }
    public DayPhase currentPhase { get; private set; }
    public event System.Action<DayPhase> OnPhaseChanged;

    [SerializeField] TextMeshProUGUI dateAndTimeText;

    private int daysPassed = 0;

    private bool mainEvent13Triggered = false;
    private bool mainEvent15Triggered = false;

    private DayManager mainEventDay = null;

    private int totalDaysElapsed = 0;

    private bool mainEvent17Completed = false;
    private bool mainEvent2Triggered = false;

    private int dayIndexWhen17Completed = -1;

    private bool mainEvent21Triggered = false;
    private DayManager mainEvent2Day = null;

    private int dayIndexWhenEvent2Occurred = -1;
    private int dayIndexWhenEvent23Occurred = -1;
    private int dayIndexWhenEvent27Occurred = -1;

    private bool mainEvent23Triggered = false;
    private bool mainEvent27Triggered = false;
    private bool mainEvent29Triggered = false;

    public string SaveID => "CalendarManager";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the event
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeCalendar(new DayManager(20, 6, 1)); // Set start date
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe from the event
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ensure the TextMeshProUGUI component is reassigned or updated
        TextMeshProUGUI dateAndTime = GameObject.Find("DateAndTime")?.GetComponent<TextMeshProUGUI>();
        if (dateAndTime != null)
        {
            dateAndTimeText = dateAndTime;
            setText();
        }
    }

    public void InitializeCalendar(DayManager startDate)
    {
        currentDate = startDate;
        currentPhase = DayPhase.Morning;
    }

    public void AdvancePhase()
    {
        // Night -> Morning (new day begins)
        if (currentPhase == DayPhase.Night)
        {
            currentPhase = DayPhase.Morning;
            currentDate.AdvanceDay();

            daysPassed++;
            totalDaysElapsed++;

            // Check if this is the 7th completed day
            if (daysPassed >= 7 && !mainEvent13Triggered)
            {
                mainEvent13Triggered = true;

                // Store the specific calendar date
                mainEventDay = new DayManager(
                    currentDate.Year,
                    currentDate.Month,
                    currentDate.Day
                );

                SceneManager.LoadScene("MainEvent1.3");
                return;
            }

            // Check for MainEvent2 trigger
            if (mainEvent17Completed && !mainEvent2Triggered)
            {
                bool isMonday = totalDaysElapsed % 7 == 0;
                bool isAfterCompletion = totalDaysElapsed > dayIndexWhen17Completed;

                if (isMonday && isAfterCompletion)
                {
                    mainEvent2Triggered = true;

                    // Store exact calendar date
                    mainEvent2Day = new DayManager(
                        currentDate.Year,
                        currentDate.Month,
                        currentDate.Day
                    );

                    dayIndexWhenEvent2Occurred = totalDaysElapsed;

                    SceneManager.LoadScene("MainEvent2");
                    return;
                }
            }

        }
        else
        {
            currentPhase++;

            if (currentPhase == DayPhase.Evening &&
                !mainEvent23Triggered &&
                dayIndexWhenEvent2Occurred >= 0 &&
                LocationManager.Instance.HasAtLeastOneDate2Completed())
            {
                int daysSinceEvent2 = totalDaysElapsed - dayIndexWhenEvent2Occurred;

                if (daysSinceEvent2 >= 4)
                {
                    mainEvent23Triggered = true;
                    dayIndexWhenEvent23Occurred = totalDaysElapsed;
                    SceneManager.LoadScene("MainEvent2.3");
                    return;
                }
            }

            if (currentPhase == DayPhase.Evening &&
                mainEvent23Triggered &&
                !mainEvent27Triggered &&
                dayIndexWhenEvent23Occurred >= 0 &&
                LocationManager.Instance.HasAtLeastOneDate2Completed())
            {
                int daysSince23 = totalDaysElapsed - dayIndexWhenEvent23Occurred;

                if (daysSince23 >= 4)
                {
                    mainEvent27Triggered = true;
                    dayIndexWhenEvent27Occurred = totalDaysElapsed;
                    SceneManager.LoadScene("MainEvent2.7");
                    return;
                }
            }

            if (currentPhase == DayPhase.Evening &&
                mainEvent27Triggered &&
                !mainEvent29Triggered &&
                dayIndexWhenEvent27Occurred >= 0 &&
                LocationManager.Instance.HasAtLeastOneDate2Completed())
            {
                int daysSince27 = totalDaysElapsed - dayIndexWhenEvent27Occurred;

                if (daysSince27 >= 4)
                {
                    mainEvent29Triggered = true;
                    StoryProgressFlags.mainEvent29Completed = true;
                    SceneManager.LoadScene("MainEvent2.9");
                    return;
                }
            }

            // Trigger MainEvent2.1 on Night of same day as MainEvent2
            if (currentPhase == DayPhase.Night &&
                mainEvent2Day != null &&
                !mainEvent21Triggered &&
                DatesMatch(currentDate, mainEvent2Day))
            {
                mainEvent21Triggered = true;
                SceneManager.LoadScene("MainEvent2.1");
                return;
            }

            // If we just entered Evening on the same event day
            if (currentPhase == DayPhase.Evening &&
                mainEventDay != null &&
                !mainEvent15Triggered &&
                DatesMatch(currentDate, mainEventDay))
            {
                mainEvent15Triggered = true;
                SceneManager.LoadScene("MainEvent1.5");
                return;
            }
        }

        setText();
        OnPhaseChanged?.Invoke(currentPhase);
    }

    public override string ToString()
    {
        return $"{currentDate.ToString()}  | {currentPhase}";
    }

    public void setText()
    {
        dateAndTimeText.text = ToString();
    }

    private bool DatesMatch(DayManager a, DayManager b)
    {
        return a.Year == b.Year &&
               a.Month == b.Month &&
               a.Day == b.Day;
    }

    public void NotifyMainEvent17Completed()
    {
        mainEvent17Completed = true;
        dayIndexWhen17Completed = totalDaysElapsed;
    }

    [System.Serializable]
    class SaveData
    {
        public int year;
        public int month;
        public int day;
        public int phase;

        public int daysPassed;
        public int totalDaysElapsed;

        public bool mainEvent13Triggered;
        public bool mainEvent15Triggered;
        public bool mainEvent17Completed;
        public bool mainEvent2Triggered;
        public bool mainEvent21Triggered;
        public bool mainEvent23Triggered;
        public bool mainEvent27Triggered;
        public bool mainEvent29Triggered;

        public int dayIndexWhen17Completed;
        public int dayIndexWhenEvent2Occurred;
        public int dayIndexWhenEvent23Occurred;
        public int dayIndexWhenEvent27Occurred;

        public DateSnapshot mainEventDay;
        public DateSnapshot mainEvent2Day;
    }

    [System.Serializable]
    class DateSnapshot
    {
        public int year;
        public int month;
        public int day;
    }

    public string CaptureState()
    {
        SaveData data = new SaveData
        {
            year = currentDate.Year,
            month = currentDate.Month,
            day = currentDate.Day,
            phase = (int)currentPhase,

            daysPassed = daysPassed,
            totalDaysElapsed = totalDaysElapsed,

            mainEvent13Triggered = mainEvent13Triggered,
            mainEvent15Triggered = mainEvent15Triggered,
            mainEvent17Completed = mainEvent17Completed,
            mainEvent2Triggered = mainEvent2Triggered,
            mainEvent21Triggered = mainEvent21Triggered,
            mainEvent23Triggered = mainEvent23Triggered,
            mainEvent27Triggered = mainEvent27Triggered,
            mainEvent29Triggered = mainEvent29Triggered,

            dayIndexWhen17Completed = dayIndexWhen17Completed,
            dayIndexWhenEvent2Occurred = dayIndexWhenEvent2Occurred,
            dayIndexWhenEvent23Occurred = dayIndexWhenEvent23Occurred,
            dayIndexWhenEvent27Occurred = dayIndexWhenEvent27Occurred
        };

        if (mainEventDay != null)
            data.mainEventDay = new DateSnapshot
            {
                year = mainEventDay.Year,
                month = mainEventDay.Month,
                day = mainEventDay.Day
            };

        if (mainEvent2Day != null)
            data.mainEvent2Day = new DateSnapshot
            {
                year = mainEvent2Day.Year,
                month = mainEvent2Day.Month,
                day = mainEvent2Day.Day
            };

        return JsonUtility.ToJson(data);
    }

    public void RestoreState(string json)
    {
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        currentDate = new DayManager(data.year, data.month, data.day);
        currentPhase = (DayPhase)data.phase;

        daysPassed = data.daysPassed;
        totalDaysElapsed = data.totalDaysElapsed;

        mainEvent13Triggered = data.mainEvent13Triggered;
        mainEvent15Triggered = data.mainEvent15Triggered;
        mainEvent17Completed = data.mainEvent17Completed;
        mainEvent2Triggered = data.mainEvent2Triggered;
        mainEvent21Triggered = data.mainEvent21Triggered;
        mainEvent23Triggered = data.mainEvent23Triggered;
        mainEvent27Triggered = data.mainEvent27Triggered;
        mainEvent29Triggered = data.mainEvent29Triggered;

        dayIndexWhen17Completed = data.dayIndexWhen17Completed;
        dayIndexWhenEvent2Occurred = data.dayIndexWhenEvent2Occurred;
        dayIndexWhenEvent23Occurred = data.dayIndexWhenEvent23Occurred;
        dayIndexWhenEvent27Occurred = data.dayIndexWhenEvent27Occurred;

        if (data.mainEventDay != null)
            mainEventDay = new DayManager(
                data.mainEventDay.year,
                data.mainEventDay.month,
                data.mainEventDay.day);

        if (data.mainEvent2Day != null)
            mainEvent2Day = new DayManager(
                data.mainEvent2Day.year,
                data.mainEvent2Day.month,
                data.mainEvent2Day.day);

        setText();
        OnPhaseChanged?.Invoke(currentPhase);
    }
}