using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vanrise.BusinessProcess
{
    public class BPDefinition
    {
        public int BPDefinitionID { get; set; }
        public Type WorkflowType { get; set; } // Type should inherit System.Activities.Activity
        public BPConfiguration Configuration { get; set; }
    }

    public class BPConfiguration
    {
        public int? MaxConcurrentWorkflows { get; set; }
        public bool RetryOnProcessFailed { get; set; }
    }
}
