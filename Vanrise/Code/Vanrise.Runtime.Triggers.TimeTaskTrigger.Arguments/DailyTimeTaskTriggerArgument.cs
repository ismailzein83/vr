using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments
{
    public class DailyTimeTaskTriggerArgument : TimeTaskTriggerArgument
    {
        public List<string> ScheduledHours { get; set; }
    }
}
