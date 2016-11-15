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

        SlidingWindowCacheExpirationChecker todayBECacheExpirationChecker;
        SlidingWindowCacheExpirationChecker previousBECacheExpirationChecker;
        public BECacheExpirationChecker()
        {
            InitializeSlidingWindowCacheExpirationCheckers();
        }

        public override bool IsCacheExpired(Vanrise.Caching.ICacheExpirationCheckerContext context)
        {
            IBEDayFilterCacheName dayFilterCacheName = context.CachedObject.CacheName as IBEDayFilterCacheName;
            if (dayFilterCacheName != null && dayFilterCacheName.FilterDay == DateTime.Today)
                return todayBECacheExpirationChecker.IsCacheExpired(context);
            else
                return previousBECacheExpirationChecker.IsCacheExpired(context);
        }

        private void InitializeSlidingWindowCacheExpirationCheckers()
        {
            ConfigManager configManager = new ConfigManager();
            CachingExpirationIntervals cachingExpirationIntervals = configManager.GetCachingExpirationIntervals();

            TimeSpan? todayEntitesTimeSpan = null;
            if (cachingExpirationIntervals.TodayEntitiesIntervalInMinutes.HasValue)
                todayEntitesTimeSpan = TimeSpan.FromMinutes(cachingExpirationIntervals.TodayEntitiesIntervalInMinutes.Value);

            TimeSpan? previousEntitesTimeSpan = null;
            if (cachingExpirationIntervals.PreviousEntitiesIntervalInMinutes.HasValue)
                previousEntitesTimeSpan = TimeSpan.FromMinutes(cachingExpirationIntervals.PreviousEntitiesIntervalInMinutes.Value);

            todayBECacheExpirationChecker = new SlidingWindowCacheExpirationChecker(todayEntitesTimeSpan);
            previousBECacheExpirationChecker = new SlidingWindowCacheExpirationChecker(previousEntitesTimeSpan);
        }
    }

    public interface IBEDayFilterCacheName
    {
        DateTime FilterDay { get; }
    }
}