using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierRateReadWithCache : ISupplierRateReader
    {
        #region ctor/Local Variables
        private DateTime _effectiveOn { get; set; }
        #endregion

        #region Public Methods
        public SupplierRateReadWithCache(DateTime effectiveOn)
        {
            _effectiveOn = effectiveOn;
        }
        public SupplierRatesByZone GetSupplierRates(int supplierId)
        {
            SupplierRatesByZone supplierRatesByZone = new SupplierRatesByZone();
            return GetCachedSupplierRates(supplierId);
        }
        #endregion

        #region Private Members
        SupplierRatesByZone GetCachedSupplierRates(int supplierId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<SupplierRateCacheManager>().GetOrCreateObject(String.Format("GetSupplierRates_{0}_{1:MM/dd/yy}", supplierId, _effectiveOn.Date),
               () =>
               {
                   ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();

                   List<SupplierRate> supplierRates = dataManager.GetEffectiveSupplierRates(supplierId, _effectiveOn);
                   SupplierRatesByZone supplierRatesByZone = new SupplierRatesByZone();
                   SupplierZoneRate supplierZoneRate;
                   foreach (SupplierRate supplierRate in supplierRates)
                   {
                       if (supplierRatesByZone.TryGetValue(supplierRate.ZoneId, out supplierZoneRate))
                       {
                           if (supplierRate.RateTypeId.HasValue)
                           {
                               if (supplierZoneRate.RatesByRateType == null)
                                   supplierZoneRate.RatesByRateType = new Dictionary<int, SupplierRate>();
                               supplierZoneRate.RatesByRateType.Add(supplierRate.RateTypeId.Value, supplierRate);
                           }
                           else
                               supplierZoneRate.Rate = supplierRate;
                       }
                       else
                       {
                           supplierZoneRate = new SupplierZoneRate();
                           if (supplierRate.RateTypeId.HasValue)
                           {
                               supplierZoneRate.RatesByRateType = new Dictionary<int, SupplierRate>();
                               supplierZoneRate.RatesByRateType.Add(supplierRate.RateTypeId.Value, supplierRate);
                           }
                           else
                               supplierZoneRate.Rate = supplierRate;
                           supplierRatesByZone.Add(supplierRate.ZoneId, supplierZoneRate);
                       }
                   }
                   return supplierRatesByZone;
               });
        }
        #endregion

    }
}
