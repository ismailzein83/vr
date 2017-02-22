using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Data
{
    public interface IBPInstanceDataManager : IDataManager
    {
        List<BPInstance> GetUpdated(ref byte[] maxTimeStamp, int nbOfRows, List<Guid> definitionsId, int parentId, string entityId);
        List<BPInstance> GetBeforeId(BPInstanceBeforeIdInput input);
        List<BPInstance> GetAllBPInstances(BPInstanceQuery query);

        List<BPInstance> GetPendingInstances(Guid definitionId, IEnumerable<BPInstanceStatus> acceptableBPStatuses, int maxCounts, Guid serviceInstanceId);

        List<BPPendingInstanceInfo> GetPendingInstancesInfo(IEnumerable<BPInstanceStatus> statuses, int nbOfInstancesToRetrieve);

        void UpdateInstanceStatus(long processInstanceId, BPInstanceStatus status, string message, Guid? workflowInstanceId);

        BPInstance GetBPInstance(long bpInstanceId);

        long InsertInstance(string processTitle, long? parentId, ProcessCompletionNotifier completionNotifier, Guid definitionID, object inputArguments, BPInstanceStatus executionStatus, int initiatorUserId, string entityId);

        void SetServiceInstancesOfBPInstances(List<BPPendingInstanceInfo> pendingInstancesToUpdate);
    }
}
