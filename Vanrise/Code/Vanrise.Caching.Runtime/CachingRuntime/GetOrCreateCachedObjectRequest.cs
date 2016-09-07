using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Caching.Runtime
{
    public class GetOrCreateCachedObjectRequest<T, Q> : Vanrise.Runtime.Entities.InterRuntimeServiceRequest<string> where T : BaseCacheManager
    {
        public string CacheName { get; set; }

        public CachedObjectCreationHandler<Q> ObjectCreationHandler { get; set; }

        public override string Execute()
        {
            return CacheManagerFactory.GetCacheManager<T>().GetOrCreateObject(this.CacheName, () =>
                {
                    var cacheFullName = DistributedCacher.BuildCacheFullName<T>(this.CacheName);
                    lock (CachingRuntimeService.s_currentCacheFullNames)
                    {
                        CachingRuntimeService.s_currentCacheFullNames.Add(cacheFullName);
                    }
                    var obj = this.ObjectCreationHandler.CreateObject();
                    return obj != null ? Vanrise.Common.Serializer.Serialize(obj) : null;
                });
        }
    }
}
