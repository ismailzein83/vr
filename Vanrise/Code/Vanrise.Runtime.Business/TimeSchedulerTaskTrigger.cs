using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Business
{
    public class TimeSchedulerTaskTrigger : SchedulerTaskTrigger
    {
        public List<DayOfWeek> ScheduledDays { get; set; }

        public List<string> ScheduledHours { get; set; }

        public override Dictionary<string, object> EvaluateExpressions(SchedulerTask task)
        {
            Dictionary<string, object> evaluatedExpressions = null;

            if (task.TaskAction.RawExpressions != null)
            {
                evaluatedExpressions = new Dictionary<string, object>();

                foreach (KeyValuePair<string, object> kvp in task.TaskAction.RawExpressions)
                {
                    object placeHolder = kvp.Value;
                    if (placeHolder.ToString() == "ScheduleTime")
                    {
                        Console.WriteLine("Original Time is {0}", task.NextRunTime);
                        placeHolder = task.NextRunTime;
                    }

                    evaluatedExpressions.Add(kvp.Key, placeHolder);
                }
            }

            return evaluatedExpressions;
        }

        public override DateTime CalculateNextTimeToRun()
        {
            List<DateTime> listofScheduledDateTimes = new List<DateTime>();

            foreach (DayOfWeek day in ScheduledDays)
            {
                foreach (string hour in ScheduledHours)
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
