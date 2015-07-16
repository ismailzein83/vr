using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Business
{
    public class DailyTimeSchedulerTaskTrigger : TimeSchedulerTaskTrigger
    {
        public List<string> ScheduledHours { get; set; }

        public override DateTime CalculateNextTimeToRun()
        {
            List<DateTime> listofScheduledDateTimes = new List<DateTime>();

            foreach (string hour in ScheduledHours)
            {
                string[] timeParts = hour.Split(':');

                TimeSpan scheduledTime = new TimeSpan(int.Parse(timeParts[0]), int.Parse(timeParts[1]), 0);
                TimeSpan spanTillThen = scheduledTime - DateTime.Now.TimeOfDay;

                int daysTillThen = 0;

                if (spanTillThen.Ticks < 0)
                    daysTillThen += 7;

                listofScheduledDateTimes.Add(DateTime.Now.AddDays(daysTillThen).Add(spanTillThen));
            }

            return listofScheduledDateTimes.OrderBy(x => x.Ticks).ToList().FirstOrDefault();
        }
    }
}
