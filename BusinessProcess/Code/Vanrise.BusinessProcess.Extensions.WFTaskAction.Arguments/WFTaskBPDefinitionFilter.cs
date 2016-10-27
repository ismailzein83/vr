using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments
{
    public class WFTaskBPDefinitionFilter : ISchedulerTaskFilter
    {
        public Guid BPDefinitionId { get; set; }

        public bool IsMatched(SchedulerTask task)
        {
            if(task == null)
                throw new ArgumentNullException("task");

            if (task.TaskSettings == null)
                throw new ArgumentNullException("task.TaskSettings");
            var wfTaskActionArgument = task.TaskSettings.TaskActionArgument as WFTaskActionArgument;
            if (wfTaskActionArgument == null)
                return false;
            if (wfTaskActionArgument.BPDefinitionID != this.BPDefinitionId)
                return false;
            return true;
        }
    }
}
