using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.WFActivities
{
    public sealed class AssignTask : NativeActivity
    {
        
        [RequiredArgument]
        public InArgument<string> TaskTitle { get; set; }

        [RequiredArgument]
        public InArgument<BPTaskInformation> TaskInformation { get; set; }

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

            var taskInformation = this.TaskInformation.Get(context);

            BPTaskManager bpTaskManager = new BPTaskManager();
            var createBPTaskInput = new CreateBPTaskInput
            {
                ProcessInstanceId = sharedData.InstanceInfo.ProcessInstanceID,
                Title = this.TaskTitle.Get(context),
                TaskInformation = taskInformation
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
