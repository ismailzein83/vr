using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities.Persistence;

namespace Vanrise.BusinessProcess
{
    public class BPSharedInstanceData : PersistenceParticipant
    {
        public long ProcessInstanceID { get; set; }
        public long? ParentProcessID { get; set; }
    }
}
