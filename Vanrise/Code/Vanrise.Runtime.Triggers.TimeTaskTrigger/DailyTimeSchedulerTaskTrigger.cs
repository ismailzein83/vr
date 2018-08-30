using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Entities;
using Vanrise.Runtime.Entities;
using Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments;

namespace Vanrise.Runtime.Triggers.TimeTaskTrigger
{
    public class DailyTimeSchedulerTaskTrigger : TimeSchedulerTaskTrigger
    {
        public override DateTime? CalculateNextTimeToRun(SchedulerTask task, SchedulerTaskState taskState, BaseTaskTriggerArgument taskTriggerArgument)
        {
            DailyTimeTaskTriggerArgument dailyTimeTaskTriggerArgument = (DailyTimeTaskTriggerArgument)taskTriggerArgument;

            List<DateTime> listofScheduledDateTimes = new List<DateTime>();
            var now = DateTime.Now;
            var nextruntime = taskState.NextRunTime.HasValue && !dailyTimeTaskTriggerArgument.IgnoreSkippedIntervals ? taskState.NextRunTime.Value : now;

            foreach (Time time in dailyTimeTaskTriggerArgument.ScheduledTimesToRun)
            {
                TimeSpan scheduledTime = new TimeSpan(time.Hour, time.Minute, 0);
                TimeSpan spanTillThen = scheduledTime - nextruntime.TimeOfDay;

                int daysTillThen = 0;

                if (spanTillThen.Ticks <= 0)
                    daysTillThen += 1;

                listofScheduledDateTimes.Add(nextruntime.AddDays(daysTillThen).Add(spanTillThen));
            }

            return listofScheduledDateTimes.OrderBy(x => x.Ticks).ToList().FirstOrDefault();
        }
    }
}
