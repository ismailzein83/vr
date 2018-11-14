using System;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierRateCacheManager : Vanrise.Caching.BaseCacheManager
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


        protected override bool ShouldSetCacheExpired(object parameter)
        {
            return base.ShouldSetCacheExpired();
        }
        
        public SupplierRate CacheAndGetRate(SupplierRate rate)
        {
            return rate;
            //Dictionary<long, SupplierRate> cachedRatesById = this.GetOrCreateObject("cachedRatesById", () => new Dictionary<long, SupplierRate>());
            //SupplierRate matchRate;
            //lock (cachedRatesById)
            //{
            //    matchRate = cachedRatesById.GetOrCreateItem(rate.SupplierRateId, () => rate);
            //}
            //return matchRate;
        }
    }
}
