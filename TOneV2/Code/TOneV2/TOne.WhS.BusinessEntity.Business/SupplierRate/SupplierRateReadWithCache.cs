using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Caching.Runtime;

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
            return GetCachedSupplierRates(supplierId);
        }
        #endregion

        #region Private Members
        SupplierRatesByZone GetCachedSupplierRates(int supplierId)
        {
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SupplierRateCacheManager>();
            return cacheManager.GetOrCreateObject(String.Format("GetSupplierRates_{0}_{1:MM/dd/yy}", supplierId, _effectiveOn.Date),
               () =>
               {
                   SupplierRatesByZone supplierRatesByZone = new SupplierRatesByZone();

                   DistributedCacher cacher = new DistributedCacher();
                   Func<SupplierRateCachedObjectCreationHandler> objectCreationHandler = () => { return new SupplierRateCachedObjectCreationHandler(supplierId, this._effectiveOn); };
                   List<SupplierRate> supplierRates = cacher.GetOrCreateObject<SupplierRateCacheManager, List<SupplierRate>>(String.Format("Distributed_GetSupplierRates_{0}_{1:MM/dd/yy}", supplierId, _effectiveOn.Date), objectCreationHandler);

                   if (supplierRates == null)
                       return null;

                   SupplierZoneRate supplierZoneRate;

                   foreach (SupplierRate supplierRate in supplierRates)
                   {
                       var cachedRate = cacheManager.CacheAndGetRate(supplierRate);

                       if (!supplierRatesByZone.TryGetValue(cachedRate.ZoneId, out supplierZoneRate))
                       {
                           supplierZoneRate = new SupplierZoneRate();
                           supplierRatesByZone.Add(cachedRate.ZoneId, supplierZoneRate);
                       }

                       if (cachedRate.RateTypeId.HasValue)
                       {
                           if (supplierZoneRate.RatesByRateType == null)
                               supplierZoneRate.RatesByRateType = new Dictionary<int, SupplierRate>();
                           supplierZoneRate.RatesByRateType.Add(cachedRate.RateTypeId.Value, cachedRate);
                       }
                       else
                           supplierZoneRate.Rate = cachedRate;
                   }
                   return supplierRatesByZone;
               });
        }
        #endregion
    }
}