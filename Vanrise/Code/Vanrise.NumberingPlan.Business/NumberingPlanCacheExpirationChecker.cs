using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;

namespace Vanrise.NumberingPlan.Business
{
    public class NumberingPlanCacheExpirationChecker : CacheExpirationChecker
    {
        static NumberingPlanCacheExpirationChecker s_instance = new NumberingPlanCacheExpirationChecker();
        public static NumberingPlanCacheExpirationChecker Instance
        {
            get
            {
                return s_instance;
            }
        }

        public override bool IsCacheExpired(Vanrise.Caching.ICacheExpirationCheckerContext context)
        {
            //ConfigManager configManager = new ConfigManager();
            //CachingExpirationIntervals cachingExpirationIntervals = configManager.GetCachingExpirationIntervals();

            IBEDayFilterCacheName dayFilterCacheName = context.CachedObject.CacheName as IBEDayFilterCacheName;
            if (dayFilterCacheName != null && dayFilterCacheName.FilterDay == DateTime.Today)
            {
                //if (!cachingExpirationIntervals.TodayEntitiesIntervalInMinutes.HasValue) 
                //    return false;

                return IsCacheExpired(context, 10); //ToDo getting 10 from settings
            }
            else
            {
                return IsCacheExpired(context, 10); //ToDo getting 10 from settings
            }
        }

        private bool IsCacheExpired(Vanrise.Caching.ICacheExpirationCheckerContext context, int entitiesIntervalInMinutes)
        {
            TimeSpan entitiesTimeSpan = TimeSpan.FromMinutes(entitiesIntervalInMinutes);
            SlidingWindowCacheExpirationChecker slidingWindowCacheExpirationChecker = new SlidingWindowCacheExpirationChecker(entitiesTimeSpan);
            return slidingWindowCacheExpirationChecker.IsCacheExpired(context);
        }
    }

    public interface IBEDayFilterCacheName
    {
        DateTime FilterDay { get; }
    }
}
