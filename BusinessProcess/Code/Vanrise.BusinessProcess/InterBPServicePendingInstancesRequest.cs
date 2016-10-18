using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess
{
    public class InterBPServicePendingInstancesRequest : Vanrise.Runtime.Entities.InterRuntimeServiceRequest<Object>
    {
        public Guid ServiceInstanceId { get; set; }

        public List<Guid> PendingBPDefinitionIds { get; set; }

        public override object Execute()
        {
            PendingItemsHandler.Current.SetPendingDefinitionsToProcess(this.ServiceInstanceId, this.PendingBPDefinitionIds);
            return null;
        }
    }
}
