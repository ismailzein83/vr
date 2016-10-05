using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Caching.Runtime;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    //public class SupplierRateReadWithCache : ISupplierRateReader
    //{
    //    #region ctor/Local Variables
    //    private DateTime _effectiveOn { get; set; }
    //    #endregion

    //    #region Public Methods
    //    public SupplierRateReadWithCache(DateTime effectiveOn)
    //    {
    //        _effectiveOn = effectiveOn;
    //    }
    //    public SupplierRatesByZone GetSupplierRates(int supplierId)
    //    {
    //        return GetCachedSupplierRates(supplierId);
    //    }
    //    #endregion

    //    #region Private Members
    //    SupplierRatesByZone GetCachedSupplierRates(int supplierId)
    //    {
    //        var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SupplierRateCacheManager>();
    //        return cacheManager.GetOrCreateObject(String.Format("GetSupplierRates_{0}_{1:MM/dd/yy}", supplierId, _effectiveOn.Date),
    //           () =>
    //           {
    //               SupplierRatesByZone supplierRatesByZone = new SupplierRatesByZone();

    //               DistributedCacher cacher = new DistributedCacher();
    //               Func<SupplierRateCachedObjectCreationHandler> objectCreationHandler = () => { return new SupplierRateCachedObjectCreationHandler(supplierId, this._effectiveOn); };
    //               List<SupplierRate> supplierRates = cacher.GetOrCreateObject<SupplierRateCacheManager, List<SupplierRate>>(String.Format("Distributed_GetSupplierRates_{0}_{1:MM/dd/yy}", supplierId, _effectiveOn.Date), objectCreationHandler);

    //               if (supplierRates == null)
    //                   return null;

    //               SupplierZoneRate supplierZoneRate;

    //               foreach (SupplierRate supplierRate in supplierRates)
    //               {
    //                   var cachedRate = cacheManager.CacheAndGetRate(supplierRate);

    //                   if (!supplierRatesByZone.TryGetValue(cachedRate.ZoneId, out supplierZoneRate))
    //                   {
    //                       supplierZoneRate = new SupplierZoneRate();
    //                       supplierRatesByZone.Add(cachedRate.ZoneId, supplierZoneRate);
    //                   }

    //                   if (cachedRate.RateTypeId.HasValue)
    //                   {
    //                       if (supplierZoneRate.RatesByRateType == null)
    //                           supplierZoneRate.RatesByRateType = new Dictionary<int, SupplierRate>();
    //                       supplierZoneRate.RatesByRateType.Add(cachedRate.RateTypeId.Value, cachedRate);
    //                   }
    //                   else
    //                       supplierZoneRate.Rate = cachedRate;
    //               }
    //               return supplierRatesByZone;
    //           });
    //    }
    //    #endregion
    //}

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

        private struct GetCachedSupplierRatesCacheName
        {
            public DateTime EffectiveOn { get; set; }
        }

        SupplierRatesByZone GetCachedSupplierRates(int supplierId)
        {
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SupplierRateCacheManager>();
            var cacheName = new GetCachedSupplierRatesCacheName
            {
                EffectiveOn = _effectiveOn.Date
            };
            var ratesBySuppliers = cacheManager.GetOrCreateObject(cacheName,
               () =>
               {
                   ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
                   List<SupplierRate> supplierRates = dataManager.GetEffectiveSupplierRates(this._effectiveOn);

                   Dictionary<int, SupplierRatesByZone> result = new Dictionary<int, SupplierRatesByZone>();
                   var priceLists = new SupplierPriceListManager().GetCachedPriceLists();
                   foreach (SupplierRate supplierRate in supplierRates)
                   {
                       var cachedRate = cacheManager.CacheAndGetRate(supplierRate);
                       SupplierPriceList supplierPriceList = priceLists.GetRecord(cachedRate.PriceListId);
                       if (supplierPriceList == null)
                           throw new NullReferenceException(String.Format("supplierPriceList '{0}'", cachedRate.PriceListId));

                       var supplierRatesByZone = result.GetOrCreateItem(supplierPriceList.SupplierId);
                       var supplierZoneRates = supplierRatesByZone.GetOrCreateItem(cachedRate.ZoneId);

                       if (cachedRate.RateTypeId.HasValue)
                       {
                           if (supplierZoneRates.RatesByRateType == null)
                               supplierZoneRates.RatesByRateType = new Dictionary<int, SupplierRate>();
                           supplierZoneRates.RatesByRateType.Add(cachedRate.RateTypeId.Value, cachedRate);
                       }
                       else
                           supplierZoneRates.Rate = cachedRate;
                   }
                   return result;
               });
            return ratesBySuppliers.GetRecord(supplierId);

        }
        #endregion
    }
}