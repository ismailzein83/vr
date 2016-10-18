using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess
{
    public class InterBPServicePendingEventsRequest : Vanrise.Runtime.Entities.InterRuntimeServiceRequest<Object>
    {
        public Guid ServiceInstanceId { get; set; }

        public List<Guid> PendingEventsBPDefinitionIds { get; set; }
        public override object Execute()
        {
            PendingItemsHandler.Current.SetPendingEventsDefinitionsToProcess(this.ServiceInstanceId, this.PendingEventsBPDefinitionIds);
            return null;
        }
    }
}
