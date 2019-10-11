using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TOne.WhS.RouteSync.Huawei.SoftX3000.Data;
using TOne.WhS.RouteSync.Huawei.SoftX3000.Entities;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Runtime;

namespace TOne.WhS.RouteSync.Huawei.SoftX3000.Business
{
    public class RouteCaseManager
    {
        string _switchId;
        IRouteCaseDataManager _dataManager;

        public RouteCaseManager(string switchId)
        {
            _switchId = switchId;

            _dataManager = RouteSyncHuaweiDataManagerFactory.GetDataManager<IRouteCaseDataManager>();
            _dataManager.SwitchId = switchId;
        }

        public Dictionary<string, Dictionary<int, RouteCase>> GetCachedRouteCasesByRSSCByRAN()
        {
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<RouteCaseCacheManager>();
            var cacheName = new GetCachedRouteCasesCacheName() { SwitchId = _switchId };

            return cacheManager.GetOrCreateObject(cacheName, RouteCaseCacheExpirationChecker.Instance, () =>
            {
                Dictionary<string, Dictionary<int, RouteCase>> results = new Dictionary<string, Dictionary<int, RouteCase>>();

                List<RouteCase> routeCases = this.GetRouteCases();
                if (routeCases != null)
                {
                    foreach (RouteCase routeCase in routeCases)
                    {
                        Dictionary<int, RouteCase> routeCasesByRSSC = results.GetOrCreateItem(routeCase.RAN);
                        routeCasesByRSSC.Add(routeCase.RSSC, routeCase);
                    }
                }

                return results;
            });
        }

        public List<RouteCase> GetNotSyncedRouteCases()
        {
            return _dataManager.GetNotSyncedRouteCases();
        }

        public void Initialize()
        {
            _dataManager.Initialize(new RouteCaseInitializeContext());
        }

        public Dictionary<string, Dictionary<int, RouteCase>> InsertAndGetRouteCases(List<RouteCaseToAdd> routeCasesToAdd, SwitchSettings settings)
        {
            int maxLockRetryCount = Int32.MaxValue;
            TimeSpan lockRetryInterval = new TimeSpan(0, 0, 1);
            string transactionLockName = $"WhS_RouteSync_HuaweiSoftX3000_{_switchId}.RouteCase";

            Dictionary<string, Dictionary<int, RouteCase>> routeCasesByRSSCByName = this.GetCachedRouteCasesByRSSCByRAN();

            long maxRouteCaseId = settings.StartRouteCaseId;
            if (routeCasesByRSSCByName != null && routeCasesByRSSCByName.Count > 0)
                maxRouteCaseId = routeCasesByRSSCByName.Select(itm => itm.Value.Values.Select(route => route.RouteCaseId).Max()).Max();

            int retryCount = 0;

            while (retryCount < maxLockRetryCount)
            {
                if (TransactionLocker.Instance.TryLock(transactionLockName, () =>
                {
                    List<RouteCase> newRouteCases = _dataManager.GetRouteCasesAfterRCNumber(maxRouteCaseId);

                    long routeCaseId = maxRouteCaseId;
                    if (newRouteCases != null && newRouteCases.Count > 0)
                    {
                        foreach (var newRouteCase in newRouteCases)
                        {
                            routeCaseId = Math.Max(routeCaseId, newRouteCase.RouteCaseId);
                            Dictionary<int, RouteCase> routeCasesByRSSC = routeCasesByRSSCByName.GetOrCreateItem(newRouteCase.RAN);
                            routeCasesByRSSC.Add(newRouteCase.RSSC, newRouteCase);
                        }
                    }

                    routeCaseId++;

                    RouteOptionsManager routeOptionsManager = new RouteOptionsManager(_switchId);
                    var routesOptionsByName = routeOptionsManager.GetCachedRoutesOptionsByName();

                    Object dbApplyStream = _dataManager.InitialiazeStreamForDBApply();
                    Dictionary<string, Dictionary<int, RouteCase>> addedRouteCasesByRSSCByName = new Dictionary<string, Dictionary<int, RouteCase>>();
                    foreach (var routeCaseToAdd in routeCasesToAdd)
                    {
                        var routeAnalysisName = routeCaseToAdd.RAN;
                        if (Helper.RAN_RSSCExists(addedRouteCasesByRSSCByName, routeAnalysisName, routeCaseToAdd.RSSC))
                            continue;

                        if (!routeCasesByRSSCByName.ContainsKey(routeAnalysisName))
                        {
                            RouteOptions routeOptions;
                            if (!routesOptionsByName.TryGetValue(routeAnalysisName, out routeOptions))
                                throw new Exception($"Route Options '{routeAnalysisName}' does not exist!");

                            RouteCase routeCase = new RouteCase() { RouteCaseId = routeCaseId, RAN = routeAnalysisName, RouteId = routeOptions.RouteId, RSSC = routeCaseToAdd.RSSC };
                            Dictionary<int, RouteCase> addedRouteCasesByRSSC = addedRouteCasesByRSSCByName.GetOrCreateItem(routeAnalysisName);
                            addedRouteCasesByRSSC.Add(routeCaseToAdd.RSSC, routeCase);
                            _dataManager.WriteRecordToStream(routeCase, dbApplyStream);
                            routeCaseId++;
                        }
                    }
                    object obj = _dataManager.FinishDBApplyStream(dbApplyStream);
                    _dataManager.ApplyRouteCaseForDB(obj);

                    if (addedRouteCasesByRSSCByName.Count > 0)
                    {
                        foreach(var addedRouteCaseByRSSCByNameKvp in addedRouteCasesByRSSCByName)
                        {
                            routeCasesByRSSCByName.Add(addedRouteCaseByRSSCByNameKvp.Key, addedRouteCaseByRSSCByNameKvp.Value);
                        }
                    }
                }))
                {
                    return routeCasesByRSSCByName;
                }
                else
                {
                    Thread.Sleep(lockRetryInterval);
                    retryCount++;
                }
            }

            throw new Exception($"Cannot Lock WhS_RouteSync_HuaweiSoftX3000_{_switchId}.RouteCase");
        }

        public void UpdateSyncedRouteCases(IEnumerable<long> RouteCaseIdNumbers)
        {
            _dataManager.UpdateSyncedRouteCases(RouteCaseIdNumbers);
        }

        private List<RouteCase> GetRouteCases()
        {
            return _dataManager.GetAllRouteCases();
        }

        #region Private Classes

        private class RouteCaseCacheManager : BaseCacheManager
        {
        }

        private class RouteCaseCacheExpirationChecker : CacheExpirationChecker
        {
            static RouteCaseCacheExpirationChecker s_instance = new RouteCaseCacheExpirationChecker();
            public static RouteCaseCacheExpirationChecker Instance { get { return s_instance; } }

            public override bool IsCacheExpired(Vanrise.Caching.ICacheExpirationCheckerContext context)
            {
                TimeSpan entitiesTimeSpan = TimeSpan.FromMinutes(15);
                SlidingWindowCacheExpirationChecker slidingWindowCacheExpirationChecker = new SlidingWindowCacheExpirationChecker(entitiesTimeSpan);
                return slidingWindowCacheExpirationChecker.IsCacheExpired(context);
            }
        }

        private struct GetCachedRouteCasesCacheName
        {
            public string SwitchId { get; set; }
        }

        #endregion
    }
}