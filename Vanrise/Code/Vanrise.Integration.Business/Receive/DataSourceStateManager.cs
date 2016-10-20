using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vanrise.Integration.Data;
using Vanrise.Integration.Entities;
using Vanrise.Runtime;

namespace Vanrise.Integration.Business
{
    public class DataSourceStateManager
    {        
        static int s_maxLockRetryCount;
        static TimeSpan s_lockRetryInterval;

        static DataSourceStateManager()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings["Integration_DataSourceState_MaxLockRetryCount"], out s_maxLockRetryCount))
                s_maxLockRetryCount = 10;
            if (!TimeSpan.TryParse(ConfigurationManager.AppSettings["Integration_DataSourceState_LockRetryInterval"], out s_lockRetryInterval))
                s_lockRetryInterval = new TimeSpan(0, 0, 1);
        }

        IDataSourceStateDataManager _dataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceStateDataManager>();


        public void GetStateWithLock(Guid dataSourceId, Func<BaseAdapterState, BaseAdapterState> onStateReady)
        {
            BaseAdapterState dsState = LockDSState(dataSourceId);
            var updatedState = onStateReady(dsState);
            _dataManager.UpdateStateAndUnlock(dataSourceId, updatedState);
        }

        private BaseAdapterState LockDSState(Guid dataSourceId)
        {
            int currentRuntimeProcessId = RunningProcessManager.CurrentProcess.ProcessId;
            RunningProcessManager runningProcessManager = new RunningProcessManager();
            int retryCount = 0;
            while (retryCount < s_maxLockRetryCount)
            {
                IEnumerable<int> runningRuntimeProcessesIds = runningProcessManager.GetCachedRunningProcesses().Select(itm => itm.ProcessId);
                BaseAdapterState adapterState;
                if (_dataManager.TryLockAndGet(dataSourceId, currentRuntimeProcessId, runningRuntimeProcessesIds, out adapterState))
                    return adapterState;
                Thread.Sleep(s_lockRetryInterval);
                retryCount++;
            }
            throw new Exception(String.Format("Cannot Lock Data Source State. data source Id '{0}'", dataSourceId));
        }
    }
}
