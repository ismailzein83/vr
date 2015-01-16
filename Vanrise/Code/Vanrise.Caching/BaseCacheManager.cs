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
            ConcurrentDictionary<string, object> objectTypeCaches = GetCacheDictionary(cacheName, parameter);
            object dummy;
            objectTypeCaches.TryRemove(cacheName, out dummy);
        }

        #endregion

        #region Overridable

        internal protected virtual void RefreshCache()
        {
        }

        protected virtual bool TryGetObjectFromCache<T>(string cacheName, ParamType parameter, out T cachedObject)
        {
            ConcurrentDictionary<string, object> objectTypeCaches = GetCacheDictionary(cacheName, parameter);

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
            ConcurrentDictionary<string, object> objectTypeCaches = GetCacheDictionary(cacheName, parameter);
            objectTypeCaches.TryAdd(cacheName, obj);
        }
                
        protected virtual ConcurrentDictionary<string, object> GetCacheDictionary(string cacheName, ParamType parameter)
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

        #endregion        
    }
}
