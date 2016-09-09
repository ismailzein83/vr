using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Configuration;
using System.Reflection;

namespace Vanrise.Caching
{
    public abstract class BaseCacheManager<ParamType> : ICacheManager<ParamType>
    {
        ConcurrentDictionary<ParamType, DateTime> _cacheExpirationTimes = new ConcurrentDictionary<ParamType, DateTime>();

        TimeSpan _timeExpirationInterval;

        protected virtual bool IsTimeExpirable
        {
            get
            {
                return false;
            }
        }
        protected internal BaseCacheManager()
        {
            string timeExpirationIntervalConfigKey = string.Format("{0}_TimeExpirationInterval", this.CacheManagerName);
            if (!TimeSpan.TryParse(ConfigurationManager.AppSettings[timeExpirationIntervalConfigKey], out _timeExpirationInterval))
                _timeExpirationInterval = new TimeSpan(0, 5, 0);
        }

        protected virtual string CacheManagerName
        {
            get
            {
                return this.GetType().FullName;
            }
        }

        #region Local Variables

        ConcurrentDictionary<string, object> _cacheNameLocks = new ConcurrentDictionary<string, object>();
        ConcurrentDictionary<ParamType, ConcurrentDictionary<string, CachedObject>> _cacheDictionaries = new ConcurrentDictionary<ParamType, ConcurrentDictionary<string, CachedObject>>();

        #endregion

        #region Public Methods

        public virtual T GetOrCreateObject<T>(string cacheName, ParamType parameter, Func<T> createObject)
        {
            CachedObject cachedObject;
            CheckCacheDictionaryExpiration(parameter);
            if (!TryGetObjectFromCache(cacheName, parameter, out cachedObject) || IsCacheObjectExpired(cachedObject))
            {
                lock (GetCacheNameLockObject(cacheName, parameter))
                {
                    if (!TryGetObjectFromCache(cacheName, parameter, out cachedObject) || IsCacheObjectExpired(cachedObject))
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

        private bool IsCacheObjectExpired(CachedObject cachedObject)
        {
            if (this.IsTimeExpirable)
            {
                if ((DateTime.Now - cachedObject.CreatedTime) > _timeExpirationInterval)
                    return true;
            }
            return false;
        }

        public virtual void RemoveObjectFromCache(string cacheName, ParamType parameter)
        {
            ConcurrentDictionary<string, CachedObject> objectTypeCaches = GetCacheDictionary(parameter);
            CachedObject dummy;
            objectTypeCaches.TryRemove(cacheName, out dummy);
        }

        public virtual void RemoveObjectFromCache(CachedObject cachedObject)
        {
            foreach (var cacheDictionary in _cacheDictionaries.Values)
            {
                CachedObject matchObj;
                if (cacheDictionary.TryGetValue(cachedObject.CacheName, out matchObj))
                {
                    if (matchObj == cachedObject)
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

        public virtual bool IsCacheExpired(ParamType parameter, ref DateTime? lastCheckTime)
        {
            CheckCacheDictionaryExpiration(parameter);
            DateTime lastExpirationTime;
            _cacheExpirationTimes.TryGetValue(parameter, out lastExpirationTime);

            if (!lastCheckTime.HasValue || lastExpirationTime > lastCheckTime.Value)
            {
                lastCheckTime = lastExpirationTime;
                return true;
            }
            else
                return false;
        }
        
        public virtual void SetCacheExpired(ParamType parameter)
        {
            ConcurrentDictionary<string, CachedObject> cacheDictionary = GetCacheDictionary(parameter);
            if (cacheDictionary != null)
                cacheDictionary.Clear();
            _cacheExpirationTimes.AddOrUpdate(parameter, DateTime.Now, (prm, existingValue) => DateTime.Now);
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
                bool isExpired;
                if (!RuntimeCacheFactory.GetCacheExpirationChecker().TryCheckExpirationFromRuntimeService(this.GetType(), parameter, out isExpired))
                    isExpired = ShouldSetCacheExpired(parameter);
                if (isExpired)
                {
                    SetCacheExpired(parameter);
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
}
