using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments
{
    public enum TimeSchedulerType { Interval = 0, Daily = 1, Weekly = 2 };

    public class TimeTaskTriggerArgument : BaseTaskTriggerArgument
    {
        public TimeSchedulerType SelectedType { get; set; }
    }
}
