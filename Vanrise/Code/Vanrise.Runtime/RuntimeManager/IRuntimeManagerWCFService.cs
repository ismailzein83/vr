using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime
{
    public interface IRuntimeManagerWCFService
    {
        PingPrimaryNodeResponse PingPrimaryNode(PingPrimaryNodeRequest request);

        void UnlockFreezedTransactions(List<TransactionLockItem> freezedTransactionLocks);

        bool TryLockRuntimeService(string serviceTypeUniqueName, int runtimeProcessId);

        GetServiceProcessIdResponse TryGetServiceProcessId(GetServiceProcessIdRequest request);

        bool TryLock(TransactionLockItem lockItem, int maxAllowedConcurrency);

        void UnLock(TransactionLockItem lockItem);

        string RegisterRunningProcess(string serializedInput);

        bool IsThisRuntimeNodeInstance(Guid runtimeNodeId, Guid instanceId);

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
