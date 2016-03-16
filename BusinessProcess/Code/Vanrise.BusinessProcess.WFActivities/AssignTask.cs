using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess.Entities;
using Vanrise.BusinessProcess.Business;

namespace Vanrise.BusinessProcess.WFActivities
{
    public sealed class AssignTask<T> : NativeActivity where T : BPTaskExecutionInformation
    {
        [RequiredArgument]
        public InArgument<string> TaskTitle { get; set; }

        [RequiredArgument]
        public InArgument<BPTaskData> TaskData { get; set; }

        [RequiredArgument]
        public InArgument<BPTaskAssignee> AssignedTo { get; set; }

        public OutArgument<BPTask> ExecutedTask { get; set; }

        public OutArgument<T> TaskExecutionInformation { get; set; }
        protected override bool CanInduceIdle
        {
            get
            {
                return true;
            }
        }

        protected override void Execute(NativeActivityContext context)
        {
            var sharedData = context.GetSharedInstanceData();

            var taskData = this.TaskData.Get(context);
            var assignedTo = this.AssignedTo.Get(context);

            BPTaskAssigneeContext bpTaskAssigneeContext = new BPTaskAssigneeContext() { ProcessInitiaterUserId = sharedData.InstanceInfo.InitiatorUserId };

            var assignedUserIds = assignedTo.GetUserIds(bpTaskAssigneeContext);
            if (assignedUserIds == null || assignedUserIds.Count() == 0)
                throw new Exception(String.Format("Could not resolve AssignedTo '{0}'", AssignedTo));

            BPTaskManager bpTaskManager = new BPTaskManager();
            var createBPTaskInput = new CreateBPTaskInput
            {
                ProcessInstanceId = sharedData.InstanceInfo.ProcessInstanceID,
                Title = this.TaskTitle.Get(context),
                TaskData = taskData,
                TaskName = taskData.TaskType,
                AssignedUserIds = assignedUserIds.ToList(),
                AssignedUserIdsDescription = assignedTo.GetDescription(bpTaskAssigneeContext),
            };

            var createTaskOutput = bpTaskManager.CreateTask(createBPTaskInput);
            if (createTaskOutput != null && createTaskOutput.Result == CreateBPTaskResult.Succeeded)
            {
                context.CreateBookmark(createTaskOutput.WFBookmarkName, OnTaskCompleted);
            }
            else
                throw new Exception(String.Format("Could not create Task. Title '{0}'", this.TaskTitle.Get(context)));
        }

        private void OnTaskCompleted(NativeActivityContext context, Bookmark bookmark, object value)
        {
            ExecuteBPTaskInput executeBPTaskInput = value as ExecuteBPTaskInput;
            if (executeBPTaskInput == null)
                throw new ArgumentNullException("ExecuteBPTaskInput");
            BPTask task;
            BPTaskManager bpTaskManager = new BPTaskManager();
            bpTaskManager.SetTaskCompleted(executeBPTaskInput, out task);
            context.SetValue(this.ExecutedTask, task);
            context.SetValue(this.TaskExecutionInformation, executeBPTaskInput.ExecutionInformation as T);
        }
    }
}
