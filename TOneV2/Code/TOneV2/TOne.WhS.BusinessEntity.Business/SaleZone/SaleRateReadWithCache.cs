using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

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
            return GetCachedSaleRates(ownerType, ownerId);
        }
        SaleRatesByZone GetCachedSaleRates(Entities.SalePriceListOwnerType ownerType, int ownerId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleRateCacheManager>().GetOrCreateObject(String.Format("GetSaleRates_{0}_{1}_{2:MM/dd/yy}", ownerType, ownerId, effectiveOn.Date),
               () =>
               {
                   ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
                   List<SaleRate> saleRates = dataManager.GetEffectiveSaleRates(ownerType, ownerId, this.effectiveOn);
                   SaleRatesByZone saleRatesByZone = new SaleRatesByZone();
                   if (saleRates != null)
                   {
                       IEnumerable<SaleRate> normalRates = saleRates.FindAllRecords(x => !x.RateTypeId.HasValue);

                       foreach (SaleRate normalRate in normalRates)
                       {
                           var saleRatePriceList = new SaleRatePriceList() { Rate = normalRate };

                           IEnumerable<SaleRate> otherRates = saleRates.FindAllRecords(x => x.RateTypeId.HasValue && x.ZoneId == normalRate.ZoneId);
                           saleRatePriceList.RatesByRateType = otherRates.ToDictionary(x => x.RateTypeId.Value);
                           
                           saleRatesByZone.Add(normalRate.ZoneId, saleRatePriceList);
                       }
                   }
                   return saleRatesByZone;
               });
        }
    }
}
