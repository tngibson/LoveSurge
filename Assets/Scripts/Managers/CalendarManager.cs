using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CalendarManager : MonoBehaviour, ISaveable
{
    public static CalendarManager instance { get; private set; }
    public DayManager currentDate { get; private set; }
    public DayPhase currentPhase { get; private set; }
    public event System.Action<DayPhase> OnPhaseChanged;
    [SerializeField] TextMeshProUGUI dateAndTimeText;

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

    void Update()
    {
  
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    AdvancePhase();
        //    Debug.Log($"Current Date and Phase: {ToString()}");
        //}
    }


    public void InitializeCalendar(DayManager startDate)
    {
        currentDate = startDate;
        currentPhase = DayPhase.Morning;
    }

    public void AdvancePhase()
    {
        if (currentPhase == DayPhase.Night)
        {
            currentPhase = DayPhase.Morning;
            currentDate.AdvanceDay();
        }
        else
        {
            currentPhase++;
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

    [System.Serializable]
    private struct CalendarSaveData
    {
        public int day;
        public int month;
        public int year;
        public int phase;
    }

    private void OnEnable() => SaveLoadManager.Register(this);
    private void OnDisable() => SaveLoadManager.Unregister(this);

    public object CaptureState()
    {
        return JsonUtility.ToJson(new CalendarSaveData
        {
            day = currentDate.Day,
            month = currentDate.Month,
            year = currentDate.Year,
            phase = (int)currentPhase
        });
    }

    public void RestoreState(object state)
    {
        var json = state as string;
        var data = JsonUtility.FromJson<CalendarSaveData>(json);

        currentDate = new DayManager(data.day, data.month, data.year);
        currentPhase = (DayPhase)data.phase;

        if (dateAndTimeText != null)
        {
            setText();
        }
    }
}
