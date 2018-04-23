using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace Vanrise.BusinessProcess
{
    public class BusinessProcessService : RuntimeService
    {

        public override Guid ConfigId { get { return new Guid("AB860C8F-78C9-44E6-AE2A-3A7B7E7D7D12"); } }

        internal const string SERVICE_TYPE_UNIQUE_NAME = "VR_BusinessProcess_BusinessProcessService";

        public override string ServiceTypeUniqueName
        {
            get
            {
                return SERVICE_TYPE_UNIQUE_NAME;
            }
        }

        BusinessProcessRuntime _businessProcessRuntime;
        public override void OnStarted(IRuntimeServiceStartContext context)
        {
            _businessProcessRuntime = new BusinessProcessRuntime(base.ServiceInstance.ServiceInstanceId);
            base.OnStarted(context);
        }

        public override void Execute()
        {
            BPInstanceCancellationRequest cancellationRequest;
            while (PendingItemsHandler.Current.TryGetCancellationRequestToProcess(base.ServiceInstance.ServiceInstanceId, out cancellationRequest))
            {
                _businessProcessRuntime.ExecuteCancellationRequest(cancellationRequest);
            }
            Guid definitionId;
            while(PendingItemsHandler.Current.TryGetPendingDefinitionsToProcess(base.ServiceInstance.ServiceInstanceId, out definitionId))
            {
                _businessProcessRuntime.ExecutePendings(definitionId);
            }
        }
    }
}
