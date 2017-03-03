using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Caching
{
    public class TimeExpirableCacheExpirationChecker : CacheExpirationChecker
    {
        TimeSpan _expiredAfter;
        public TimeExpirableCacheExpirationChecker(TimeSpan expiredAfter)
        {
            _expiredAfter = expiredAfter;
        }
        public override bool IsCacheExpired(ICacheExpirationCheckerContext context)
        {
            return (DateTime.Now - context.CachedObject.CreatedTime) >= _expiredAfter;
        }
    }
}
