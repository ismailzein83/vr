using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing
{
    public class InterServicePendingSummaryBatchesRequest : Vanrise.Runtime.Entities.InterRuntimeServiceRequest<Object>
    {
        public Guid ServiceInstanceId { get; set; }

        public override object Execute()
        {
            PendingItemsHandler.Current.SetPendingSummaryItemsToProcess(this.ServiceInstanceId);
            return null;
        }
    }
}
