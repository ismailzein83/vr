using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;
using Vanrise.Caching;

namespace Vanrise.Queueing
{
    public class QueueActivatorConfigManager
    {

        #region Public Methods
        public IEnumerable<QueueActivatorConfig> GetQueueActivatorsConfig()
        {
            var cachedQueueActivatorConfig = GetCachedQueueActivatorConfig();
            return cachedQueueActivatorConfig.Values;
        }
        #endregion


        #region Private Methods
        private Dictionary<int, QueueActivatorConfig> GetCachedQueueActivatorConfig()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetQueueExecutionFlowDefinitions",
               () =>
               {
                   IQueueActivatorConfigDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueActivatorConfigDataManager>();
                   IEnumerable<QueueActivatorConfig> queueExecutionFlowDefinitions = dataManager.GetAllQueueActivatorConfig();
                   return queueExecutionFlowDefinitions.ToDictionary(kvp => kvp.QueueActivatorConfigId, kvp => kvp);
               });
        }

        #endregion



        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IQueueExecutionFlowDefinitionDataManager _dataManager = QDataManagerFactory.GetDataManager<IQueueExecutionFlowDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreQueueExecutionFlowDefinitionUpdated(ref _updateHandle);
            }
        }


        #endregion
       

    }
}
