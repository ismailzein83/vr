using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Data
{
    public interface IBPInstanceDataManager : IDataManager
    {
        BPInstance GetBPInstance(long bpInstanceId, bool getFromArchive);

        List<BPInstance> GetFilteredBPInstances(BPInstanceQuery query, List<int> grantedPermissionSetIds, bool getFromArchive);

        List<BPInstance> GetFirstPage(out byte[] maxTimeStamp, int nbOfRows, List<Guid> definitionsId, int parentId, List<string> entityIds, List<int> grantedPermissionSetIds);
        
        List<Entities.BPInstance> GetFirstPageFromArchive(int nbOfRows, List<Guid> definitionsId, int parentId, List<string> entityIds, List<int> grantedPermissionSetIds);

        List<BPInstance> GetUpdated(ref byte[] maxTimeStamp, int nbOfRows, List<Guid> definitionsId, int parentId, List<string> entityIds, List<int> grantedPermissionSetIds);

        List<BPInstance> GetBeforeId(BPInstanceBeforeIdInput input, List<int> grantedPermissionSetIds, bool getFromArchive);

        List<BPInstance> GetAfterId(long? processInstanceId, Guid bpDefinitionId, bool getFromArchive);

        List<BPInstance> GetPendingInstances(Guid definitionId, IEnumerable<BPInstanceStatus> acceptableBPStatuses, BPInstanceAssignmentStatus assignmentStatus, int maxCounts, Guid serviceInstanceId);

        List<BPInstance> GetPendingInstancesInfo(IEnumerable<BPInstanceStatus> statuses);

        void UpdateInstanceStatus(long processInstanceId, BPInstanceStatus status, BPInstanceAssignmentStatus assignmentStatus, string message, bool clearServiceInstanceId, Guid? workflowInstanceId);

        void UpdateInstanceLastMessage(long processInstanceId, string message);

        long InsertInstance(string processTitle, long? parentId, ProcessCompletionNotifier completionNotifier, Guid definitionID, object inputArguments, BPInstanceStatus executionStatus, int initiatorUserId, string entityId, int? viewInstanceRequiredPermissionSetId, Guid? taskId);


        void UpdateServiceInstancesAndAssignmentStatus(List<BPInstance> pendingInstancesToUpdate);

        bool HasRunningInstances(Guid definitionId, List<string> entityIds, IEnumerable<BPInstanceStatus> statuses);

        List<BPDefinitionSummary> GetBPDefinitionSummary(IEnumerable<BPInstanceStatus> executionStatus);

        void UpdateInstanceAssignmentStatus(long processInstanceId, BPInstanceAssignmentStatus assignmentStatus);

        void SetCancellationRequestUserId(long bpInstanceId, List<BPInstanceStatus> allowedStatuses, int cancelRequestByUserId);

        void ArchiveInstances(List<BPInstanceStatus> completedStatuses, DateTime completedBefore, int nbOfInstances);
    }
}