using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Runtime.Entities;
using Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments;

namespace Vanrise.Runtime.Triggers.TimeTaskTrigger
{
    public class MonthlyTimeSchedulerTaskTrigger : TimeSchedulerTaskTrigger
    {
        public override DateTime? CalculateNextTimeToRun(SchedulerTask task, SchedulerTaskState taskState, BaseTaskTriggerArgument taskTriggerArgument)
        {
            return DateTime.MaxValue;
            //MonthlyTimeTaskTriggerArgument monthlyTimeTaskTriggerArgument = (MonthlyTimeTaskTriggerArgument)taskTriggerArgument;

            //List<DateTime> listofScheduledDateTimes = new List<DateTime>();
            //var now = DateTime.Now;
            
            //var nextruntime = taskState.NextRunTime.HasValue && !monthlyTimeTaskTriggerArgument.IgnoreSkippedIntervals ? taskState.NextRunTime.Value : now;
                
            //foreach (DayOfMonth dayOfMonth in monthlyTimeTaskTriggerArgument.ScheduledDays)
            //{
            //    int lastDayOfMonth = DateTime.DaysInMonth(now.Year, now.Month);
            //    int day;
            //    switch (dayOfMonth.MonthlyValue)
            //    {
            //        case MonthlyEnum.LastDay:
            //            day = lastDayOfMonth;
            //            break;
            //        default:
            //            day = dayOfMonth.SpecificDay.Value;
            //            break;
            //    }
            //    int daysTillThen = day - nextruntime.Day;

            //    foreach (Time time in monthlyTimeTaskTriggerArgument.ScheduledTimesToRun)
            //    {
            //        TimeSpan scheduledTime = new TimeSpan(time.Hour, time.Minute, 0);
            //        TimeSpan spanTillThen = scheduledTime - nextruntime.TimeOfDay;

            //        int daysToAdd = daysTillThen;
            //        if ((daysTillThen < 0) || (daysTillThen == 0 && spanTillThen.Ticks <= 0))
            //            daysToAdd += lastDayOfMonth;

            //        listofScheduledDateTimes.Add(nextruntime.AddDays(daysToAdd).Add(spanTillThen));
            //    }
            //}

            //return listofScheduledDateTimes.OrderBy(x => x.Ticks).ToList().FirstOrDefault();
        }
    }
}
