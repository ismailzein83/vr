using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
namespace Vanrise.BusinessProcess.Data
{
    public interface IBPDataManager : IDataManager
    {
        Dictionary<long, BPInstanceStatus> GetProcessesStatuses(List<long> Ids);

        BPDefinition GetDefinition(int ID);

        Vanrise.Entities.BigResult<BPDefinition> GetFilteredDefinitions(Vanrise.Entities.DataRetrievalInput<BPDefinitionQuery> input);

        System.Collections.Generic.List<BPDefinition> GetDefinitions();

        T GetDefinitionObjectState<T>(int definitionId, string objectKey);

        int InsertDefinitionObjectState(int definitionId, string objectKey, object objectValue);

        int UpdateDefinitionObjectState(int definitionId, string objectKey, object objectValue);

        Vanrise.Entities.BigResult<BPInstance> GetInstancesByCriteria(Vanrise.Entities.DataRetrievalInput<BPInstanceQuery> input);

        List<BPInstance> GetRecentInstances(DateTime? StatusUpdatedAfter);

        List<BPInstance> GetInstancesByCriteria(int definitionID, DateTime dateFrom, DateTime dateTo);
        
        BPInstance GetInstance(long instanceId);

        int InsertEvent(long processInstanceId, string bookmarkName, object eventData);
        long InsertInstance(string processTitle, long? parentId, int definitionID, object inputArguments, BPInstanceStatus executionStatus);        
    }
}
