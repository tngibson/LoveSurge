using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.SceneManagement;
using static LocationManager;

public class CalendarManager : MonoBehaviour
{
    public static CalendarManager instance { get; private set; }
    public DayManager currentDate { get; private set; }
    public DayPhase currentPhase { get; private set; }
    public event System.Action<DayPhase> OnPhaseChanged;
    [SerializeField] TextMeshProUGUI date;
    [SerializeField] TextMeshProUGUI time;

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
        setText();
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
        return $"{currentDate}  | {currentPhase}";
    }

    public void setText()
    {
        try
        {
            date = GameObject.Find("Date").GetComponent<TextMeshProUGUI>();
            time = GameObject.Find("Time").GetComponent<TextMeshProUGUI>();
        }
        catch
        {
            //ignore
        }
        
        if (date != null) date.text = currentDate.ToString();
        if (time != null) time.text = currentPhase.ToString();
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
}
