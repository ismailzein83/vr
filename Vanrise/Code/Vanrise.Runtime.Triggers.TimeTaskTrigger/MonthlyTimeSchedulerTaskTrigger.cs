using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Entities;
using Vanrise.Runtime.Entities;
using Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments;

namespace Vanrise.Runtime.Triggers.TimeTaskTrigger
{
    public class MonthlyTimeSchedulerTaskTrigger : TimeSchedulerTaskTrigger
    {
        public override DateTime? CalculateNextTimeToRun(SchedulerTask task, SchedulerTaskState taskState, BaseTaskTriggerArgument taskTriggerArgument)
        {
            MonthlyTimeTaskTriggerArgument monthlyTimeTaskTriggerArgument = taskTriggerArgument as MonthlyTimeTaskTriggerArgument;

            var now = DateTime.Now;
            var nextRuntime = taskState.NextRunTime.HasValue && !monthlyTimeTaskTriggerArgument.IgnoreSkippedIntervals ? taskState.NextRunTime.Value : now;

            int lastDayOfMonth = DateTime.DaysInMonth(now.Year, now.Month);

            List<DateTime> listofScheduledDateTimes = new List<DateTime>();

            foreach (DayOfMonth dayOfMonth in monthlyTimeTaskTriggerArgument.ScheduledDays)
            {
                int day;

                switch (dayOfMonth.DayOfMonthType)
                {
                    case DayOfMonthTypeEnum.LastDay: day = lastDayOfMonth; break;
                    case DayOfMonthTypeEnum.SpecificDay: day = dayOfMonth.SpecificDay.Value; break;
                    default: throw new NotSupportedException($"DayOfMonthType '{dayOfMonth.DayOfMonthType}' is not supported");
                }

                int daysTillThen = day - nextRuntime.Day;

                foreach (Time time in monthlyTimeTaskTriggerArgument.ScheduledTimesToRun)
                {
                    TimeSpan scheduledTime = new TimeSpan(time.Hour, time.Minute, 0);
                    TimeSpan spanTillThen = scheduledTime - nextRuntime.TimeOfDay;

                    int daysToAdd = daysTillThen;
                    if ((daysTillThen < 0) || (daysTillThen == 0 && spanTillThen.Ticks <= 0))
                        daysToAdd += lastDayOfMonth;

                    listofScheduledDateTimes.Add(nextRuntime.AddDays(daysToAdd).Add(spanTillThen));
                }
            }

            return listofScheduledDateTimes.OrderBy(x => x.Ticks).ToList().FirstOrDefault();
        }
    }
}