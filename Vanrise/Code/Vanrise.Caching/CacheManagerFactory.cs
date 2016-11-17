using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Vanrise.Common;

namespace Vanrise.Caching
{
    public static class CacheManagerFactory
    {
        internal static VRDictionary<Type, ICacheManager> s_defaultCacheManagers = new VRDictionary<Type, ICacheManager>(true);

        public static T GetCacheManager<T>(Guid? cacheManagerId = null) where T : class, ICacheManager
        {
            return GetCacheManager(typeof(T), cacheManagerId) as T;
        }

        public static ICacheManager GetCacheManager(Type cacheManagerType, Guid? cacheManagerId = null)
        {
            if (!cacheManagerId.HasValue)
            {
                ICacheManager cacheManager;
                if (!s_defaultCacheManagers.TryGetValue(cacheManagerType, out cacheManager))
                {
                    lock (s_defaultCacheManagers)
                    {
                        if (!s_defaultCacheManagers.TryGetValue(cacheManagerType, out cacheManager))
                        {
                            s_defaultCacheManagers.TryAdd(cacheManagerType, CreateCacheManagerObj(cacheManagerType));
                            s_defaultCacheManagers.TryGetValue(cacheManagerType, out cacheManager);
                        }
                    }
                }

                return cacheManager;
            }
            else
            {
                var cacheManager = GetCacheManager<CacheManagerForManagers>();
                return cacheManager.GetOrCreateObject(String.Format("CacheManager_{0}", cacheManagerId),
                    () =>
                    {
                        return CreateCacheManagerObj(cacheManagerType);
                    });
            }
        }


        public static void RemoveCacheManager(Guid cacheManagerId)
        {
            var cacheManager = GetCacheManager<CacheManagerForManagers>();
            cacheManager.RemoveObjectFromCache(String.Format("CacheManager_{0}", cacheManagerId));
        }

        //private static T CreateCacheManagerObj<T>()
        //{
        //    //if (!typeof(BaseCacheManager<>).IsAssignableFrom(typeof(T)))
        //    if (!IsSubclassOfRawGeneric(typeof(BaseCacheManager<>), typeof(T)))
        //        throw new ArgumentException("T should inherits CacheManager<>", "T");
        //    return Activator.CreateInstance<T>();
        //}

        private static ICacheManager CreateCacheManagerObj(Type cacheManagerType)
        {
            //if (!typeof(BaseCacheManager<>).IsAssignableFrom(typeof(T)))
            if (!IsSubclassOfRawGeneric(typeof(BaseCacheManager<>), cacheManagerType))
                throw new ArgumentException("cacheManagerType should inherits CacheManager<>", "cacheManagerType");
            return Activator.CreateInstance(cacheManagerType) as ICacheManager;
        }

        static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        public static void SetCacheObjectCleanable(CachedObject cachedObject)
        {
            CacheCleaner.SetCacheObjectCleanable(cachedObject);
        }
    }

    internal class CacheManagerForManagers : BaseCacheManager
    {
        public override CacheObjectSize ApproximateObjectSize
        {
            get
            {
                return CacheObjectSize.ExtraSmall;
            }
        }
    }
}
