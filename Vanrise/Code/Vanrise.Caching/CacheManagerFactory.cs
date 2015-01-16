﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Vanrise.Caching
{
    public static class CacheManagerFactory
    {
        static ConcurrentDictionary<Type, ICacheManager> s_defaultCacheManagers = new ConcurrentDictionary<Type, ICacheManager>();

        public static T GetCacheManager<T>(Guid? cacheManagerId = null) where T : class, ICacheManager
        {
            if (cacheManagerId == null)
            {
                if (!s_defaultCacheManagers.ContainsKey(typeof(T)))
                    s_defaultCacheManagers.TryAdd(typeof(T), CreateCacheManagerObj<T>());

                return s_defaultCacheManagers[typeof(T)] as T;
            }
            else
            {
                var cacheManager = GetCacheManager<SystemCacheManager>();
                return cacheManager.GetOrCreateObject(String.Format("CacheManager_{0}", cacheManagerId),
                    SystemCacheObjectType.CacheManager,
                    () =>
                    {
                        return CreateCacheManagerObj<T>();
                    });
            }
        }

        public static void RemoveCacheManager(Guid cacheManagerId)
        {
            var cacheManager = GetCacheManager<SystemCacheManager>();
            cacheManager.RemoveObjectFromCache(String.Format("CacheManager_{0}", cacheManagerId), SystemCacheObjectType.CacheManager);
        }

        private static T CreateCacheManagerObj<T>()
        {
            //if (!typeof(BaseCacheManager<>).IsAssignableFrom(typeof(T)))
            if (!IsSubclassOfRawGeneric(typeof(BaseCacheManager<>), typeof(T)))
                throw new ArgumentException("T should inherits CacheManager<>", "T");
            return Activator.CreateInstance<T>();
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
    }

    internal enum SystemCacheObjectType { CacheManager }
    internal class SystemCacheManager : BaseCacheManager<SystemCacheObjectType>
    {
    }
}
