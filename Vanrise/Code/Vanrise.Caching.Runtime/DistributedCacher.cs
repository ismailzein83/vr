using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Caching.Runtime
{
    public class DistributedCacher : IDistributedCacher
    {
        //#region Singleton

        //static DistributedCacher s_current = new DistributedCacher();

        //public static DistributedCacher Current
        //{
        //    get
        //    {
        //        return s_current;
        //    }
        //}

        //#endregion

        Vanrise.Runtime.InterRuntimeServiceManager _interRuntimeServiceManager = new Vanrise.Runtime.InterRuntimeServiceManager();
        public Q GetOrCreateObject<T, Q>(string cacheName, Func<CachedObjectCreationHandler<Q>> getObjectCreationHandler) where T : BaseCacheManager
        {
            return Caching.CacheManagerFactory.GetCacheManager<T>().GetOrCreateObject(cacheName, () =>
                {
                    var cacheFullName = BuildCacheFullName<T>(cacheName);
                    int? runtimeProcessId = GetCachingRuntimeProcessId(cacheFullName);
                    if (runtimeProcessId.HasValue)
                    {
                        var getOrCreateCachedObjectRequest = new GetOrCreateCachedObjectRequest<T, Q>
                        {
                            CacheName = cacheName,
                            ObjectCreationHandler = getObjectCreationHandler()
                        };
                        var serializedObj = _interRuntimeServiceManager.SendRequest(runtimeProcessId.Value, getOrCreateCachedObjectRequest);
                        return serializedObj != null ? Common.Utilities.DeserializeAndValidate<Q>(serializedObj) : default(Q);
                    }
                    else
                        return getObjectCreationHandler().CreateObject();
                });
        }

        private int? GetCachingRuntimeProcessId(string cacheFullName)
        {
            var distributorRuntimeProcessId = Vanrise.Runtime.SingletonServiceProcessIdGetter.GetRuntimeProcessId(CachingDistributorRuntimeService.SERVICE_TYPE_UNIQUE_NAME);
            if (distributorRuntimeProcessId.HasValue)
            {
                try
                {
                    return new Vanrise.Runtime.InterRuntimeServiceManager().SendRequest(distributorRuntimeProcessId.Value, new GetCachingRuntimeProcessIdRequest
                        {
                            CacheFullName = cacheFullName
                        });
                }
                catch
                {
                    Vanrise.Runtime.SingletonServiceProcessIdGetter.ResetRuntimeProcessId(CachingDistributorRuntimeService.SERVICE_TYPE_UNIQUE_NAME);
                }
            }
            return null;
        }

        internal static string BuildCacheFullName<T>(string cacheName) where T : BaseCacheManager
        {
            return string.Format("{0}_{1}", typeof(T).AssemblyQualifiedName, cacheName);
        }
    }
}
