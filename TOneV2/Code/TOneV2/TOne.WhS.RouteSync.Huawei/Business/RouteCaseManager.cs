using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Huawei.Data;
using TOne.WhS.RouteSync.Huawei.Entities;
using Vanrise.Caching;
using Vanrise.Runtime;

namespace TOne.WhS.RouteSync.Huawei.Business
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

        public Dictionary<string, RouteCase> GetCachedRouteCasesByRSName()
        {
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<RouteCaseCacheManager>();
            var cacheName = new GetCachedRouteCasesCacheName() { SwitchId = _switchId };

            return cacheManager.GetOrCreateObject(cacheName, RouteCaseCacheExpirationChecker.Instance, () =>
            {
                Dictionary<string, RouteCase> results = new Dictionary<string, RouteCase>();

                List<RouteCase> routeCases = this.GetRouteCases();
                if (routeCases != null)
                {
                    foreach (RouteCase routeCase in routeCases)
                        results.Add(routeCase.RSName, routeCase);
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

        public Dictionary<string, RouteCase> InsertAndGetRouteCases(List<RouteCase> routeCasesToAdd)
        {
            int maxLockRetryCount = Int32.MaxValue;
            TimeSpan lockRetryInterval = new TimeSpan(0, 0, 1);
            string transactionLockName = String.Concat("WhS_Huawei_{0}.RouteCase", _switchId);

            Dictionary<string, RouteCase> routeCasesByRSName = this.GetCachedRouteCasesByRSName();

            int maxRCNumber = 0;
            if (routeCasesByRSName != null && routeCasesByRSName.Count > 0)
                maxRCNumber = routeCasesByRSName.Select(itm => itm.Value.RCNumber).Max();

            int retryCount = 0;

            while (retryCount < maxLockRetryCount)
            {
                if (TransactionLocker.Instance.TryLock(transactionLockName, () =>
                {
                    Dictionary<string, RouteCase> newRouteCasesByRSSN = _dataManager.GetRouteCasesAfterRCNumber(maxRCNumber);

                    int rcNumber = maxRCNumber;
                    if (newRouteCasesByRSSN != null && newRouteCasesByRSSN.Count > 0)
                    {
                        foreach (var newRouteCaseKvp in newRouteCasesByRSSN)
                        {
                            rcNumber = Math.Max(rcNumber, newRouteCaseKvp.Value.RCNumber);
                            routeCasesByRSName.Add(newRouteCaseKvp.Key, newRouteCaseKvp.Value);
                        }
                    }
                    rcNumber++;

                    Object dbApplyStream = _dataManager.InitialiazeStreamForDBApply();
                    foreach (var routeCaseToAdd in routeCasesToAdd)
                    {
                        if (routeCasesByRSName == null || !routeCasesByRSName.ContainsKey(routeCaseToAdd.RSName))
                        {
                            routeCasesByRSName.Add(routeCaseToAdd.RSName, routeCaseToAdd);
                            _dataManager.WriteRecordToStream(routeCaseToAdd, dbApplyStream);
                            rcNumber++;
                        }
                    }
                    object obj = _dataManager.FinishDBApplyStream(dbApplyStream);
                    _dataManager.ApplyRouteCaseForDB(obj);
                }))
                {
                    return routeCasesByRSName;
                }
                else
                {
                    Thread.Sleep(lockRetryInterval);
                    retryCount++;
                }
            }

            throw new Exception(String.Format("Cannot Lock WhS_Ericsson_{0}.RouteCase", _switchId));
        }

        public void UpdateSyncedRouteCases(IEnumerable<int> rcNumbers)
        {
            _dataManager.UpdateSyncedRouteCases(rcNumbers);
        }

        private List<RouteCase> GetRouteCases()
        {
            return _dataManager.GetAllRouteCases();
        }

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
    }
}