using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities.Persistence;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess
{
    public class BPSharedInstanceData : PersistenceParticipant
    {
        public BPInstance InstanceInfo { get; set; }

    }
}
