using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Entities;
using Vanrise.BusinessProcess.Business;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.WFActivities
{

    public sealed class RunProcess : NativeActivity
    {
        // Define an activity input argument of type string       

        public InArgument<BaseProcessInputArgument> Input { get; set; }
        public InArgument<bool> WaitProcessCompleted { get; set; }
        public OutArgument<long> ProcessInstanceId { get; set; }
        public OutArgument<ProcessCompletedEventPayload> ProcessCompletedEventPayload { get; set; }

        public InArgument<bool> TerminateIfFailed { get; set; }

        public InArgument<string> TerminationMessage { get; set; }

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
            input.InputArguments.UserId = sharedData.InstanceInfo.InitiatorUserId;
            BPInstanceManager manager = new BPInstanceManager();
            var output = manager.CreateNewProcess(input);
            this.ProcessInstanceId.Set(context, output.ProcessInstanceId);

            if (this.WaitProcessCompleted.Get(context))
                context.CreateBookmark(output.ProcessInstanceId.ToString(), OnProcessCompleted);
        }

        private void OnProcessCompleted(NativeActivityContext context,
              Bookmark bookmark,
              object value)
        {
            ProcessCompletedEventPayload processCompletedEventPayload = value as ProcessCompletedEventPayload;
            context.SetValue(this.ProcessCompletedEventPayload, processCompletedEventPayload);
            if(processCompletedEventPayload != null && processCompletedEventPayload.ProcessStatus != BPInstanceStatus.Completed && this.TerminateIfFailed.Get(context))
            {
                string errorMessage = string.Format("{0}. Error: {1}", this.TerminationMessage.Get(context), processCompletedEventPayload.ErrorBusinessMessage);
                if (processCompletedEventPayload.ExceptionDetail != null)
                    throw new VRBusinessException(errorMessage, new Exception(processCompletedEventPayload.ExceptionDetail));
                else
                    throw new VRBusinessException(errorMessage);
            }
        }
    }
}
