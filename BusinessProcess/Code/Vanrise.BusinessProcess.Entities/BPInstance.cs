using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPInstance
    {
        public Guid ProcessInstanceID { get; set; }
        public string Title { get; set; }
        public Guid? ParentProcessID { get; set; }
        public int DefinitionID { get; set; }
        public Guid? WorkflowInstanceID { get; set; }
        public BPInstanceStatus Status { get; set; }
        public int RetryCount { get; set; }
        public object InputArgument { get; set; }
        public string LastMessage { get; set; }
    }
}
