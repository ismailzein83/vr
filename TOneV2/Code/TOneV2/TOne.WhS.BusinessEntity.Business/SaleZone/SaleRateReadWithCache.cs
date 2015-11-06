using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleRateReadWithCache : ISaleRateReader
    {
        private DateTime effectiveOn { get; set; }
        public SaleRateReadWithCache(DateTime effectiveOn)
        {
            this.effectiveOn = effectiveOn;
        }

        public SaleRatesByZone GetZoneRates(Entities.SalePriceListOwnerType ownerType, int ownerId)
        {
            SaleRatesByZone saleRatesByZone = new SaleRatesByZone();
            List<SaleRate> saleRates = GetCachedSaleRates(ownerType, ownerId);
            foreach (SaleRate saleRate in saleRates)
            {
                if (!saleRatesByZone.ContainsKey(saleRate.ZoneId))
                {
                    saleRatesByZone.Add(saleRate.ZoneId, saleRate);
                }
            }
            return saleRatesByZone;
        }
        List<SaleRate> GetCachedSaleRates(Entities.SalePriceListOwnerType ownerType,int ownerId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleRateCacheManager>().GetOrCreateObject(String.Format("GetSaleRates_{0}_{1:MM/dd/yy}", ownerId, effectiveOn.Date),
               () =>
               {
                   ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
                   return dataManager.GetEffectiveSaleRates(ownerType, ownerId, this.effectiveOn);
               });
        }
    }
}
