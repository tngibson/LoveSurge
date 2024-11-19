using UnityEngine;

public class CalendarManager : MonoBehaviour
{
    public static CalendarManager Instance { get; private set; }
    public DayManager CurrentDate { get; private set; }
    public DayPhase CurrentPhase { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        InitializeCalendar(new DayManager(2023, 1, 1)); // Set start date
    }

    public void InitializeCalendar(DayManager startDate)
    {
        CurrentDate = startDate;
        CurrentPhase = DayPhase.Morning;
    }

    public void AdvancePhase()
    {
        if (CurrentPhase == DayPhase.Evening)
        {
            CurrentPhase = DayPhase.Morning;
            CurrentDate.AdvanceDay();
        }
        else
        {
            CurrentPhase++;
        }
    }

    public override string ToString()
    {
        return $"{CurrentDate.ToString()} - {CurrentPhase}";
    }
}
