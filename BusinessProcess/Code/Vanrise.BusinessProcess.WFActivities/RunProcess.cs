using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.WFActivities
{

    public sealed class RunProcess : NativeActivity
    {
        // Define an activity input argument of type string

        [RequiredArgument]
        public InArgument<string> ProcessName { get; set; }

        public InArgument<object> Input { get; set; }
        public InArgument<bool> WaitProcessCompleted { get; set; }
        public OutArgument<long> ProcessInstanceId { get; set; }
        public OutArgument<ProcessCompletedEventPayload> ProcessCompletedEventPayload { get; set; }

        protected override bool CanInduceIdle
        {
            get
            {
                return true;
            }
        }              

        protected override void Execute(NativeActivityContext context)
        {
            var sharedData = context.GetExtension<BPSharedInstanceData>();
            if (sharedData == null)
                throw new NullReferenceException("BPSharedInstanceData");

            var input = new CreateProcessInput
            {
                ProcessName = this.ProcessName.Get(context),
                InputArguments = this.Input.Get(context),
                ParentProcessID = sharedData.ProcessInstanceId
            };
            ProcessManager processManager = new ProcessManager();
            var output = processManager.CreateNewProcess(input);
            this.ProcessInstanceId.Set(context, output.ProcessInstanceId);

            if (this.WaitProcessCompleted.Get(context))
                context.CreateBookmark(output.ProcessInstanceId.ToString(), OnProcessCompleted);
        }

        private void OnProcessCompleted(NativeActivityContext context,
              Bookmark bookmark,
              object value)
        {
            context.SetValue(this.ProcessCompletedEventPayload, value as ProcessCompletedEventPayload);
        }
    }
}
