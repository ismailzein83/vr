using System;
using System.Globalization;

namespace TABS.Addons.Utilities
{
    public static class Extensions
    {
        public static int WeekNumber(this DateTime dtPassed)
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(dtPassed, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday);
            return weekNum;
        }

        public static int MonthNumber(this DateTime dtPassed)
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int monthNum = ciCurr.Calendar.GetMonth(dtPassed);
            return monthNum;
        }
    }
}
