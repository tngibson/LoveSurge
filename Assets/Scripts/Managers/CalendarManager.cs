using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalendarManager : MonoBehaviour
{
    public static CalendarManager instance { get; private set; }
    public DayManager currentDate { get; private set; }
    public DayPhase currentPhase { get; private set; }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeCalendar(new DayManager(20, 6, 2)); // Set start date
    }

    public void InitializeCalendar(DayManager startDate)
    {
        currentDate = startDate;
        currentPhase = DayPhase.Morning;
    }

    public void AdvancePhase()
    {
        if (currentPhase == DayPhase.Evening)
        {
            currentPhase = DayPhase.Morning;
            currentDate.AdvanceDay();
        }
        else
        {
            currentPhase++;
        }
    }

    public override string ToString()
    {
        return $"{currentDate.ToString()} - {currentPhase}";
    }
}
