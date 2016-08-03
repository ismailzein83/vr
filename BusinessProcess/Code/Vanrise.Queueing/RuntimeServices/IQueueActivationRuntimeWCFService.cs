using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing
{
    [ServiceContract(Namespace = "http://queueing.vanrise.com/IQueueActivationRuntimeWCFService")]
    internal interface IQueueActivationRuntimeWCFService
    {
        [OperationContract]
        void SetPendingQueuesToProcess(Guid activatorId, List<int> queueIds);
    }
}
