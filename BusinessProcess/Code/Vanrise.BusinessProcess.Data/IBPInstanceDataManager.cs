using System;
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

        List<BPInstance> GetPendingInstances(int definitionId, IEnumerable<BPInstanceStatus> acceptableBPStatuses, int maxCounts, Guid serviceInstanceId);

        List<BPPendingInstanceInfo> GetPendingInstancesInfo(IEnumerable<BPInstanceStatus> statuses);

        void UpdateInstanceStatus(long processInstanceId, BPInstanceStatus status, string message, Guid? workflowInstanceId);

        BPInstance GetBPInstance(long bpInstanceId);

        long InsertInstance(string processTitle, long? parentId, int definitionID, object inputArguments, BPInstanceStatus executionStatus, int initiatorUserId,string entityId);

        void SetServiceInstancesOfBPInstances(List<BPPendingInstanceInfo> pendingInstancesToUpdate);
    }
}
