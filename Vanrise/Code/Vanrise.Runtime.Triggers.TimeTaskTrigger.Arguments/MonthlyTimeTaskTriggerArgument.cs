using System.Collections.Generic;

namespace Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments
{
    public enum DayOfMonthTypeEnum { SpecificDay = 0, LastDay = 1 }

    public class MonthlyTimeTaskTriggerArgument : DailyTimeTaskTriggerArgument
    {
        public List<DayOfMonth> ScheduledDays { get; set; }
    }

    public class DayOfMonth
    {
        public DayOfMonthTypeEnum DayOfMonthType { get; set; }

        public int? SpecificDay { get; set; }
    }
}