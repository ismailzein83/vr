using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess
{
    internal class InterBPServiceCancelInstanceRequest : Vanrise.Runtime.Entities.InterRuntimeServiceRequest<Object>
    {
        public Guid ServiceInstanceId { get; set; }

        public BPInstanceCancellationRequest CancellationRequest { get; set; }

        public override object Execute()
        {
            PendingItemsHandler.Current.AddCancellationRequest(this.ServiceInstanceId, this.CancellationRequest);
            return null;
        }
    }
}
