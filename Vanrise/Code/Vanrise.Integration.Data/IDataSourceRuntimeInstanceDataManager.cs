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
        void AddNewInstance(Guid runtimeInstanceId, int dataSourceId);

        void TryAddNewInstance(Guid runtimeInstanceId, int dataSourceId, int maxNumberOfParallelInstances);

        DataSourceRuntimeInstance TryGetOneAndLock(int currentRuntimeProcessId);

        void SetInstanceCompleted(Guid dsRuntimeInstanceId);

        bool IsAnyInstanceRunning(int dataSourceId, IEnumerable<int> runningRuntimeProcessesIds);

        void DeleteDSInstances(int dataSourceId);
    }
}
