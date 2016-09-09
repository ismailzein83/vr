using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Caching
{
    public interface ICacheManager
    {
        IEnumerable<CachedObject> GetAllCachedObjects();

        void RemoveObjectFromCache(CachedObject cachedObject);

        CacheObjectSize ApproximateObjectSize { get; }
    }

    public interface ICacheManager<ParamType> : ICacheManager
    {
        bool IsCacheExpired(ParamType parameter, ref DateTime? lastCheckTime);
    }
}
