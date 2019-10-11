using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Huawei.SoftX3000.Data;
using TOne.WhS.RouteSync.Huawei.SoftX3000.Entities;
using Vanrise.Caching;
using Vanrise.Runtime;

namespace TOne.WhS.RouteSync.Huawei.SoftX3000.Business
{
    public class RouteOptionsManager
    {
        #region Properties/Ctor

        string _switchId;
        IRouteOptionsDataManager _dataManager;

        public RouteOptionsManager(string switchId)
        {
            _switchId = switchId;

            _dataManager = RouteSyncHuaweiDataManagerFactory.GetDataManager<IRouteOptionsDataManager>();
            _dataManager.SwitchId = switchId;
        }

        #endregion

        #region First Step
        public void Initialize()
        {
            _dataManager.Initialize(new RouteOptionsInitializeContext());
        }

        #endregion

        #region Second Step

        #region Public Methods

        public Dictionary<string, RouteOptions> GetCachedRoutesOptionsByName()
        {
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<RouteOptionsCacheManager>();
            var cacheName = new GetCachedRoutesOptionsCacheName() { SwitchId = _switchId };

            return cacheManager.GetOrCreateObject(cacheName, RouteOptionsCacheExpirationChecker.Instance, () =>
            {
                Dictionary<string, RouteOptions> results = new Dictionary<string, RouteOptions>();

                List<RouteOptions> routesOptions = this.GetRoutesOptions();
                if (routesOptions != null)
                {
                    foreach (RouteOptions routeOptions in routesOptions)
                        results.Add(routeOptions.RouteOptionsAsString, routeOptions);
                }

                return results;
            });
        }

        public Dictionary<string, RouteOptions> InsertAndGetRoutesOptions(List<RouteOptions> routesOptionsToAdd, SwitchSettings settings)
        {
            int maxLockRetryCount = Int32.MaxValue;
            TimeSpan lockRetryInterval = new TimeSpan(0, 0, 1);
            string transactionLockName = $"WhS_RouteSync_HuaweiSoftX3000_{_switchId}.RouteOptions";

            Dictionary<string, RouteOptions> routesOptionsByName = this.GetCachedRoutesOptionsByName();
            long maxRouteNumber = settings.StartRouteId;
            if (routesOptionsByName != null && routesOptionsByName.Count > 0)
                maxRouteNumber = routesOptionsByName.Select(itm => itm.Value.RouteId).Max();

            int retryCount = 0;
            while (retryCount < maxLockRetryCount)
            {
                if (TransactionLocker.Instance.TryLock(transactionLockName, () =>
                {
                    Dictionary<string, RouteOptions> newRoutesOptionsByName = _dataManager.GetRouteOptionsAfterRouteNumber(maxRouteNumber);

                    long routeNumber = maxRouteNumber;
                    if (newRoutesOptionsByName != null && newRoutesOptionsByName.Count > 0)
                    {
                        foreach (var newRouteOptionsKvp in newRoutesOptionsByName)
                        {
                            routeNumber = Math.Max(routeNumber, newRouteOptionsKvp.Value.RouteId);
                            routesOptionsByName.Add(newRouteOptionsKvp.Key, newRouteOptionsKvp.Value);
                        }
                    }

                    routeNumber++;

                    Object dbApplyStream = _dataManager.InitialiazeStreamForDBApply();
                    Dictionary<string, RouteOptions> addedRoutesOptionsByName = new Dictionary<string, RouteOptions>();
                    foreach (var routeOptionsToAdd in routesOptionsToAdd)
                    {
                        string routeOptionsAsString = routeOptionsToAdd.RouteOptionsAsString;

                        if (addedRoutesOptionsByName.ContainsKey(routeOptionsAsString))
                            continue;

                        if (routesOptionsByName == null || !routesOptionsByName.ContainsKey(routeOptionsAsString))
                        {
                            RouteOptions routeOptions = new RouteOptions() { RouteId = routeNumber, RouteOptionsAsString = routeOptionsAsString };
                            addedRoutesOptionsByName.Add(routeOptionsAsString, routeOptions);
                            _dataManager.WriteRecordToStream(routeOptions, dbApplyStream);
                            routeNumber++;
                        }
                    }
                    object obj = _dataManager.FinishDBApplyStream(dbApplyStream);
                    _dataManager.ApplyRouteOptionsForDB(obj);

                    if (addedRoutesOptionsByName.Count > 0)
                    {
                        foreach (var addedRouteOptionsByNameKvp in addedRoutesOptionsByName)
                        {
                            routesOptionsByName.Add(addedRouteOptionsByNameKvp.Key, addedRouteOptionsByNameKvp.Value);
                        }
                    }
                }))
                {
                    return routesOptionsByName;
                }
                else
                {
                    Thread.Sleep(lockRetryInterval);
                    retryCount++;
                }
            }

            throw new Exception($"Cannot Lock WhS_RouteSync_HuaweiSoftX3000_{_switchId}.RouteOptions");
        }

        #endregion

        #region Private Methods

        private List<RouteOptions> GetRoutesOptions()
        {
            return _dataManager.GetAllRoutesOptions();
        }

        #endregion

        #endregion

        #region Third Step

        public List<RouteOptions> GetNotSyncedRoutesOptions()
        {
            return _dataManager.GetNotSyncedRoutesOptions();
        }

        public void UpdateSyncedRoutesOptions(IEnumerable<long> routeIds)
        {
            _dataManager.UpdateSyncedRoutesOptions(routeIds);
        }
        #endregion

        #region Private Classes

        private class RouteOptionsCacheManager : BaseCacheManager
        {
        }

        private class RouteOptionsCacheExpirationChecker : CacheExpirationChecker
        {
            static RouteOptionsCacheExpirationChecker s_instance = new RouteOptionsCacheExpirationChecker();
            public static RouteOptionsCacheExpirationChecker Instance { get { return s_instance; } }

            public override bool IsCacheExpired(Vanrise.Caching.ICacheExpirationCheckerContext context)
            {
                TimeSpan entitiesTimeSpan = TimeSpan.FromMinutes(15);
                SlidingWindowCacheExpirationChecker slidingWindowCacheExpirationChecker = new SlidingWindowCacheExpirationChecker(entitiesTimeSpan);
                return slidingWindowCacheExpirationChecker.IsCacheExpired(context);
            }
        }

        private struct GetCachedRoutesOptionsCacheName
        {
            public string SwitchId { get; set; }
        }

        #endregion
    }
}