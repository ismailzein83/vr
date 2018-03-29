using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Ericsson.Entities;
using Vanrise.Caching;

namespace TOne.WhS.RouteSync.Ericsson.Business
{
    public class RouteCaseManager
    {
        public List<RouteCase> GetRouteCases()
        {
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<RouteCaseCacheManager>();

            throw new NotImplementedException();
            //return cacheManager.GetOrCreateObject("RouteCases", RouteCaseCacheExpirationChecker.Instance, () =>
            //{
                
            //});
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
