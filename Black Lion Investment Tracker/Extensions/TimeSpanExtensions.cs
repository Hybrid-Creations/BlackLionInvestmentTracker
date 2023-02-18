using System;
using System.Text;

namespace BLIT.Extensions;

public static partial class TimeSpanExtensions
{
    public static int Months(this TimeSpan value, DateTime referenceLocalEndDate)
    {
        var end = referenceLocalEndDate;
        if (value.Days > DateTime.DaysInMonth(end.Year, end.Month))
        {
            int index = 0;
            int months = 0;
            int daysRemaining = value.Days;
            while (daysRemaining > 0)
            {
                var daysInMonth = DateTime.DaysInMonth(end.Year + index, end.Month + index);
                if (daysInMonth > daysRemaining != false)
                    daysRemaining = 0;
                else
                {
                    daysRemaining -= daysInMonth;
                    months++;
                }
            }
            return months;
        }
        else return 0;
    }
}
