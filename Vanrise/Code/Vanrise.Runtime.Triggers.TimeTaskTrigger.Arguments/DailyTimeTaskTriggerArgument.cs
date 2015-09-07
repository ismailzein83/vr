using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments
{
    public class DailyTimeTaskTriggerArgument : TimeTaskTriggerArgument
    {
        public List<Time> ScheduledTimesToRun { get; set; }
    }
}
