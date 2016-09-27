using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess
{
    public class ProcessManager
    {
        #region BP Transaction Methods

        public T GetDefinitionObjectState<T>(int definitionId, string objectKey)
        {
            IBPDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDataManager>();
            return dataManager.GetDefinitionObjectState<T>(definitionId, objectKey);
        }

        public void SaveDefinitionObjectState(int definitionId, string objectKey, object objectValue)
        {
            IBPDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDataManager>();
            if (dataManager.UpdateDefinitionObjectState(definitionId, objectKey, objectValue) <= 0)
                dataManager.InsertDefinitionObjectState(definitionId, objectKey, objectValue);
        }

        #endregion

        #region Private Methods

        #endregion

    }
}
