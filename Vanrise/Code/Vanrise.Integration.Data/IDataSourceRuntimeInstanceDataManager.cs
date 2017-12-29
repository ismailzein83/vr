using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Data
{
    public interface IDataSourceRuntimeInstanceDataManager : IDataManager
    {
        void AddNewInstance(Guid runtimeInstanceId, Guid dataSourceId);

        void TryAddNewInstance(Guid runtimeInstanceId, Guid dataSourceId, int maxNumberOfParallelInstances);

        List<DataSourceRuntimeInstance> GetAll();

        bool IsStillExist(Guid dsRuntimeInstanceId);

        void DeleteInstance(Guid dsRuntimeInstanceId);

        bool DoesAnyDSRuntimeInstanceExist(Guid dataSourceId);
    }
}
