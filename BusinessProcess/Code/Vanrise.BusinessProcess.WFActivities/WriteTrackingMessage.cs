using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.WFActivities
{

    public sealed class WriteTrackingMessage : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> Message { get; set; }

        public InArgument<LogEntryType?> Severity { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var sharedData = context.GetSharedInstanceData();

            Console.WriteLine("{0}: {1}", DateTime.Now, this.Message.Get(context));
            LogEntryType severity = this.Severity.Get(context) ?? LogEntryType.Information;
            context.WriteTrackingMessage(severity, this.Message.Get(context));
        }
    }
}
