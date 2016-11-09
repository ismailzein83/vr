using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Configuration;
using System.Reflection;
using System.Timers;
using System.Collections;
using Vanrise.Common;

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

        ConcurrentDictionary<Object, object> _cacheNameLocks = new ConcurrentDictionary<Object, object>();
        ConcurrentDictionary<ParamType, CacheStore> _cacheDictionaries = new ConcurrentDictionary<ParamType, CacheStore>();

        #endregion

        #region Public Methods

        public virtual T GetOrCreateObject<T>(Object cacheName, ParamType parameter, CacheExpirationChecker cacheExpirationChecker, Func<T> createObject)
        {
            CachedObject cachedObject;
            CacheStore cacheDictionary = GetCacheDictionary(parameter);
            CheckCacheDictionaryExpiration(parameter, cacheDictionary);
            if (!cacheDictionary.TryGetValue(cacheName, out cachedObject) || IsCacheObjectExpired(cachedObject))
            {
                lock (GetCacheNameLockObject(cacheName, parameter))
                {
                    if (!cacheDictionary.TryGetValue(cacheName, out cachedObject) || IsCacheObjectExpired(cachedObject))
                    {
                        T obj = createObject();
                        cachedObject = new CachedObject(obj)
                        {
                            CacheName = cacheName,
                            ApproximateSize = this.ApproximateObjectSize,
                            CacheExpirationChecker = cacheExpirationChecker,
                            CacheManager = this,
                            AdditionalInfo = parameter,
                            LastAccessedTime = VRClock.Now
                        };
                        if (cachedObject.CacheExpirationChecker != null)
                            CacheManagerFactory.SetCacheObjectCleanable(cachedObject);
                        cacheDictionary.Add(cacheName, cachedObject);
                    }
                }
            }
            lock (cachedObject)
            {
                cachedObject.LastAccessedTime = VRClock.Now;
            }
            return cachedObject.Object != null ? (T)cachedObject.Object : default(T);
        }

        public virtual T GetOrCreateObject<T>(Object cacheName, ParamType parameter, Func<T> createObject)
        {
            return GetOrCreateObject<T>(cacheName, parameter, null, createObject);
        }

        private bool IsCacheObjectExpired(CachedObject cachedObject)
        {
            if (this.IsTimeExpirable)
            {
                if ((VRClock.Now - cachedObject.CreatedTime) > _timeExpirationInterval)
                    return true;
            }
            return false;
        }

        public virtual void RemoveObjectFromCache(Object cacheName, ParamType parameter)
        {
            CacheStore objectTypeCaches = GetCacheDictionary(parameter);
           objectTypeCaches.TryRemove(cacheName);
        }

        public virtual void RemoveObjectFromCache(CachedObject cachedObject)
        {
            ParamType parameter = cachedObject.AdditionalInfo != null ? (ParamType)cachedObject.AdditionalInfo : default(ParamType);
            this.RemoveObjectFromCache(cachedObject.CacheName, parameter);
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
            CacheStore cacheDictionary = GetCacheDictionary(parameter);
            CheckCacheDictionaryExpiration(parameter, cacheDictionary);
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
            CacheStore cacheDictionary = GetCacheDictionary(parameter);
            if (cacheDictionary != null)
                cacheDictionary.Clear();
            _cacheExpirationTimes.AddOrUpdate(parameter, VRClock.Now, (prm, existingValue) => VRClock.Now);
        }


        #endregion

        #region Overridable

        protected virtual CacheStore GetCacheDictionary(ParamType parameter)
        {
            CacheStore objectTypeCaches;
            if (!_cacheDictionaries.TryGetValue(parameter, out objectTypeCaches))
            {
                _cacheDictionaries.TryAdd(parameter, new CacheStore());
                objectTypeCaches = _cacheDictionaries[parameter];
            }
            return objectTypeCaches;
        }

        protected virtual object GetCacheNameLockObject(Object cacheName, ParamType parameter)
        {
            var key = new FullCacheName { CacheName = cacheName, Parameter = parameter };// String.Format("{0}_{1}", cacheName, parameter);
            Object lockObj;
            if (!_cacheNameLocks.TryGetValue(key, out lockObj))
            {
                _cacheNameLocks.TryAdd(key, new object());
                lockObj = _cacheNameLocks[key];
            }

            return lockObj;
        }

        private struct FullCacheName
        {
            public Object CacheName { get; set; }

            public ParamType Parameter { get; set; }
        }

        protected virtual bool ShouldSetCacheExpired(ParamType parameter)
        {
            return false;
        }

        #endregion

        #region Private Methods



        private void CheckCacheDictionaryExpiration(ParamType parameter, CacheStore cacheDictionaryInfo)
        {            
            if ((VRClock.Now - cacheDictionaryInfo.LastExpirationCheckTime).TotalSeconds <= 4)//dont check expiration if it is checked recently
                return;
            lock (cacheDictionaryInfo)
            {
                if ((VRClock.Now - cacheDictionaryInfo.LastExpirationCheckTime).TotalSeconds <= 4)
                    return;
                bool isExpired;
                if (!RuntimeCacheFactory.GetCacheExpirationChecker().TryCheckExpirationFromRuntimeService(this.GetType(), parameter, out isExpired))
                    isExpired = ShouldSetCacheExpired(parameter);
                if (isExpired)
                {
                    SetCacheExpired(parameter);
                }

                cacheDictionaryInfo.LastExpirationCheckTime = VRClock.Now;
            }
        }

        #endregion
    }
}
