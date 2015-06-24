using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Entities;
using Vanrise.BusinessProcess.Client;

namespace Vanrise.BusinessProcess.WFActivities
{

    public sealed class RunProcess : NativeActivity
    {
        // Define an activity input argument of type string       

        public InArgument<BaseProcessInputArgument> Input { get; set; }
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
            var sharedData = context.GetSharedInstanceData();

            var input = new CreateProcessInput
            {
                InputArguments = this.Input.Get(context),
                ParentProcessID = sharedData.InstanceInfo.ProcessInstanceID
            };
            BPClient client = new BPClient();
            var output = client.CreateNewProcess(input);
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
