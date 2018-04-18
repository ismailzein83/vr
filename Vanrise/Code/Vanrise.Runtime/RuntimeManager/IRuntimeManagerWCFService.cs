using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime
{
    [ServiceContract(Namespace = "http://runtime.vanrise.com/IRuntimeManagerWCFService")]
    internal interface IRuntimeManagerWCFService
    {
        [OperationContract]
        PingPrimaryNodeResponse PingPrimaryNode(PingPrimaryNodeRequest request);
        
        [OperationContract]
        void UnlockFreezedTransactions(List<TransactionLockItem> freezedTransactionLocks);

        [OperationContract]
        bool TryLockRuntimeService(string serviceTypeUniqueName, int runtimeProcessId);

        [OperationContract]
        GetServiceProcessIdResponse TryGetServiceProcessId(GetServiceProcessIdRequest request);

        [OperationContract]
        bool TryLock(TransactionLockItem lockItem, int maxAllowedConcurrency);

        [OperationContract]
        void UnLock(TransactionLockItem lockItem);

        [OperationContract]
        string RegisterRunningProcess(string serializedInput);

        [OperationContract]
        bool IsThisRuntimeNodeInstance(Guid runtimeNodeId, Guid instanceId);

        [OperationContract]
        void SetRuntimeProcessesAndServicesChanged();
    }

    public class PingPrimaryNodeRequest
    {
        public Guid RuntimeNodeInstanceId { get; set; }

        public bool RunningProcessesChangedInCurrentNode { get; set; }
    }

    public class PingPrimaryNodeResponse
    {
    }

    public class GetServiceProcessIdRequest
    {
        public string ServiceTypeUniqueName { get; set; }
    }

    public class GetServiceProcessIdResponse
    {
        public int? RuntimeProcessId { get; set; }
    }
}
