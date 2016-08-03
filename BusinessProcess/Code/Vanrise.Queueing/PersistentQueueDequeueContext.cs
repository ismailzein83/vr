using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing
{
    public class PersistentQueueDequeueContext : IPersistentQueueDequeueContext
    {
        public Guid? ActivatorInstanceId
        {
            get;
            set;
        }
    }
}
