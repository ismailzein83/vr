namespace Vanrise.BusinessProcess.Data
{
    public interface IBPDataManager : IDataManager
    {
        T GetDefinitionObjectState<T>(int definitionId, string objectKey);

        int InsertDefinitionObjectState(int definitionId, string objectKey, object objectValue);

        int UpdateDefinitionObjectState(int definitionId, string objectKey, object objectValue);     
    }
}
