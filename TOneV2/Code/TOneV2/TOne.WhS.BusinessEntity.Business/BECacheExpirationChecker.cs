using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Business
{
    public class BECacheExpirationChecker : Vanrise.Caching.SlidingWindowCacheExpirationChecker
    {
        static BECacheExpirationChecker s_instance = new BECacheExpirationChecker();
        public static BECacheExpirationChecker Instance
        {
            get
            {
                return s_instance;
            }
        }
        public BECacheExpirationChecker() : base(TimeSpan.FromMinutes(10))//10 should be retrieved from the ConfigManager
        {

        }

        public override bool IsCacheExpired(Vanrise.Caching.ICacheExpirationCheckerContext context)
        {
            IBEDayFilterCacheName dayFilterCacheName = context.CachedObject.CacheName as IBEDayFilterCacheName;
            if (dayFilterCacheName != null && dayFilterCacheName.FilterDay == DateTime.Today)
                return false;
            else
                return base.IsCacheExpired(context);
        }
    }

    public interface IBEDayFilterCacheName
    {
        DateTime FilterDay { get; }
    }
}
