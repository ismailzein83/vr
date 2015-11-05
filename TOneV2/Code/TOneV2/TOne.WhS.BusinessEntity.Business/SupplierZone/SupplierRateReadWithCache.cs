using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierRateReadWithCache : ISupplierRateReader
    {
        private DateTime effectiveOn { get; set; }
        public SupplierRateReadWithCache(DateTime effectiveOn)
        {
            this.effectiveOn = effectiveOn;
        }

        public SupplierRatesByZone GetSupplierRates(int supplierId)
        {
            SupplierRatesByZone supplierRatesByZone = new SupplierRatesByZone();
            List<SupplierRate> supplierRates = GetCachedSupplierRates(supplierId);
            foreach (SupplierRate supplierRate in supplierRates)
            {  
                if(!supplierRatesByZone.ContainsKey(supplierRate.ZoneId))
                {
                    supplierRatesByZone.Add(supplierRate.ZoneId, supplierRate);
                }
            }
            return supplierRatesByZone;
        }
        List<SupplierRate> GetCachedSupplierRates(int supplierId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<SupplierRateCacheManager>().GetOrCreateObject(String.Format("GetSupplierRates_{0}_{1:MM/dd/yy H:mm:ss}", supplierId, effectiveOn),
               () =>
               {
                   ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
                   return dataManager.GetEffectiveSupplierRates(supplierId, this.effectiveOn);
               });
        }
    }
}
