using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Data
{
    public interface IBPInstanceDataManager : IDataManager
    {
        List<BPInstance> GetUpdated(ref byte[] maxTimeStamp, int nbOfRows, List<int> definitionsId, int parentId, string entityId);
        List<BPInstance> GetBeforeId(BPInstanceBeforeIdInput input);
        BigResult<BPInstanceDetail> GetFilteredBPInstances(DataRetrievalInput<BPInstanceQuery> input);

        List<BPInstance> GetPendingInstances(int definitionId, IEnumerable<BPInstanceStatus> acceptableBPStatuses, int maxCounts, int currentRuntimeProcessId, IEnumerable<int> runningRuntimeProcessesIds);

        bool TryLockProcessInstance(long processInstanceId, System.Guid workflowInstanceId, int currentRuntimeProcessId, IEnumerable<int> runningRuntimeProcessesIds, IEnumerable<BPInstanceStatus> acceptableBPStatuses);

        void UnlockProcessInstance(long processInstanceId, int currentRuntimeProcessId);

        void UpdateInstanceStatus(long processInstanceId, BPInstanceStatus status, string message, int retryCount);

        void SetRunningStatusTerminated(BPInstanceStatus bPInstanceStatus, IEnumerable<int> runningRuntimeProcessesIds);

        //void SetChildrenStatusesTerminated(IEnumerable<BPInstanceStatus> enumerable, IEnumerable<int> runningRuntimeProcessesIds);

        BPInstance GetBPInstance(long bpInstanceId);

        bool TryGetBPInstanceStatus(long bpInstanceId, out BPInstanceStatus instanceStatus);

        long InsertInstance(string processTitle, long? parentId, int definitionID, object inputArguments, BPInstanceStatus executionStatus, int initiatorUserId,string entityId);       
    }
}
