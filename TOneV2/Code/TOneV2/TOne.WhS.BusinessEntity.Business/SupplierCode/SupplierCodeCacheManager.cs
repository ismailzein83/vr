using System;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierCodeCacheManager : Vanrise.Caching.BaseCacheManager
    {
        protected override bool UseCentralizedCacheRefresher
        {
            get
            {
                return true;
            }
        }

        public override Vanrise.Caching.CacheObjectSize ApproximateObjectSize
        {
            get
            {
                return Vanrise.Caching.CacheObjectSize.ExtraLarge;
            }
        }

        public override T GetOrCreateObject<T>(object cacheName, Func<T> createObject)
        {
            return GetOrCreateObject(cacheName, BECacheExpirationChecker.Instance, createObject);
        }

        protected override bool ShouldSetCacheExpired()
        {
            return base.ShouldSetCacheExpired();
        }

        public SupplierCode CacheAndGetCode(SupplierCode code)
        {
            return code;
            //Dictionary<long, SupplierCode> cachedCodesById = this.GetOrCreateObject("cachedCodesById", () => new Dictionary<long, SupplierCode>());
            //SupplierCode matchCode;
            //lock (cachedCodesById)
            //{
            //    matchCode = cachedCodesById.GetOrCreateItem(code.SupplierCodeId, () => code);
            //}
            //return matchCode;
        }
    }
}
