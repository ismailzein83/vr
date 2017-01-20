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
    public class WeeklyTimeSchedulerTaskTrigger : TimeSchedulerTaskTrigger
    {
        public override DateTime CalculateNextTimeToRun(SchedulerTask task, SchedulerTaskState taskState, BaseTaskTriggerArgument taskTriggerArgument)
        {
            WeeklyTimeTaskTriggerArgument weeklyTimeTaskTriggerArgument = (WeeklyTimeTaskTriggerArgument)taskTriggerArgument;

            List<DateTime> listofScheduledDateTimes = new List<DateTime>();
            var now = DateTime.Now;

            foreach (DayOfWeek day in weeklyTimeTaskTriggerArgument.ScheduledDays)
            {
                foreach (Time time in weeklyTimeTaskTriggerArgument.ScheduledTimesToRun)
                {
                    TimeSpan scheduledTime = new TimeSpan(time.Hour, time.Minute, 0);
                    TimeSpan spanTillThen = scheduledTime - now.TimeOfDay;

                    int daysTillThen = (int)day - (int)DateTime.Today.DayOfWeek;

                    if ((daysTillThen < 0) || (daysTillThen == 0 && spanTillThen.Ticks < 0))
                        daysTillThen += 7;

                    listofScheduledDateTimes.Add(now.AddDays(daysTillThen).Add(spanTillThen));
                }
            }

            return listofScheduledDateTimes.OrderBy(x => x.Ticks).ToList().FirstOrDefault();
        }
    }
}
