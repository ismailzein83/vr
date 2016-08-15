using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Data;
using Vanrise.BEBridge.Entities;
using Vanrise.Common;

namespace Vanrise.BEBridge.Business
{
    public class BEReceiveDefinitionManager
    {
        #region Public Methods
        public BEReceiveDefinition GetBEReceiveDefinition(Guid id)
        {
            var allBEReceiveDefinitions = GetCachedBEReceiveDefinitions();
            return allBEReceiveDefinitions.GetRecord(id);

        }

        #endregion

        #region Private Methods
        Dictionary<Guid, BEReceiveDefinition> GetCachedBEReceiveDefinitions()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetBEReceiveDefinitions",
               () =>
               {
                   IBEReceiveDefinitionDataManager dataManager = BEBridgeDataManagerFactory.GetDataManager<IBEReceiveDefinitionDataManager>();
                   IEnumerable<BEReceiveDefinition> carrierAccounts = dataManager.GetBEReceiveDefinitions();
                   return carrierAccounts.ToDictionary(kvp => kvp.BEReceiveDefinitionId, kvp => kvp);
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IBEReceiveDefinitionDataManager _dataManager = BEBridgeDataManagerFactory.GetDataManager<IBEReceiveDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreBEReceiveDefinitionsUpdated(ref _updateHandle);
            }
        }

        #endregion
    }
}
