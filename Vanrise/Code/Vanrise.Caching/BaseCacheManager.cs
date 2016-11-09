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
        CacheStore _cacheDictionary = new CacheStore();

        public override T GetOrCreateObject<T>(Object cacheName, object parameter, CacheExpirationChecker cacheExpirationChecker, Func<T> createObject)
        {
            return base.GetOrCreateObject<T>(cacheName, dummyParameter, cacheExpirationChecker, createObject);
        }

        protected override CacheStore GetCacheDictionary(object parameter)
        {
            return _cacheDictionary;
        }

        public override IEnumerable<CachedObject> GetAllCachedObjects()
        {
            return _cacheDictionary.Values;
        }

        public virtual T GetOrCreateObject<T>(Object cacheName, CacheExpirationChecker cacheExpirationChecker, Func<T> createObject)
        {
            return base.GetOrCreateObject(cacheName, dummyParameter, cacheExpirationChecker, createObject);
        }

        public virtual T GetOrCreateObject<T>(Object cacheName, Func<T> createObject)
        {
            return this.GetOrCreateObject(cacheName, null, createObject);
        }

        public void RemoveObjectFromCache(Object cacheName)
        {
            base.RemoveObjectFromCache(cacheName, dummyParameter);
        }

        public override void RemoveObjectFromCache(CachedObject cachedObject)
        {
            _cacheDictionary.TryRemove(cachedObject.CacheName);
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

    //public abstract class BaseTenantCacheManager : BaseCacheManager<int>
    //{
    //    private int GetCurrentTenantId()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    #region Public Methods

    //    public virtual T GetOrCreateObjectInCurrentTenant<T>(string cacheName, Func<T> createObject)
    //    {
    //        return base.GetOrCreateObject(cacheName, GetCurrentTenantId(), createObject);
    //    }

    //    public virtual void RemoveObjectFromCacheInCurrentTenant(string cacheName)
    //    {
    //        base.RemoveObjectFromCache(cacheName, GetCurrentTenantId());
    //    }

    //    public virtual bool IsCacheExpiredInCurrentTenant(ref DateTime? lastCheckTime)
    //    {
    //        return base.IsCacheExpired(GetCurrentTenantId(), ref lastCheckTime);
    //    }

    //    public virtual void SetCacheExpiredInCurrentTenant()
    //    {
    //        base.SetCacheExpired(GetCurrentTenantId());
    //    }

    //    #endregion
    //}

    //public struct TenantParamType <T>
    //{
    //    public int TenantId { get; set; }

    //    public T Param { get; set; }
    //}

    //public abstract class BaseTenantCacheManager<T> : BaseCacheManager<TenantParamType<T>>
    //{
    //    TenantParamType<T> GetTenantParam(int tenantId, T prm)
    //    {
    //        return new TenantParamType<T>
    //        {
    //            TenantId = tenantId,
    //            Param = prm
    //        };
    //    }

    //    private int GetCurrentTenantId()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    #region Public Methods

    //    public virtual T GetOrCreateObject(string cacheName, int tenantId, T parameter, Func<T> createObject)
    //    {
    //        return base.GetOrCreateObject(cacheName, GetTenantParam(tenantId, parameter), createObject);
    //    }

    //    public virtual T GetOrCreateObjectInCurrentTenant(string cacheName, T parameter, Func<T> createObject)
    //    {
    //        return this.GetOrCreateObject(cacheName, GetCurrentTenantId(), parameter, createObject);
    //    }

    //    public virtual void RemoveObjectFromCache(string cacheName, int tenantId, T parameter)
    //    {
    //        base.RemoveObjectFromCache(cacheName, GetTenantParam(tenantId, parameter));
    //    }

    //    public virtual void RemoveObjectFromCacheInCurrentTenant(string cacheName, T parameter)
    //    {
    //        this.RemoveObjectFromCache(cacheName, GetCurrentTenantId(), parameter);
    //    }

    //    public virtual bool IsCacheExpired(int tenantId, T parameter, ref DateTime? lastCheckTime)
    //    {
    //        return base.IsCacheExpired(GetTenantParam(tenantId, parameter), ref lastCheckTime);
    //    }

    //    public virtual bool IsCacheExpiredInCurrentTenant(T parameter, ref DateTime? lastCheckTime)
    //    {
    //        return this.IsCacheExpired(GetCurrentTenantId(), parameter, ref lastCheckTime);
    //    }

    //    public virtual void SetCacheExpired(int tenantId, T parameter)
    //    {
    //        base.SetCacheExpired(GetTenantParam(tenantId, parameter));
    //    }

    //    public virtual void SetCacheExpiredInCurrentTenant(T parameter)
    //    {
    //        this.SetCacheExpired(GetCurrentTenantId(), parameter);
    //    }


    //    #endregion

    //    #region Overridable

    //    protected virtual bool TryGetObjectFromCache(string cacheName, int tenantId, T parameter, out CachedObject cachedObject)
    //    {
    //        return base.TryGetObjectFromCache(cacheName, GetTenantParam(tenantId, parameter), out cachedObject);
    //    }

    //    protected virtual void AddObjectToCache(string cacheName, int tenantId, T parameter, CachedObject cachedObject)
    //    {
    //        base.AddObjectToCache(cacheName, GetTenantParam(tenantId, parameter), cachedObject);
    //    }

    //    protected virtual ConcurrentDictionary<string, CachedObject> GetCacheDictionary(int tenantId, T parameter)
    //    {
    //        return base.GetCacheDictionary(GetTenantParam(tenantId, parameter));
    //    }

    //    protected virtual object GetCacheNameLockObject(string cacheName, int tenantId, T parameter)
    //    {
    //        return base.GetCacheNameLockObject(cacheName, GetTenantParam(tenantId, parameter));
    //    }

    //    protected virtual bool ShouldSetCacheExpired(int tenantId, T parameter)
    //    {
    //        return base.ShouldSetCacheExpired(GetTenantParam(tenantId, parameter));
    //    }

    //    #endregion
    //}
}
