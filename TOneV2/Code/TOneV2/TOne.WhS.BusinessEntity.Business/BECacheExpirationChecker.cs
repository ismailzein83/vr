using System;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Caching;

namespace TOne.WhS.BusinessEntity.Business
{
    public class BECacheExpirationChecker : CacheExpirationChecker
    {
        static BECacheExpirationChecker s_instance = new BECacheExpirationChecker();
        public static BECacheExpirationChecker Instance
        {
            get
            {
                return s_instance;
            }
        }

        public override bool IsCacheExpired(Vanrise.Caching.ICacheExpirationCheckerContext context)
        {
            ConfigManager configManager = new ConfigManager();
            CachingExpirationIntervals cachingExpirationIntervals = configManager.GetCachingExpirationIntervals();

            IBEDayFilterCacheName dayFilterCacheName = context.CachedObject.CacheName as IBEDayFilterCacheName;
            if (dayFilterCacheName != null && dayFilterCacheName.FilterDay == DateTime.Today)
            {
                if (!cachingExpirationIntervals.TodayEntitiesIntervalInMinutes.HasValue)
                    return false;
                return IsCacheExpired(context, cachingExpirationIntervals.TodayEntitiesIntervalInMinutes.Value);
            }
            else
            {
                return IsCacheExpired(context, cachingExpirationIntervals.PreviousEntitiesIntervalInMinutes);
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