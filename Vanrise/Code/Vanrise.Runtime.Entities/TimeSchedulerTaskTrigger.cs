using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public class TimeSchedulerTaskTrigger : SchedulerTaskTrigger
    {
        DateTime _dateToRun;
        public DateTime DateToRun
        {
            get
            {
                return _dateToRun;
            }
            set
            {
                _dateToRun = value.ToLocalTime();
            }
        }

        public string TimeToRun { get; set; }

        public override bool CheckIfTimeToRun()
        {
            string[] timeParts = TimeToRun.Split(':');

            DateToRun = DateToRun.Date;

            if (timeParts.Length > 0)
            {
                DateToRun = DateToRun.AddHours(double.Parse(timeParts[0]));

                if (timeParts.Length > 1 && timeParts[1] != null)
                {
                    DateToRun = DateToRun.AddMinutes(double.Parse(timeParts[1]));
                }

                if (timeParts.Length > 2 && timeParts[2] != null)
                {
                    DateToRun = DateToRun.AddSeconds(double.Parse(timeParts[2]));
                }
            }

            return DateToRun.Date.Equals(DateTime.Now.Date)
                && DateToRun.Hour.Equals(DateTime.Now.Hour)
                && DateToRun.Minute.Equals(DateTime.Now.Minute);
        }


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
                        Console.WriteLine("Original Time is {0}", DateToRun);
                        placeHolder = DateToRun.ToString();
                    }

                    evaluatedExpressions.Add(kvp.Key, placeHolder);
                }
            }

            return evaluatedExpressions;
        }
    }
}
