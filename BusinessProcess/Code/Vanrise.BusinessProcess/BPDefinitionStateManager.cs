using System;
using Vanrise.BusinessProcess.Data;

namespace Vanrise.BusinessProcess
{
    public class BPDefinitionStateManager
    {
        #region BP Transaction Methods

        public T GetDefinitionObjectState<T>(Guid definitionId, string objectKey)
        {
            IBPDefinitionStateDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDefinitionStateDataManager>();
            return dataManager.GetDefinitionObjectState<T>(definitionId, objectKey);
        }

        public void SaveDefinitionObjectState(Guid definitionId, string objectKey, object objectValue)
        {
            IBPDefinitionStateDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDefinitionStateDataManager>();
            if (dataManager.UpdateDefinitionObjectState(definitionId, objectKey, objectValue) <= 0)
                dataManager.InsertDefinitionObjectState(definitionId, objectKey, objectValue);
        }

        #endregion
    }
}