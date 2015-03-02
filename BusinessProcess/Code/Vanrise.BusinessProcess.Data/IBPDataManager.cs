using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
namespace Vanrise.BusinessProcess.Data
{
    public interface IBPDataManager : IDataManager
    {
        int ClearLoadedFlag();
        int DeleteEvent(long eventId);
        System.Collections.Generic.List<BPDefinition> GetDefinitions();

        T GetDefinitionObjectState<T>(int definitionId, string objectKey);

        int InsertDefinitionObjectState(int definitionId, string objectKey, object objectValue);

        int UpdateDefinitionObjectState(int definitionId, string objectKey, object objectValue);

        List<BPInstance> GetInstancesByCriteria(int definitionID, DateTime datefrom, DateTime dateto);
        
        BPInstance GetInstance(long instanceId);

        int InsertEvent(long processInstanceId, string bookmarkName, object eventData);
        long InsertInstance(string processTitle, long? parentId, int definitionID, object inputArguments, BPInstanceStatus executionStatus);
        void LoadPendingEvents(Action<BPEvent> onEventLoaded);
        void LoadPendingProcesses(Action<BPInstance> onInstanceLoaded);
        int UpdateInstanceStatus(long processInstanceId, BPInstanceStatus status, string message, int retryCount);
        int UpdateLoadedFlag(long processInstanceId, bool loaded);
        int UpdateWorkflowInstanceID(long processInstanceId, Guid workflowInstanceId);
    }
}
