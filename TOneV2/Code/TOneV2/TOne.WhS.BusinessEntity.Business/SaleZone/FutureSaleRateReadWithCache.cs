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
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleRateCacheManager>().GetOrCreateObject(String.Format("GetFutureSaleRates_{0}_{1}", ownerType, ownerId), () =>
            {
                SaleRatesByZone futureRatesByZone = new SaleRatesByZone();

                ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
                IEnumerable<SaleRate> futureRates = dataManager.GetFutureSaleRates(ownerType, ownerId);

                if (futureRates == null)
                    return futureRatesByZone;

                foreach (SaleRate futureRate in futureRates.OrderBy(x => x.BED))
                {
                    SaleRatePriceList saleRatePriceList;

                    if (!futureRatesByZone.TryGetValue(futureRate.ZoneId, out saleRatePriceList))
                    {
                        saleRatePriceList = new SaleRatePriceList();
                        saleRatePriceList.RatesByRateType = new Dictionary<int, SaleRate>();
                        futureRatesByZone.Add(futureRate.ZoneId, saleRatePriceList);
                    }

                    if (futureRate.RateTypeId.HasValue)
                    {
                        if (!saleRatePriceList.RatesByRateType.ContainsKey(futureRate.RateTypeId.Value))
                            saleRatePriceList.RatesByRateType.Add(futureRate.RateTypeId.Value, futureRate);
                    }
                    else if (saleRatePriceList.Rate == null)
                        saleRatePriceList.Rate = futureRate;
                }

                return futureRatesByZone;
            });
        }
    }
}
