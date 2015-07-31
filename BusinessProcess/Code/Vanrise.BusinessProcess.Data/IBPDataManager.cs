using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
namespace Vanrise.BusinessProcess.Data
{
    public interface IBPDataManager : IDataManager
    {
        int DeleteEvent(long eventId);

        BPDefinition GetDefinition(int ID);

        List<BPDefinition> GetFilteredDefinitions(string title);

        System.Collections.Generic.List<BPDefinition> GetDefinitions();

        T GetDefinitionObjectState<T>(int definitionId, string objectKey);

        int InsertDefinitionObjectState(int definitionId, string objectKey, object objectValue);

        int UpdateDefinitionObjectState(int definitionId, string objectKey, object objectValue);

        Vanrise.Entities.BigResult<BPInstance> GetInstancesByCriteria(Vanrise.Entities.DataRetrievalInput<BPInstanceQuery> input);

        List<BPInstance> GetOpenedInstances();

        List<BPInstance> GetInstancesByCriteria(int definitionID, DateTime dateFrom, DateTime dateTo);
        
        BPInstance GetInstance(long instanceId);

        int InsertEvent(long processInstanceId, string bookmarkName, object eventData);
        long InsertInstance(string processTitle, long? parentId, int definitionID, object inputArguments, BPInstanceStatus executionStatus);
        void LoadPendingEvents(long lastRetrievedId, Action<BPEvent> onEventLoaded);
        void LoadPendingProcesses(List<long> excludedProcessInstanceIds, IEnumerable<BPInstanceStatus> acceptableBPStatuses, Action<BPInstance> onInstanceLoaded);
        int UpdateInstanceStatus(long processInstanceId, BPInstanceStatus status, string message, int retryCount);
        
        int UpdateWorkflowInstanceID(long processInstanceId, Guid workflowInstanceId);

        bool TryLockProcessInstance(long processInstanceId, Guid workflowInstanceId, int currentRuntimeProcessId, IEnumerable<int> runningRuntimeProcessesIds, IEnumerable<BPInstanceStatus> acceptableBPStatuses);

        void UnlockProcessInstance(long processInstanceId, int currentRuntimeProcessId);

        void UpdateProcessInstancesStatus(BPInstanceStatus fromStatus, BPInstanceStatus toStatus, IEnumerable<int> runningRuntimeProcessesIds);
    }
}
