using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities.Persistence;

namespace Vanrise.BusinessProcess
{
    public class BPSharedInstanceData : PersistenceParticipant
    {
        public int ProcessDefinitionId { get; set; }
        public long ProcessInstanceId { get; set; }
        public long? ParentProcessId { get; set; }

    }
}
