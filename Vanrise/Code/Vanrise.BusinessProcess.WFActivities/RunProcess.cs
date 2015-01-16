using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;

namespace Vanrise.BusinessProcess.WFActivities
{

    public sealed class RunProcess<T> : NativeActivity where T : Activity, IBPWorkflow
    {
        // Define an activity input argument of type string
        public InArgument<object> Input { get; set; }
        public InArgument<bool> WaitProcessCompleted { get; set; }
        public OutArgument<Guid> ProcessInstanceId { get; set; }
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
                InputArguments = this.Input.Get(context),
                ParentProcessID = sharedData.ProcessInstanceID
            };
            var output = BusinessProcessRuntime.Current.CreateNewProcess<T>(input);
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
