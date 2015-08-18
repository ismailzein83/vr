using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;
using Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments;

namespace Vanrise.Runtime.Triggers.TimeTaskTrigger
{
    public class WeeklyTimeSchedulerTaskTrigger : TimeSchedulerTaskTrigger
    {
        public override DateTime CalculateNextTimeToRun(BaseTaskTriggerArgument taskTriggerArgument)
        {
            WeeklyTimeTaskTriggerArgument weeklyTimeTaskTriggerArgument = (WeeklyTimeTaskTriggerArgument)taskTriggerArgument;

            List<DateTime> listofScheduledDateTimes = new List<DateTime>();

            foreach (DayOfWeek day in weeklyTimeTaskTriggerArgument.ScheduledDays)
            {
                foreach (string hour in weeklyTimeTaskTriggerArgument.ScheduledHours)
                {
                    string[] timeParts = hour.Split(':');

                    TimeSpan scheduledTime = new TimeSpan(int.Parse(timeParts[0]), int.Parse(timeParts[1]), 0);
                    TimeSpan spanTillThen = scheduledTime - DateTime.Now.TimeOfDay;

                    int daysTillThen = (int)day - (int)DateTime.Today.DayOfWeek;

                    if ((daysTillThen < 0) || (daysTillThen == 0 && spanTillThen.Ticks < 0))
                        daysTillThen += 7;

                    listofScheduledDateTimes.Add(DateTime.Now.AddDays(daysTillThen).Add(spanTillThen));
                }
            }

            return listofScheduledDateTimes.OrderBy(x => x.Ticks).ToList().FirstOrDefault();
        }
    }
}
