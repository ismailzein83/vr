using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Business
{
    public class WeeklyTimeSchedulerTaskTrigger : DailyTimeSchedulerTaskTrigger
    {
        public List<DayOfWeek> ScheduledDays { get; set; }

        public override DateTime CalculateNextTimeToRun()
        {
            List<DateTime> listofScheduledDateTimes = new List<DateTime>();

            foreach (DayOfWeek day in ScheduledDays)
            {
                foreach (string hour in base.ScheduledHours)
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
