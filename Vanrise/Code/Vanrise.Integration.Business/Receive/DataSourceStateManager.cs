using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vanrise.Common;
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
            LockDataSourceState(dataSourceId,
                () =>
                {
                    string dsStateAsString = _dataManager.GetDataSourceState(dataSourceId);
                    BaseAdapterState dsState = dsStateAsString != null ? Serializer.Deserialize(dsStateAsString) as BaseAdapterState : null;
                    BaseAdapterState updatedDSState = onStateReady(dsState);
                    string updatedDSStateAsString = updatedDSState != null ? Serializer.Serialize(updatedDSState) : null;
                    if (updatedDSStateAsString != dsStateAsString)
                        _dataManager.InsertOrUpdateDataSourceState(dataSourceId, updatedDSStateAsString);
                });
        }

        private void LockDataSourceState(Guid dataSourceId, Action afterLockAction)
        {
            string dsStateTransactionLockName = String.Concat("DataSourceState_", dataSourceId);
            int retryCount = 0;
            while (retryCount < s_maxLockRetryCount)
            {
                if (TransactionLocker.Instance.TryLock(dsStateTransactionLockName, afterLockAction))
                    return;
                Thread.Sleep(s_lockRetryInterval);
                retryCount++;
            }
            throw new Exception(String.Format("Cannot Lock Data Source State. data source Id '{0}'", dataSourceId));
            
        }
    }
}
