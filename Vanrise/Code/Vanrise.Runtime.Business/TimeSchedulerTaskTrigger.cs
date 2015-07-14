using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Business
{
    public enum TimeSchedulerType { Interval = 0, Daily = 1, Weekly = 2};

    public class TimeSchedulerTaskTrigger : SchedulerTaskTrigger
    {
        public TimeSchedulerType SelectedType { get; set; }

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
            return DateTime.MinValue;
        }
    }
}
