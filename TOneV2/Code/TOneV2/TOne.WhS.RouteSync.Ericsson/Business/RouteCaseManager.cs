using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Ericsson.Data;
using TOne.WhS.RouteSync.Ericsson.Entities;
using Vanrise.Caching;
using Vanrise.Runtime;

namespace TOne.WhS.RouteSync.Ericsson.Business
{
    public class RouteCaseManager
    {
        public Dictionary<string, RouteCase> InsertAndGetRouteCases(string switchId, HashSet<string> routeCaseOptionsToAdd)
        {
            int maxLockRetryCount = Int32.MaxValue;
            TimeSpan lockRetryInterval = new TimeSpan(0, 0, 1);
            IRouteCaseDataManager dataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<IRouteCaseDataManager>();
            dataManager.SwitchId = switchId;

            string transactionLockName = String.Concat("WhS_Ericsson_{0}.RouteCase", switchId);
            int retryCount = 0;
            List<RouteCase> routeCasesToAdd = new List<RouteCase>();

            Dictionary<string, RouteCase> routeCases = GetCachedRouteCasesGroupedByOptions(switchId);
            int lastCurrentRCNumber = 0;
            if (routeCases != null)
                lastCurrentRCNumber = routeCases.Select(itm => itm.Value.RCNumber).Max();

            while (retryCount < maxLockRetryCount)
            {
                if (TransactionLocker.Instance.TryLock(transactionLockName, () =>
                {
                    Dictionary<string, RouteCase> newRouteCasesByOptions = dataManager.GetRouteCasesAfterRCNumber(lastCurrentRCNumber);

                    int rcNumber = lastCurrentRCNumber;
                    if (newRouteCasesByOptions != null && newRouteCasesByOptions.Count > 0)
                    {
                        foreach (var newRouteCaseByOptionsKVP in newRouteCasesByOptions)
                        {
                            rcNumber = Math.Max(rcNumber, newRouteCaseByOptionsKVP.Value.RCNumber);
                            routeCases.Add(newRouteCaseByOptionsKVP.Key, newRouteCaseByOptionsKVP.Value);
                        }
                    }
                    rcNumber++;

                    Object dbApplyStream = dataManager.InitialiazeStreamForDBApply();
                    foreach (string routeCaseOption in routeCaseOptionsToAdd)
                    {
                        if (newRouteCasesByOptions == null || !newRouteCasesByOptions.ContainsKey(routeCaseOption))
                        {
                            RouteCase routeCaseToAdd = new RouteCase() { RCNumber = rcNumber, RouteCaseOptionsAsString = routeCaseOption };
                            routeCases.Add(routeCaseOption, routeCaseToAdd);
                            dataManager.WriteRecordToStream(routeCaseToAdd, dbApplyStream);
                            rcNumber++;
                        }
                    }
                    object obj = dataManager.FinishDBApplyStream(dbApplyStream);
                    dataManager.ApplyRouteCaseForDB(obj);
                }))
                {
                    return routeCases;
                }
                else
                {
                    Thread.Sleep(lockRetryInterval);
                    retryCount++;
                }
            }
            throw new Exception(String.Format("Cannot Lock WhS_Ericsson_{0}.RouteCase", switchId));
        }

        private struct GetCachedRouteCasesGroupedByOptionsCacheName
        {
            public string SwitchId { get; set; }
        }

        public Dictionary<string, RouteCase> GetCachedRouteCasesGroupedByOptions(string switchId)
        {
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<RouteCaseCacheManager>();
            var cacheName = new GetCachedRouteCasesGroupedByOptionsCacheName() { SwitchId = switchId };

            return cacheManager.GetOrCreateObject(cacheName, RouteCaseCacheExpirationChecker.Instance, () =>
            {
                Dictionary<string, RouteCase> result = new Dictionary<string, RouteCase>();
                IRouteCaseDataManager dataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<IRouteCaseDataManager>();
                dataManager.SwitchId = switchId;
                IEnumerable<RouteCase> routeCases = dataManager.GetAllRouteCases();
                if (routeCases != null)
                {
                    foreach (RouteCase routeCase in routeCases)
                        result.Add(routeCase.RouteCaseOptionsAsString, routeCase);
                }
                return result;
            });
        }

        #region Private Classes

        private class RouteCaseCacheManager : BaseCacheManager
        {

        }

        private class RouteCaseCacheExpirationChecker : CacheExpirationChecker
        {
            static RouteCaseCacheExpirationChecker s_instance = new RouteCaseCacheExpirationChecker();
            public static RouteCaseCacheExpirationChecker Instance
            {
                get
                {
                    return s_instance;
                }
            }

            public override bool IsCacheExpired(Vanrise.Caching.ICacheExpirationCheckerContext context)
            {
                TimeSpan entitiesTimeSpan = TimeSpan.FromMinutes(15);
                SlidingWindowCacheExpirationChecker slidingWindowCacheExpirationChecker = new SlidingWindowCacheExpirationChecker(entitiesTimeSpan);
                return slidingWindowCacheExpirationChecker.IsCacheExpired(context);
            }
        }
        #endregion
    }
}
