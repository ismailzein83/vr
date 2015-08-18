using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;
using Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments;

namespace Vanrise.Runtime.Triggers.TimeTaskTrigger
{
    public class IntervalTimeSchedulerTaskTrigger : TimeSchedulerTaskTrigger
    {
        public override DateTime CalculateNextTimeToRun(BaseTaskTriggerArgument taskTriggerArgument)
        {
            IntervalTimeTaskTriggerArgument intervalTimeTaskTriggerArgument = (IntervalTimeTaskTriggerArgument)taskTriggerArgument;

            DateTime nextRunTime = DateTime.MinValue;

            switch (intervalTimeTaskTriggerArgument.IntervalType)
            {
                case IntervalType.Hour:
                    nextRunTime = DateTime.Now.AddHours(intervalTimeTaskTriggerArgument.Interval);
                    break;
                case IntervalType.Minute:
                    nextRunTime = DateTime.Now.AddMinutes(intervalTimeTaskTriggerArgument.Interval);
                    break;
            }

            return nextRunTime;
        }
    }
}
