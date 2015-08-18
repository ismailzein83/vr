using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments
{
    public class WeeklyTimeTaskTriggerArgument : DailyTimeTaskTriggerArgument
    {
        public List<DayOfWeek> ScheduledDays { get; set; }
    }
}
