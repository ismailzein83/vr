﻿using System;
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

        public virtual T GetOrCreateObject<T>(Object cacheName, ParamType parameter, Func<T> createObject)
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
                cachedObject.LastAccessedTime = VRClock.Now;
            }
            return cachedObject.Object != null ? (T)cachedObject.Object : default(T);
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

        public virtual void RemoveObjectFromCache(string cacheName, ParamType parameter)
        {
            CacheStore objectTypeCaches = GetCacheDictionary(parameter);
           objectTypeCaches.TryRemove(cacheName);
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
                        cacheDictionary.TryRemove(cachedObject.CacheName);
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
            CacheStore cacheDictionary = GetCacheDictionary(parameter);
            if (cacheDictionary != null)
                cacheDictionary.Clear();
            _cacheExpirationTimes.AddOrUpdate(parameter, VRClock.Now, (prm, existingValue) => VRClock.Now);
        }


        #endregion

        #region Overridable

        protected virtual bool TryGetObjectFromCache(Object cacheName, ParamType parameter, out CachedObject cachedObject)
        {
            CacheStore objectTypeCaches = GetCacheDictionary(parameter);
            return objectTypeCaches.TryGetValue(cacheName, out cachedObject);
        }

        protected virtual void AddObjectToCache(Object cacheName, ParamType parameter, CachedObject cachedObject)
        {
            CacheStore objectTypeCaches = GetCacheDictionary(parameter);
            objectTypeCaches.Add(cacheName, cachedObject);
        }

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

        protected virtual CacheDictionaryInfo GetCacheDictionaryInfo(ParamType parameter)
        {
            CacheDictionaryInfo cacheDictionaryInfo;
            if (!_cacheDictionariesInfo.TryGetValue(parameter, out cacheDictionaryInfo))
            {
                cacheDictionaryInfo = new CacheDictionaryInfo();
                if (!_cacheDictionariesInfo.TryAdd(parameter, cacheDictionaryInfo))
                    cacheDictionaryInfo = _cacheDictionariesInfo[parameter];
            }
            return cacheDictionaryInfo;
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

        ConcurrentDictionary<ParamType, CacheDictionaryInfo> _cacheDictionariesInfo = new ConcurrentDictionary<ParamType, CacheDictionaryInfo>();

        private void CheckCacheDictionaryExpiration(ParamType parameter)
        {
            CacheDictionaryInfo cacheDictionaryInfo = GetCacheDictionaryInfo(parameter);
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

    public class CacheDictionaryInfo
    {
        public DateTime LastExpirationCheckTime { get; set; }
    }
    
}
