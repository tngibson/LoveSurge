using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CalendarManager : MonoBehaviour
{
    public static CalendarManager instance { get; private set; }
    public DayManager currentDate { get; private set; }
    public DayPhase currentPhase { get; private set; }
    public event System.Action<DayPhase> OnPhaseChanged;
    [SerializeField] TextMeshProUGUI dateAndTimeText;
    CalendarParent parent;
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
        return $"{currentDate.ToString()}  - {currentPhase}";
    }

    public void setText()
    {
        dateAndTimeText.text = parent.getText();
    }
}
