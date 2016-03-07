using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;
using Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments;

namespace Vanrise.Runtime.Triggers.TimeTaskTrigger
{
    public class TimeSchedulerTaskTrigger : SchedulerTaskTrigger
    {
        public override Dictionary<string, object> EvaluateExpressions(SchedulerTask task, SchedulerTaskState taskState)
        {
            Dictionary<string, object> evaluatedExpressions = null;

            if (task.TaskSettings.TaskActionArgument != null && task.TaskSettings.TaskActionArgument.RawExpressions != null)
            {
                evaluatedExpressions = new Dictionary<string, object>();

                foreach (KeyValuePair<string, object> kvp in task.TaskSettings.TaskActionArgument.RawExpressions)
                {
                    object placeHolder = kvp.Value;
                    if (placeHolder.ToString() == "ScheduleTime")
                    {
                        Console.WriteLine("Original Time is {0}", taskState.NextRunTime);
                        placeHolder = taskState.NextRunTime;
                    }

                    evaluatedExpressions.Add(kvp.Key, placeHolder);
                }
            }

            return evaluatedExpressions;
        }

        public override DateTime CalculateNextTimeToRun(SchedulerTask task, SchedulerTaskState taskState, BaseTaskTriggerArgument taskTriggerArgument)
        {
            TimeTaskTriggerArgument timeTaskTriggerArgument = (TimeTaskTriggerArgument)taskTriggerArgument;
            TimeSchedulerTaskTrigger timerTaskTrigger = (TimeSchedulerTaskTrigger)Activator.CreateInstance(Type.GetType(timeTaskTriggerArgument.TimerTriggerTypeFQTN));

            return timerTaskTrigger.CalculateNextTimeToRun(task, taskState, timeTaskTriggerArgument);
        }
    }
}
