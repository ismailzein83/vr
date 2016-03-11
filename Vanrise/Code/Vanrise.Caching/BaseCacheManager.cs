using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Configuration;

namespace Vanrise.Caching
{
    public abstract class BaseCacheManager : BaseCacheManager<Object>
    {
        object dummyParameter = new object();
        ConcurrentDictionary<string, CachedObject> _cacheDictionary = new ConcurrentDictionary<string, CachedObject>();

        public override T GetOrCreateObject<T>(string cacheName, object parameter, Func<T> createObject)
        {
            return base.GetOrCreateObject<T>(cacheName, dummyParameter, createObject);
        }

        protected override ConcurrentDictionary<string, CachedObject> GetCacheDictionary(object parameter)
        {
            return _cacheDictionary;
        }

        public override IEnumerable<CachedObject> GetAllCachedObjects()
        {
            return _cacheDictionary.Values;
        }

        public T GetOrCreateObject<T>(string cacheName, Func<T> createObject)
        {
            return base.GetOrCreateObject(cacheName, dummyParameter, createObject);
        }

        public void RemoveObjectFromCache(string cacheName)
        {
            base.RemoveObjectFromCache(cacheName, dummyParameter);
        }

        public override void RemoveObjectFromCache(CachedObject cachedObject)
        {
            CachedObject dummy;
            _cacheDictionary.TryRemove(cachedObject.CacheName, out dummy);
        }

        protected override bool ShouldSetCacheExpired(object parameter)
        {
            return ShouldSetCacheExpired();
        }

        protected virtual bool ShouldSetCacheExpired()
        {
            return base.ShouldSetCacheExpired(dummyParameter);
        }

        public override void SetCacheExpired(object parameter)
        {
            this.SetCacheExpired();
        }

        public void SetCacheExpired()
        {
            base.SetCacheExpired(dummyParameter);
        }

        public override bool IsCacheExpired(object parameter, ref DateTime? lastCheckTime)
        {
            return this.IsCacheExpired(ref lastCheckTime);
        }
        public bool IsCacheExpired(ref DateTime? lastCheckTime)
        {
            return base.IsCacheExpired(dummyParameter, ref lastCheckTime);
        }
    }
}
