using System;
using System.Collections.Generic;

namespace Vanrise.Runtime.Entities
{
    public abstract class SchedulerTaskTrigger
    {
        public abstract DateTime? CalculateNextTimeToRun(SchedulerTask task, SchedulerTaskState taskState, BaseTaskTriggerArgument taskTriggerArgument);

        public abstract Dictionary<string, object> EvaluateExpressions(SchedulerTask task, SchedulerTaskState taskState);
    }
}
