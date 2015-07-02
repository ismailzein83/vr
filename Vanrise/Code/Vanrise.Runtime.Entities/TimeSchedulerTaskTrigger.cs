using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public class TimeSchedulerTaskTrigger : SchedulerTaskTrigger
    {
        //DateTime _dateToRun;
        //public DateTime DateToRun
        //{
        //    get
        //    {
        //        return _dateToRun;
        //    }
        //    set
        //    {
        //        _dateToRun = value.ToLocalTime();
        //    }
        //}

        public List<DayOfWeek> ScheduledDays { get; set; }

        public List<string> ScheduledHours { get; set; }

        public override Dictionary<string, string> EvaluateExpressions(Dictionary<string, string> rawExpressions)
        {
            Dictionary<string, string> evaluatedExpressions = null;

            if (rawExpressions != null)
            {
                evaluatedExpressions = new Dictionary<string, string>();

                foreach (KeyValuePair<string, string> kvp in rawExpressions)
                {
                    string placeHolder = kvp.Value;
                    if (placeHolder == "ScheduleTime")
                    {
                        Console.WriteLine("Original Time is {0}", CalculateNextTimeToRun());
                        placeHolder = CalculateNextTimeToRun().ToString();
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
