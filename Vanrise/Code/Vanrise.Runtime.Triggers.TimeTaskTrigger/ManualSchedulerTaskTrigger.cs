using System;
using System.Collections.Generic;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Triggers.TimeTaskTrigger
{
    public class ManualSchedulerTaskTrigger : SchedulerTaskTrigger
    {
        public override bool UpdateNextRuntimeWhenNull { get { return false; } }

        public override Dictionary<string, object> EvaluateExpressions(SchedulerTask task, SchedulerTaskState taskState)
        {
            Dictionary<string, object> evaluatedExpressions = null;
            DateTime now = DateTime.Now;

            if (task.TaskSettings.TaskActionArgument != null && task.TaskSettings.TaskActionArgument.RawExpressions != null)
            {
                evaluatedExpressions = new Dictionary<string, object>();

                foreach (KeyValuePair<string, object> kvp in task.TaskSettings.TaskActionArgument.RawExpressions)
                {
                    object placeHolder = kvp.Value;
                    if (placeHolder.ToString() == "ScheduleTime")
                    {
                        Console.WriteLine("Original Time is {0}", now);
                        placeHolder = now;
                    }

                    evaluatedExpressions.Add(kvp.Key, placeHolder);
                }
            }

            return evaluatedExpressions;
        }

        public override DateTime? CalculateNextTimeToRun(SchedulerTask task, SchedulerTaskState taskState, BaseTaskTriggerArgument taskTriggerArgument)
        {
            return null;
        }
    }
}