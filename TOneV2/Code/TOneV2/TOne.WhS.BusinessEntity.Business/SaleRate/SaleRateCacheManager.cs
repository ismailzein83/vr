using System;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleRateCacheManager : Vanrise.Caching.BaseCacheManager
    {
        public override Vanrise.Caching.CacheObjectSize ApproximateObjectSize
        {
            get
            {
                return Vanrise.Caching.CacheObjectSize.ExtraLarge;
            }
        }
        protected override bool UseCentralizedCacheRefresher
        {
            get
            {
                return true;
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


        public SaleRate CacheAndGetRate(SaleRate rate)
        {
            return rate;
            //Dictionary<long, SaleRate> cachedRatesById = this.GetOrCreateObject("cachedRatesById", () => new Dictionary<long, SaleRate>());
            //SaleRate matchRate;
            //lock (cachedRatesById)
            //{
            //    matchRate = cachedRatesById.GetOrCreateItem(rate.SaleRateId, () => rate);
            //}
            //return matchRate;
        }

    }
}
