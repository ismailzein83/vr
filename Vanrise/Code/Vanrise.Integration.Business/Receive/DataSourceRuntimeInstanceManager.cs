using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Data;
using Vanrise.Integration.Entities;
using Vanrise.Runtime;

namespace Vanrise.Integration.Business
{
    public class DataSourceRuntimeInstanceManager
    {
        IDataSourceRuntimeInstanceDataManager _dataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceRuntimeInstanceDataManager>();

        internal void AddNewInstance(Guid dataSourceId)
        {
            _dataManager.AddNewInstance(Guid.NewGuid(), dataSourceId);
        }

        internal void TryAddNewInstance(Guid dataSourceId, int maxNumberOfParallelInstances)
        {
            _dataManager.TryAddNewInstance(Guid.NewGuid(), dataSourceId, maxNumberOfParallelInstances);
        }

        internal bool DoesAnyDSRuntimeInstanceExist(Guid dataSourceId)
        {
            return _dataManager.DoesAnyDSRuntimeInstanceExist(dataSourceId);
        }
    }
}
