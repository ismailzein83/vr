using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Vanrise.Caching
{
    public interface ICacheManager
    {
    }

    public abstract class BaseCacheManager<ParamType> : ICacheManager
    {
        protected internal BaseCacheManager()
        {
        }

        #region Local Variables

        ConcurrentDictionary<string, object> _cacheNameLocks = new ConcurrentDictionary<string, object>();
        ConcurrentDictionary<ParamType, ConcurrentDictionary<string, object>> _cacheDictionaries = new ConcurrentDictionary<ParamType, ConcurrentDictionary<string, object>>();

        #endregion

        #region Public Methods

        public T GetOrCreateObject<T>(string cacheName, ParamType parameter, Func<T> createObject)
        {            
            T obj;
            CheckCacheDictionaryExpiration(parameter);
            if (!TryGetObjectFromCache(cacheName, parameter, out obj))
            {
                lock (GetCacheNameLockObject(cacheName, parameter))
                {
                    if (!TryGetObjectFromCache(cacheName, parameter, out obj))
                    {
                        obj = createObject();
                        AddObjectToCache(cacheName, parameter, obj);
                    }
                }
            }
            return obj;
        }

        public virtual void RemoveObjectFromCache(string cacheName, ParamType parameter)
        {
            ConcurrentDictionary<string, object> objectTypeCaches = GetCacheDictionary(parameter);
            object dummy;
            objectTypeCaches.TryRemove(cacheName, out dummy);
        }

        #endregion

        #region Overridable

        protected virtual bool TryGetObjectFromCache<T>(string cacheName, ParamType parameter, out T cachedObject)
        {
            ConcurrentDictionary<string, object> objectTypeCaches = GetCacheDictionary(parameter);

            object obj;
            bool exists = objectTypeCaches.TryGetValue(cacheName, out obj);
            if (exists)
                cachedObject = (T)obj;
            else
                cachedObject = default(T);
            return exists;
        }

        protected virtual void AddObjectToCache<T>(string cacheName, ParamType parameter, T obj)
        {
            ConcurrentDictionary<string, object> objectTypeCaches = GetCacheDictionary(parameter);
            objectTypeCaches.TryAdd(cacheName, obj);
        }
                
        protected virtual ConcurrentDictionary<string, object> GetCacheDictionary(ParamType parameter)
        {
            ConcurrentDictionary<string, object> objectTypeCaches;
            if (!_cacheDictionaries.ContainsKey(parameter))
                _cacheDictionaries.TryAdd(parameter, new ConcurrentDictionary<string, object>());

            objectTypeCaches = _cacheDictionaries[parameter];
            return objectTypeCaches;
        }

        protected virtual object GetCacheNameLockObject(string cacheName, ParamType parameter)
        {
            string key = String.Format("{0}_{1}", cacheName, parameter);
            if (!_cacheNameLocks.ContainsKey(key))
                _cacheNameLocks.TryAdd(key, new object());

            return _cacheNameLocks[key];
        }

        protected virtual bool ShouldSetCacheExpired(ParamType parameter)
        {
            return false;
        }

        #endregion       
 
        #region Private Methods

        ConcurrentDictionary<ParamType, CacheDictionaryInfo> _cacheDictionariesInfo = new ConcurrentDictionary<ParamType, CacheDictionaryInfo>();

        private void CheckCacheDictionaryExpiration(ParamType parameter)
        {
            CacheDictionaryInfo cacheDictionaryInfo;
            if (!_cacheDictionariesInfo.TryGetValue(parameter, out cacheDictionaryInfo))
            {
                cacheDictionaryInfo = new CacheDictionaryInfo();
                if (!_cacheDictionariesInfo.TryAdd(parameter, cacheDictionaryInfo))
                    cacheDictionaryInfo = _cacheDictionariesInfo[parameter];
            }
            lock (cacheDictionaryInfo)
            {
                if ((DateTime.Now - cacheDictionaryInfo.LastExpirationCheckTime).TotalSeconds <= 2)//dont check expiration if it is checked recently
                    return;
                if (ShouldSetCacheExpired(parameter))
                {
                    ConcurrentDictionary<string, object> cacheDictionary = GetCacheDictionary(parameter);
                    if (cacheDictionary != null)
                        cacheDictionary.Clear();
                }

                    
                cacheDictionaryInfo.LastExpirationCheckTime = DateTime.Now;
            }            
        }

        private class CacheDictionaryInfo
        {
            public DateTime LastExpirationCheckTime { get; set; }
        }

        #endregion
    }

    public abstract class BaseCacheManager : BaseCacheManager<Object>
    {
        ConcurrentDictionary<string, object> _cacheDictionary = new ConcurrentDictionary<string, object>();

        protected override ConcurrentDictionary<string, object> GetCacheDictionary(object parameter)
        {
            return _cacheDictionary;
        }

        public T GetOrCreateObject<T>(string cacheName, Func<T> createObject)
        {
            return base.GetOrCreateObject(cacheName, null, createObject);
        }
    }
}
