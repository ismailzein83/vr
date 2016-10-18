using System;
namespace Vanrise.BusinessProcess.Data
{
    public interface IBPDataManager : IDataManager
    {
        T GetDefinitionObjectState<T>(Guid definitionId, string objectKey);

        int InsertDefinitionObjectState(Guid definitionId, string objectKey, object objectValue);

        int UpdateDefinitionObjectState(Guid definitionId, string objectKey, object objectValue);     
    }
}
