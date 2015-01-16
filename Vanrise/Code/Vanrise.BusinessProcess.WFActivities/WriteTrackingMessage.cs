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
            var sharedData = context.GetExtension<BPSharedInstanceData>();
            if (sharedData == null)
                throw new NullReferenceException("BPSharedInstanceData");

            Console.WriteLine("{0}: {1}", DateTime.Now, this.Message.Get(context));
            BPTrackingSeverity severity = this.Severity.Get(context) ?? BPTrackingSeverity.Information;
            BPTrackingChannel.Current.WriteTrackingMessage(new BPTrackingMessage
            {
                ProcessInstanceId = sharedData.ProcessInstanceID,
                ParentProcessId = sharedData.ParentProcessID,
                Message = this.Message.Get(context),
                Severity = severity,
                EventTime = DateTime.Now
            });
        }
    }
}
