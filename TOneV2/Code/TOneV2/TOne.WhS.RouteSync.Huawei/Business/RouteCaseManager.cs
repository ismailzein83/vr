using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Huawei.Data;
using TOne.WhS.RouteSync.Huawei.Entities;
using Vanrise.Caching;

namespace TOne.WhS.RouteSync.Huawei.Business
{
    public class RouteCaseManager
    {
        public void Initialize(string switchId)
        {
            IRouteCaseDataManager routeCaseDataManager = RouteSyncHuaweiDataManagerFactory.GetDataManager<IRouteCaseDataManager>();
            routeCaseDataManager.SwitchId = switchId;
            routeCaseDataManager.Initialize(new RouteCaseInitializeContext());
        }

        public Dictionary<string, RouteCase> GetCachedRouteCases(string switchId)
        {
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<RouteCaseCacheManager>();
            var cacheName = new GetCachedRouteCasesCacheName() { SwitchId = switchId };

            return cacheManager.GetOrCreateObject(cacheName, RouteCaseCacheExpirationChecker.Instance, () =>
            {
                Dictionary<string, RouteCase> results = new Dictionary<string, RouteCase>();
                IRouteCaseDataManager dataManager = RouteSyncHuaweiDataManagerFactory.GetDataManager<IRouteCaseDataManager>();
                dataManager.SwitchId = switchId;

                IEnumerable<RouteCase> routeCases = dataManager.GetAllRouteCases();
                if (routeCases != null)
                {
                    foreach (RouteCase routeCase in routeCases)
                        results.Add(routeCase.RouteCaseAsString, routeCase);
                }

                return results.Count > 0 ? results : null;
            });
        }

        private class RouteCaseCacheManager : BaseCacheManager
        {

        }

        private struct GetCachedRouteCasesCacheName
        {
            public string SwitchId { get; set; }
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
    }
}
