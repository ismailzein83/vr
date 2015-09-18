﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Runtime.Entities;
using Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments;

namespace Vanrise.Runtime.Triggers.TimeTaskTrigger
{
    public class DailyTimeSchedulerTaskTrigger : TimeSchedulerTaskTrigger
    {
        public override DateTime CalculateNextTimeToRun(SchedulerTask task, BaseTaskTriggerArgument taskTriggerArgument)
        {
            DailyTimeTaskTriggerArgument dailyTimeTaskTriggerArgument = (DailyTimeTaskTriggerArgument)taskTriggerArgument;

            List<DateTime> listofScheduledDateTimes = new List<DateTime>();

            foreach (Time time in dailyTimeTaskTriggerArgument.ScheduledTimesToRun)
            {
                TimeSpan scheduledTime = new TimeSpan(time.Hour, time.Minute, 0);
                TimeSpan spanTillThen = scheduledTime - DateTime.Now.TimeOfDay;

                int daysTillThen = 0;

                if (spanTillThen.Ticks < 0)
                    daysTillThen += 1;

                listofScheduledDateTimes.Add(DateTime.Now.AddDays(daysTillThen).Add(spanTillThen));
            }

            return listofScheduledDateTimes.OrderBy(x => x.Ticks).ToList().FirstOrDefault();
        }
    }
}
