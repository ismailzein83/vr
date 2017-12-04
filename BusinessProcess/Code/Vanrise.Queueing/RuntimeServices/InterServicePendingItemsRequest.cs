using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing
{
    public class InterServicePendingItemsRequest : Vanrise.Runtime.Entities.InterRuntimeServiceRequest<Object>
    {
        public Guid ServiceInstanceId { get; set; }

        public List<int> QueueIds { get; set; }

        public override object Execute()
        {
            PendingItemsHandler.Current.SetPendingQueuesToProcess(this.ServiceInstanceId, this.QueueIds);
            return null;
        }
    }
}
