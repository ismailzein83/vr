using System;
using Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments;
using Vanrise.Runtime.Entities;

namespace Vanrise.BusinessProcess.MainExtensions.SchedulerTask
{
    public class BPDefinitionSchedulerTaskFilter : ISchedulerTaskFilter
    {
        public Guid BPDefinitionId { get; set; }
        public bool IsMatched(Runtime.Entities.SchedulerTask task)
        {
            if (task == null || task.TaskSettings == null || task.TaskSettings.TaskActionArgument == null)
                return false;

            WFTaskActionArgument taskActionArgument = task.TaskSettings.TaskActionArgument as WFTaskActionArgument;
            if (taskActionArgument == null || taskActionArgument.BPDefinitionID != this.BPDefinitionId)
                return false;

            return true;
        }
    }
}