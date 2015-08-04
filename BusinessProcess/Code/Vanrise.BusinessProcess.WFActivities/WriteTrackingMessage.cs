using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.WFActivities
{

    public sealed class WriteTrackingMessage : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> Message { get; set; }

        public InArgument<BPTrackingSeverity?> Severity { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var sharedData = context.GetSharedInstanceData();

            Console.WriteLine("{0}: {1}", DateTime.Now, this.Message.Get(context));
            BPTrackingSeverity severity = this.Severity.Get(context) ?? BPTrackingSeverity.Information;
            BPTrackingChannel.Current.WriteTrackingMessage(new BPTrackingMessage
            {
                ProcessInstanceId = sharedData.InstanceInfo.ProcessInstanceID,
                ParentProcessId = sharedData.InstanceInfo.ParentProcessID,
                TrackingMessage = this.Message.Get(context),
                Severity = severity,
                EventTime = DateTime.Now
            });
        }
    }
}
