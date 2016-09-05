using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class FutureSaleRateReadWithCache : ISaleRateReader
    {
        public SaleRatesByZone GetZoneRates(Entities.SalePriceListOwnerType ownerType, int ownerId)
        {
            return GetCachedFutureRates(ownerType, ownerId);
        }

        private SaleRatesByZone GetCachedFutureRates(Entities.SalePriceListOwnerType ownerType, int ownerId)
        {
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleRateCacheManager>();
            return cacheManager.GetOrCreateObject(String.Format("GetFutureSaleRates_{0}_{1}", ownerType, ownerId), () =>
            {
                SaleRatesByZone futureRatesByZone = new SaleRatesByZone();

                ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
                IEnumerable<SaleRate> futureRates = dataManager.GetFutureSaleRates(ownerType, ownerId);

                if (futureRates == null)
                    return futureRatesByZone;

                foreach (SaleRate futureRate in futureRates.OrderBy(x => x.BED))
                {
                    var cachedRate = cacheManager.CacheAndGetRate(futureRate);
                    SaleRatePriceList saleRatePriceList;

                    if (!futureRatesByZone.TryGetValue(cachedRate.ZoneId, out saleRatePriceList))
                    {
                        saleRatePriceList = new SaleRatePriceList();
                        saleRatePriceList.RatesByRateType = new Dictionary<int, SaleRate>();
                        futureRatesByZone.Add(cachedRate.ZoneId, saleRatePriceList);
                    }

                    if (cachedRate.RateTypeId.HasValue)
                    {
                        if (!saleRatePriceList.RatesByRateType.ContainsKey(cachedRate.RateTypeId.Value))
                            saleRatePriceList.RatesByRateType.Add(cachedRate.RateTypeId.Value, cachedRate);
                    }
                    else if (saleRatePriceList.Rate == null)
                        saleRatePriceList.Rate = cachedRate;
                }

                return futureRatesByZone;
            });
        }
    }
}
