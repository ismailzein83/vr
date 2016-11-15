using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Caching
{
    public class SlidingWindowCacheExpirationChecker : CacheExpirationChecker
    {
        TimeSpan? _cacheIntervalAfterLastAccess;
        public SlidingWindowCacheExpirationChecker(TimeSpan? cacheIntervalAfterLastAccess)
        {
            _cacheIntervalAfterLastAccess = cacheIntervalAfterLastAccess;
        }

        public override bool IsCacheExpired(ICacheExpirationCheckerContext context)
        {
            if (!_cacheIntervalAfterLastAccess.HasValue)
                return false;

            return (VRClock.Now - context.CachedObject.LastAccessedTime) > _cacheIntervalAfterLastAccess.Value;
        }
    }
}
