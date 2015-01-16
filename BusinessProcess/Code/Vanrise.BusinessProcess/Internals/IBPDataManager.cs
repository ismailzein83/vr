using System;
namespace Vanrise.BusinessProcess
{
    interface IBPDataManager
    {
        int ClearLoadedFlag();
        int DeleteEvent(long eventId);
        System.Collections.Generic.List<BPDefinition> GetDefinitions();
        int InsertEvent(Guid processInstanceId, string bookmarkName, object eventData);
        int InsertInstance(Guid processInstanceId, string processTitle, Guid? parentId, int definitionID, object inputArguments, BPInstanceStatus executionStatus);
        void LoadPendingEvents(Action<BPEvent> onEventLoaded);
        void LoadPendingProcesses(Action<BPInstance> onInstanceLoaded);
        int UpdateInstanceStatus(Guid processInstanceId, BPInstanceStatus status, string message, int retryCount);
        int UpdateLoadedFlag(Guid processInstanceId, bool loaded);
        int UpdateWorkflowInstanceID(Guid processInstanceId, Guid workflowInstanceId);
    }
}
