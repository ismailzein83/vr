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

        internal DataSourceRuntimeInstance TryGetOneAndLock()
        {
            return _dataManager.TryGetOneAndLock(RunningProcessManager.CurrentProcess.ProcessId);
        }

        internal void SetInstanceCompleted(Guid dsRuntimeInstanceId)
        {
            _dataManager.SetInstanceCompleted(dsRuntimeInstanceId);
        }

        internal bool AreDSInstancesCompleted(Guid dataSourceId)
        {
            RunningProcessManager runningProcessManager = new RunningProcessManager();
            IEnumerable<int> runningRuntimeProcessesIds = runningProcessManager.GetCachedRunningProcesses().Select(itm => itm.ProcessId);
            if (!_dataManager.IsAnyInstanceRunning(dataSourceId, runningRuntimeProcessesIds))
            {
                _dataManager.DeleteDSInstances(dataSourceId);
                return true;
            }
            else
                return false;
        }
    }
}
