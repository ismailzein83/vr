using System;
using Vanrise.Runtime.Entities;
using Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments;

namespace Vanrise.Runtime.Triggers.TimeTaskTrigger
{
    public class IntervalTimeSchedulerTaskTrigger : TimeSchedulerTaskTrigger
    {
        public override DateTime? CalculateNextTimeToRun(SchedulerTask task, SchedulerTaskState taskState, BaseTaskTriggerArgument taskTriggerArgument)
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
                        do
                            nextRunTime = nextRunTime.AddHours(intervalTimeTaskTriggerArgument.Interval);
                        while (intervalTimeTaskTriggerArgument.IgnoreSkippedIntervals && nextRunTime < now);
                        break;
                    case IntervalType.Minute:
                        do
                            nextRunTime = nextRunTime.AddMinutes(intervalTimeTaskTriggerArgument.Interval);
                        while (intervalTimeTaskTriggerArgument.IgnoreSkippedIntervals && nextRunTime < now);
                        break;
                    case IntervalType.Second:
                        do
                            nextRunTime = nextRunTime.AddSeconds(intervalTimeTaskTriggerArgument.Interval);
                        while (intervalTimeTaskTriggerArgument.IgnoreSkippedIntervals && nextRunTime < now);
                        break;
                }
            }

            return nextRunTime;
        }
    }
}