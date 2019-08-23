using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments
{
    public enum MonthlyEnum { SpecificDay, LastDay }
    public class MonthlyTimeTaskTriggerArgument : DailyTimeTaskTriggerArgument
    {
        public List<DayOfMonth> ScheduledDays { get; set; }
    }

    public class DayOfMonth
    {
        public MonthlyEnum MonthlyValue { get; set; }

        public int SpecificDay { get; set; }
    }
}
