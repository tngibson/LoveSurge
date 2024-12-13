using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CalendarParent : MonoBehaviour
{
    CalendarManager calendarManager = CalendarManager.instance;
    
    public static CalendarParent instance { get; private set; }

    public static TextMeshProUGUI dateAndTime;

    private void Update()
    {
        if (dateAndTime.text != null) 
        {
            print(calendarManager.ToString());
            dateAndTime.text = calendarManager.ToString();
        }
        else
        {
            print("text is null");
        }
        
    }
    public string getText()
    {
        return dateAndTime.text;
    }
}
