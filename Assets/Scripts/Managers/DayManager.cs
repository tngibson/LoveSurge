public enum DayPhase { Morning, Afternoon, Evening }

[System.Serializable]
public class DayManager
{
    public int Year;
    public int Month;
    public int Day;

    public DayManager(int year, int month, int day)
    {
        Year = year;
        Month = month;
        Day = day;
    }

    public void AdvanceDay()
    {
        Day++;
        // Handle month and year overflow
        if (Day > DaysInMonth(Month, Year))
        {
            Day = 1;
            Month++;
            if (Month > 12)
            {
                Month = 1;
                Year++;
            }
        }
    }

    private int DaysInMonth(int month, int year)
    {
        // Handle February for leap years
        if (month == 2)
            return (year % 4 == 0 && year % 100 != 0) || (year % 400 == 0) ? 29 : 28;

        // Return days for other months
        return month switch
        {
            4 or 6 or 9 or 11 => 30,
            _ => 31
        };
    }

    public override string ToString()
    {
        return $"{Year:0000}-{Month:00}-{Day:00}";
    }
}