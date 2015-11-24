using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;

namespace TOne.WhS.Routing.Business
{
    public class RoutingDatabaseManager
    {

        public IEnumerable<RoutingDatabaseInfo> GetRoutingDatabaseInfo()
        {
            IEnumerable<RoutingDatabase> routingDatabases = GetNotDeletedDatabases().Values;
            return routingDatabases.MapRecords(RoutingDatabaseInfoMapper);
        }

        public Dictionary<int, RoutingDatabase> GetNotDeletedDatabases()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetRoutingDatabases",
               () =>
               {
                   IRoutingDatabaseDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRoutingDatabaseDataManager>();
                   IEnumerable<RoutingDatabase> routingDatabases = dataManager.GetNotDeletedDatabases();
                   return routingDatabases.ToDictionary(kvp => kvp.ID, kvp => kvp);
               });
        }

        #region Private Methods

        private RoutingDatabaseInfo RoutingDatabaseInfoMapper(RoutingDatabase routingDatabase)
        {
            return new RoutingDatabaseInfo()
            {
                RoutingDatabaseId = routingDatabase.ID,
                Title = routingDatabase.Title
            };
        }

        #endregion


        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IRoutingDatabaseDataManager _dataManager = RoutingDataManagerFactory.GetDataManager<IRoutingDatabaseDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreRoutingDatabasesUpdated(ref _updateHandle);
            }
        }

        #endregion
    }
}
