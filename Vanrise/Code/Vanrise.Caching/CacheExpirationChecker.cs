using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Caching
{
    public abstract class CacheExpirationChecker
    {
        public abstract bool IsCacheExpired(ICacheExpirationCheckerContext context);
    }

    public interface ICacheExpirationCheckerContext
    {
        CachedObject CachedObject { get; }
    }

    public class CacheExpirationCheckerContext : ICacheExpirationCheckerContext
    {
        public CachedObject CachedObject { get; set; }
    }
}
