using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPPendingInstanceInfo
    {
        public int BPDefinitionId { get; set; }

        public long ProcessInstanceId { get; set; }

        public long? ParentProcessInstanceId { get; set; }

        public BPInstanceStatus Status { get; set; }

        public Guid? ServiceInstanceId { get; set; }
    }
}
