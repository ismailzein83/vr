using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.MainExtensions.BPTaskTypes
{
    public class BPGenericTaskTypeActionManager
    {
        public List<BPGenericTaskTypeAction> GetTaskTypeActions(long taskId)
        {
            var taskTypeActions = new List<BPGenericTaskTypeAction>();
            BPTaskTypeManager taskTypeManager = new BPTaskTypeManager();
            BPTaskManager taskmanager = new BPTaskManager();
            var task = taskmanager.GetTask(taskId);
            task.ThrowIfNull("task", taskId);
            var taskType = taskTypeManager.GetBPTaskType(task.TypeId);
            taskType.ThrowIfNull("taskType", task.TypeId);
            taskType.Settings.ThrowIfNull("taskType.Settings", task.TypeId);
            var genericTaskTypeSettings = taskType.Settings.CastWithValidate<BPGenericTaskTypeSettings>("genericTaskTypeSettings");
            if (genericTaskTypeSettings.TaskTypeActions != null && genericTaskTypeSettings.TaskTypeActions.Count > 0)
            {
                var context = new BPGenericTaskTypeActionFilterConditionContext()
                {
                    Task = task
                };
                foreach (var action in genericTaskTypeSettings.TaskTypeActions)
                {
                    if (action.FilterCondition == null || action.FilterCondition.IsFilterMatch(context))
                        taskTypeActions.Add(action);
                }
            }
            return taskTypeActions;
        }
    }
}
