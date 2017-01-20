using System;
using Vanrise.Runtime.Entities;
using Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments;

namespace Vanrise.Runtime.Triggers.TimeTaskTrigger
{
    public class IntervalTimeSchedulerTaskTrigger : TimeSchedulerTaskTrigger
    {
        public override DateTime CalculateNextTimeToRun(SchedulerTask task, SchedulerTaskState taskState, BaseTaskTriggerArgument taskTriggerArgument)
        {
            DateTime nextRunTime;
            DateTime now = DateTime.Now;

            if (!taskState.NextRunTime.HasValue)
            {
                nextRunTime = now;
            }
            else
            {
                IntervalTimeTaskTriggerArgument intervalTimeTaskTriggerArgument = (IntervalTimeTaskTriggerArgument)taskTriggerArgument;
                nextRunTime = taskState.NextRunTime.Value;

                switch (intervalTimeTaskTriggerArgument.IntervalType)
                {
                    case IntervalType.Hour:
                        while (nextRunTime < now)
                            nextRunTime = nextRunTime.AddHours(intervalTimeTaskTriggerArgument.Interval);
                        break;
                    case IntervalType.Minute:
                        while (nextRunTime < now)
                            nextRunTime = nextRunTime.AddMinutes(intervalTimeTaskTriggerArgument.Interval);
                        break;
                    case IntervalType.Second:
                        while (nextRunTime < now)
                            nextRunTime = nextRunTime.AddSeconds(intervalTimeTaskTriggerArgument.Interval);
                        break;
                }
            }

            return nextRunTime;
        }
    }
}