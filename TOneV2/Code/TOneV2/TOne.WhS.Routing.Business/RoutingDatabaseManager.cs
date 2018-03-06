using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Business
{
    public class RoutingDatabaseManager : IRoutingDatabaseManager
    {
        public IEnumerable<RoutingDatabaseInfo> GetRoutingDatabaseInfo(RoutingDatabaseInfoFilter filter)
        {
            //return GetNotDeletedDatabases().MapRecords(RoutingDatabaseInfoMapper, x => x.ProcessType == filter.ProcessType).OrderBy(x => x.Title);

            List<RoutingDatabaseInfo> routingDatabases = new List<RoutingDatabaseInfo>();
            foreach (RoutingDatabaseType routingDatabaseType in Enum.GetValues(typeof(RoutingDatabaseType)))
            {
                RoutingDatabase database = GetLatestRoutingDatabase(filter.ProcessType, routingDatabaseType);
                if (database != null)
                {
                    var item = RoutingDatabaseInfoMapper(database);
                    item.Title = routingDatabaseType.ToString();
                    item.Type = database.Type;
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
            var notDeletedDBs = GetNotDeletedDatabases();
            RoutingDatabase db = notDeletedDBs.GetRecord(routingDatabaseId);
            if (db == null && notDeletedDBs != null && !notDeletedDBs.Values.Any(itm => itm.ID > routingDatabaseId))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                db = GetNotDeletedDatabases().GetRecord(routingDatabaseId);
            }
            return db;
        }

        public RoutingDatabase GetRoutingDatabaseFromDB(int routingDatabaseId)
        {
            IRoutingDatabaseDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRoutingDatabaseDataManager>();
            return dataManager.GetRoutingDatabase(routingDatabaseId);
        }

        public DateTime? GetLatestRoutingDatabaseEffectiveTime(RoutingProcessType routingProcessType, RoutingDatabaseType routingDatabaseType)
        {
            var latestRoutingDatabase = GetLatestRoutingDatabase(routingProcessType, routingDatabaseType);

            if (latestRoutingDatabase == null)
                return null;

            DateTime? effectiveTime = latestRoutingDatabase.EffectiveTime;

            if (routingDatabaseType == RoutingDatabaseType.Current && !effectiveTime.HasValue)
                throw new DataIntegrityValidationException(string.Format("Routing Database with Id {0} of type Current has no effective Time", latestRoutingDatabase.ID));

            return effectiveTime;
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
            int databaseId = dataManager.CreateDatabase(name, type, processType, effectiveTime, information, settings);
            CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            return databaseId;
        }

        public RoutingDatabase GetLatestRoutingDatabase(RoutingProcessType routingProcessType, RoutingDatabaseType routingDatabaseType)
        {
            IEnumerable<RoutingDatabase> routingDatabases = GetRoutingDatabasesReady(routingProcessType, routingDatabaseType);
            if (routingDatabases == null || routingDatabases.Count() == 0)
                return null;
            return routingDatabases.OrderByDescending(itm => itm.CreatedTime).First();
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
