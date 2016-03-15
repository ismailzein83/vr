using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess.Entities;
using Vanrise.BusinessProcess.Business;

namespace Vanrise.BusinessProcess.WFActivities
{
    public sealed class AssignTask<T> : NativeActivity
    {
        [RequiredArgument]
        public InArgument<string> TaskTitle { get; set; }

        [RequiredArgument]
        public InArgument<BPTaskData> TaskData { get; set; }

        [RequiredArgument]
        public InArgument<BPTaskAssignee> Users { get; set; }

        public OutArgument<BPTask> ExecutedTask { get; set; }

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
            var users = this.Users.Get(context);

            BPTaskManager bpTaskManager = new BPTaskManager();
            var createBPTaskInput = new CreateBPTaskInput
            {
                ProcessInstanceId = sharedData.InstanceInfo.ProcessInstanceID,
                Title = this.TaskTitle.Get(context),
                TaskData = taskData,
                TaskName = typeof(T).FullName,
                AssignedTo = users
            };

            var createTaskOutput = bpTaskManager.CreateTask(createBPTaskInput);
            if (createTaskOutput != null && createTaskOutput.Result == CreateBPTaskResult.Succeeded)
            {
                context.CreateBookmark(createTaskOutput.WFBookmarkName, OnTaskCompleted);
            }
            else
                throw new Exception(String.Format("Could not create Task. Title '{0}'", this.TaskTitle.Get(context)));
        }

        private void OnTaskCompleted(NativeActivityContext context,
              Bookmark bookmark,
              object value)
        {
            ExecuteBPTaskInput executeBPTaskInput = value as ExecuteBPTaskInput;
            if (executeBPTaskInput == null)
                throw new ArgumentNullException("ExecuteBPTaskInput");
            BPTask task;
            BPTaskManager bpTaskManager = new BPTaskManager();
            bpTaskManager.SetTaskCompleted(executeBPTaskInput, out task);
            context.SetValue(this.ExecutedTask, task);
        }
    }
}
