using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public abstract class SchedulerTaskAction
    {
        public abstract SchedulerTaskExecuteOutput Execute(SchedulerTask task, BaseTaskActionArgument taskActionArgument, Dictionary<string, object> evaluatedExpressions);

        public virtual SchedulerTaskCheckProgressOutput CheckProgress(ISchedulerTaskCheckProgressContext context)
        {
            return new SchedulerTaskCheckProgressOutput() {Result = ExecuteOutputResult.Completed};
        }
    }
}
