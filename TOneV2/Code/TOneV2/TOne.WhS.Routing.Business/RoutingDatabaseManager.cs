using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.Caching;
using Vanrise.Common;

namespace TOne.WhS.Routing.Business
{
    public class RoutingDatabaseManager
    {
        public IEnumerable<RoutingDatabaseInfo> GetRoutingDatabaseInfo(RoutingDatabaseInfoFilter filter)
        {
            //return GetNotDeletedDatabases().MapRecords(RoutingDatabaseInfoMapper, x => x.ProcessType == filter.ProcessType).OrderBy(x => x.Title);

            List<RoutingDatabaseInfo> routingDatabases = new List<RoutingDatabaseInfo>();
            foreach (RoutingDatabaseType routingDatabaseType in Enum.GetValues(typeof(RoutingDatabaseType)))
            {
                IEnumerable<RoutingDatabase> databases = GetRoutingDatabasesReady(filter.ProcessType, routingDatabaseType);
                if (databases != null && databases.Count() > 0)
                {
                    var item = RoutingDatabaseInfoMapper(databases.OrderByDescending(itm => itm.CreatedTime).First());
                    item.Title = routingDatabaseType.ToString();
                    routingDatabases.Add(item);
                }
            }
            return routingDatabases;
        }

        public IEnumerable<RoutingDatabase> GetRoutingDatabasesReady(RoutingProcessType routingProcessType, RoutingDatabaseType routingDatabaseType)
        {
            Func<RoutingDatabase, bool> filterExpression = (itm) => (itm.ProcessType == routingProcessType && itm.Type == routingDatabaseType && itm.IsReady);
            return GetNotDeletedDatabases().FindAllRecords(filterExpression);
        }
        public IEnumerable<RoutingDatabase> GetRoutingDatabases(RoutingProcessType routingProcessType, RoutingDatabaseType routingDatabaseType)
        {
            Func<RoutingDatabase, bool> filterExpression = (itm) => (itm.ProcessType == routingProcessType && itm.Type == routingDatabaseType);
            return GetNotDeletedDatabases().FindAllRecords(filterExpression);
        }

        public RoutingDatabase GetRoutingDatabase(int routingDatabaseId)
        {
            var items = GetNotDeletedDatabases();
            return items.GetRecord(routingDatabaseId);
        }

        public RoutingDatabase GetRoutingDatabaseFromDB(int routingDatabaseId)
        {
            IRoutingDatabaseDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRoutingDatabaseDataManager>();
            return dataManager.GetRoutingDatabase(routingDatabaseId);
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

        public void DropDatabase(RoutingDatabase routingDatabase)
        {
            IRoutingDatabaseDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRoutingDatabaseDataManager>();
            dataManager.DropDatabase(routingDatabase);
            CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
        }

        public void SetReady(int databaseId)
        {
            IRoutingDatabaseDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRoutingDatabaseDataManager>();
            dataManager.SetReady(databaseId);
            CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
        }

        public int CreateDatabase(string name, RoutingDatabaseType type, RoutingProcessType processType, DateTime? effectiveTime, RoutingDatabaseInformation information, RoutingDatabaseSettings settings)
        {
            IRoutingDatabaseDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRoutingDatabaseDataManager>();
            int databaseId =  dataManager.CreateDatabase(name, type, processType, effectiveTime, information, settings);
            CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            return databaseId;
        }
        #region Private Methods

        private RoutingDatabaseInfo RoutingDatabaseInfoMapper(RoutingDatabase routingDatabase)
        {
            return new RoutingDatabaseInfo()
            {
                RoutingDatabaseId = routingDatabase.ID,
                Title = routingDatabase.Title,
                Information = routingDatabase.Information
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
