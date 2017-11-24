using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vanrise.BusinessProcess.Entities
{
    public class CreateProcessInput
    {
        public long? ParentProcessID { get; set; }

        public Guid? TaskId { get; set; }

        public BaseProcessInputArgument InputArguments { get; set; }

        public ProcessCompletionNotifier CompletionNotifier { get; set; }
    }

    public abstract class ProcessCompletionNotifier
    {
        public abstract void OnProcessInstanceCompleted(ProcessCompletedEventPayload eventPayload);
    }
}
