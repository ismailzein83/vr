using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Data
{
    public interface IBPInstanceDataManager : IDataManager
    {
        BPInstance GetBPInstance(long bpInstanceId);

        List<BPInstance> GetAllBPInstances(BPInstanceQuery query, List<int> grantedPermissionSetIds);

        List<BPInstance> GetUpdated(ref byte[] maxTimeStamp, int nbOfRows, List<Guid> definitionsId, int parentId, List<string> entityIds, List<int> grantedPermissionSetIds);

        List<BPInstance> GetBeforeId(BPInstanceBeforeIdInput input, List<int> grantedPermissionSetIds);

        List<BPInstance> GetAfterId(long? processInstanceId, Guid bpDefinitionId); 

        List<BPInstance> GetPendingInstances(Guid definitionId, IEnumerable<BPInstanceStatus> acceptableBPStatuses, int maxCounts, Guid serviceInstanceId);

        List<BPInstance> GetPendingInstancesInfo(IEnumerable<BPInstanceStatus> statuses, int nbOfInstancesToRetrieve);

        void UpdateInstanceStatus(long processInstanceId, BPInstanceStatus status, string message, Guid? workflowInstanceId);

        void UpdateInstanceLastMessage(long processInstanceId, string message);

        long InsertInstance(string processTitle, long? parentId, ProcessCompletionNotifier completionNotifier, Guid definitionID, object inputArguments, BPInstanceStatus executionStatus, int initiatorUserId, string entityId, int? viewInstanceRequiredPermissionSetId);

        void SetServiceInstancesOfBPInstances(List<BPInstance> pendingInstancesToUpdate);

        bool HasRunningInstances(Guid definitionId, List<string> entityIds, IEnumerable<BPInstanceStatus> statuses);
    }
}
