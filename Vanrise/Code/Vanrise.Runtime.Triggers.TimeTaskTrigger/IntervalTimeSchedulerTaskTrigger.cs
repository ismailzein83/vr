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
        public override DateTime CalculateNextTimeToRun(SchedulerTask task, SchedulerTaskState taskState, BaseTaskTriggerArgument taskTriggerArgument)
        {
            DateTime nextRunTime = DateTime.MinValue;

            if (taskState.NextRunTime == null)
            {
                nextRunTime = DateTime.Now;
            }
            else
            {
                IntervalTimeTaskTriggerArgument intervalTimeTaskTriggerArgument = (IntervalTimeTaskTriggerArgument)taskTriggerArgument;

                switch (intervalTimeTaskTriggerArgument.IntervalType)
                {
                    case IntervalType.Hour:
                        nextRunTime = DateTime.Now.AddHours(intervalTimeTaskTriggerArgument.Interval);
                        break;
                    case IntervalType.Minute:
                        nextRunTime = DateTime.Now.AddMinutes(intervalTimeTaskTriggerArgument.Interval);
                        break;
                    case IntervalType.Second:
                        nextRunTime = DateTime.Now.AddSeconds(intervalTimeTaskTriggerArgument.Interval);
                        break;
                }
            }

            return nextRunTime;
        }
    }
}
