using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Vanrise.Caching
{
    public enum CacheObjectSize : short
    {
        ExtraSmall = 0, Small = 1, Medium = 2, Large = 3, ExtraLarge = 4
    }
    public interface ICacheManager
    {
        IEnumerable<CachedObject> GetAllCachedObjects();

        void RemoveObjectFromCache(CachedObject cachedObject);

        CacheObjectSize ApproximateObjectSize { get; }
    }

    public abstract class BaseCacheManager<ParamType> : ICacheManager
    {
        protected internal BaseCacheManager()
        {
        }

        #region Local Variables

        ConcurrentDictionary<string, object> _cacheNameLocks = new ConcurrentDictionary<string, object>();
        ConcurrentDictionary<ParamType, ConcurrentDictionary<string, CachedObject>> _cacheDictionaries = new ConcurrentDictionary<ParamType, ConcurrentDictionary<string, CachedObject>>();

        #endregion

        #region Public Methods

        public T GetOrCreateObject<T>(string cacheName, ParamType parameter, Func<T> createObject)
        {            
            CachedObject cachedObject;
            CheckCacheDictionaryExpiration(parameter);
            if (!TryGetObjectFromCache(cacheName, parameter, out cachedObject))
            {
                lock (GetCacheNameLockObject(cacheName, parameter))
                {
                    if (!TryGetObjectFromCache(cacheName, parameter, out cachedObject))
                    {
                        T obj = createObject();
                        cachedObject = new CachedObject(obj)
                        {
                            CacheName = cacheName,
                            ApproximateSize = this.ApproximateObjectSize
                        };
                        AddObjectToCache(cacheName, parameter, cachedObject);
                    }
                }
            }
            lock (cachedObject)
            {
                cachedObject.LastAccessedTime = DateTime.Now;
            }
            return cachedObject.Object != null ? (T)cachedObject.Object : default(T);
        }

        public virtual void RemoveObjectFromCache(string cacheName, ParamType parameter)
        {
            ConcurrentDictionary<string, CachedObject> objectTypeCaches = GetCacheDictionary(parameter);
            CachedObject dummy;
            objectTypeCaches.TryRemove(cacheName, out dummy);
        }

        public virtual void RemoveObjectFromCache(CachedObject cachedObject)
        {
            foreach(var cacheDictionary in _cacheDictionaries.Values)
            {
                CachedObject matchObj;
                if (cacheDictionary.TryGetValue(cachedObject.CacheName, out matchObj))
                {
                    if(matchObj == cachedObject)
                    {
                        cacheDictionary.TryRemove(cachedObject.CacheName, out matchObj);
                        break;
                    }
                }
            }
        }


        public virtual IEnumerable<CachedObject> GetAllCachedObjects()
        {
            return _cacheDictionaries.Values.SelectMany(itm => itm.Values);
        }

        public virtual CacheObjectSize ApproximateObjectSize
        {
            get
            {
                return CacheObjectSize.Medium;
            }
        }

        #endregion

        #region Overridable

        protected virtual bool TryGetObjectFromCache(string cacheName, ParamType parameter, out CachedObject cachedObject)
        {
            ConcurrentDictionary<string, CachedObject> objectTypeCaches = GetCacheDictionary(parameter);
            return objectTypeCaches.TryGetValue(cacheName, out cachedObject);
        }

        protected virtual void AddObjectToCache(string cacheName, ParamType parameter, CachedObject cachedObject)
        {
            ConcurrentDictionary<string, CachedObject> objectTypeCaches = GetCacheDictionary(parameter);
            objectTypeCaches.TryAdd(cacheName, cachedObject);
        }

        protected virtual ConcurrentDictionary<string, CachedObject> GetCacheDictionary(ParamType parameter)
        {
            ConcurrentDictionary<string, CachedObject> objectTypeCaches;
            if (!_cacheDictionaries.ContainsKey(parameter))
                _cacheDictionaries.TryAdd(parameter, new ConcurrentDictionary<string, CachedObject>());

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
                    SetCacheExpired(parameter);
                }

                    
                cacheDictionaryInfo.LastExpirationCheckTime = DateTime.Now;
            }            
        }

        public void SetCacheExpired(ParamType parameter)
        {
            ConcurrentDictionary<string, CachedObject> cacheDictionary = GetCacheDictionary(parameter);
            if (cacheDictionary != null)
                cacheDictionary.Clear();
        }

        private class CacheDictionaryInfo
        {
            public DateTime LastExpirationCheckTime { get; set; }
        }

        #endregion
    }

    public abstract class BaseCacheManager : BaseCacheManager<Object>
    {
        ConcurrentDictionary<string, CachedObject> _cacheDictionary = new ConcurrentDictionary<string, CachedObject>();

        protected override ConcurrentDictionary<string, CachedObject> GetCacheDictionary(object parameter)
        {
            return _cacheDictionary;
        }

        public override IEnumerable<CachedObject> GetAllCachedObjects()
        {
            return _cacheDictionary.Values;
        }

        object dummyParameterType = new object();

        public T GetOrCreateObject<T>(string cacheName, Func<T> createObject)
        {
            return base.GetOrCreateObject(cacheName, dummyParameterType, createObject);
        }

        public void RemoveObjectFromCache(string cacheName)
        {
            base.RemoveObjectFromCache(cacheName, dummyParameterType);
        }

        public override void RemoveObjectFromCache(CachedObject cachedObject)
        {
            CachedObject dummy;
            _cacheDictionary.TryRemove(cachedObject.CacheName, out dummy);
        }

        protected virtual bool ShouldSetCacheExpired()
        {
            return false;
        }

        protected override bool ShouldSetCacheExpired(object parameter)
        {
            return ShouldSetCacheExpired();
        }

        public void SetCacheExpired()
        {
            base.SetCacheExpired(dummyParameterType);
        }
    }
}
