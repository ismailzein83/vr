using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Entities;

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
        public SupplierRatesByZone GetSupplierRates(int supplierId, DateTime effectiveOn)
        {
            List<SupplierRatesByZoneInfo> supplierRatesByZoneInfoList = GetCachedSupplierRates(supplierId);
            SupplierRatesByZoneInfo SupplierRatesByZoneInfo = Helper.GetBusinessEntityInfo<SupplierRatesByZoneInfo>(supplierRatesByZoneInfoList, effectiveOn);

            if (SupplierRatesByZoneInfo == null)
                return null;

            return SupplierRatesByZoneInfo.SupplierRatesByZone;
        }
        #endregion

        #region Private Members

        private struct GetCachedSupplierRatesCacheName : IBEDayFilterCacheName
        {
            public DateTime EffectiveOn { get; set; }

            public DateTime FilterDay
            {
                get { return this.EffectiveOn; }
            }
        }

        List<SupplierRatesByZoneInfo> GetCachedSupplierRates(int supplierId)
        {
            DateTimeRange dateTimeRange = Helper.GetDateTimeRangeWithOffset(_effectiveOn);

            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SupplierRateCacheManager>();
            var cacheName = new GetCachedSupplierRatesCacheName
            {
                EffectiveOn = dateTimeRange.From
            };
            var ratesBySuppliers = cacheManager.GetOrCreateObject(cacheName,
               () =>
               {
                   ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
                   List<SupplierRate> supplierRates = dataManager.GetEffectiveSupplierRates(dateTimeRange.From, dateTimeRange.To);

                   Dictionary<int, List<SupplierRatesByZoneInfo>> result = new Dictionary<int, List<SupplierRatesByZoneInfo>>();

                   var priceLists = new SupplierPriceListManager().GetCachedPriceLists();

                   Helper.StructureBusinessEntitiesByDate<SupplierRate>(supplierRates, dateTimeRange.From, dateTimeRange.To, (matchingSupplierRates, bed, eed) =>
                   {
                       Dictionary<int, SupplierRatesByZone> data = new Dictionary<int, SupplierRatesByZone>();

                       foreach (SupplierRate supplierRate in matchingSupplierRates)
                       {
                           var cachedRate = cacheManager.CacheAndGetRate(supplierRate);
                           SupplierPriceList supplierPriceList = priceLists.GetRecord(cachedRate.PriceListId);
                           if (supplierPriceList == null)
                               throw new NullReferenceException(String.Format("supplierPriceList '{0}'", cachedRate.PriceListId));

                           var supplierRatesByZone = data.GetOrCreateItem(supplierPriceList.SupplierId);
                           var supplierZoneRates = supplierRatesByZone.GetOrCreateItem(cachedRate.ZoneId);

                           if (cachedRate.RateTypeId.HasValue)
                           {
                               if (supplierZoneRates.RatesByRateType == null)
                                   supplierZoneRates.RatesByRateType = new Dictionary<int, SupplierRate>();

                               if (!supplierZoneRates.RatesByRateType.ContainsKey(cachedRate.RateTypeId.Value))
                                   supplierZoneRates.RatesByRateType.Add(cachedRate.RateTypeId.Value, cachedRate);
                           }
                           else
                               supplierZoneRates.Rate = cachedRate;
                       }

                       foreach (KeyValuePair<int, SupplierRatesByZone> item in data)
                       {
                           var matchingItem = result.GetOrCreateItem(item.Key);
                           matchingItem.Add(new SupplierRatesByZoneInfo() { BED = bed, EED = eed, SupplierRatesByZone = item.Value });
                       }
                   });
                   return result;
               });
            return ratesBySuppliers.GetRecord(supplierId);
        }

        #endregion

        private class SupplierRatesByZoneInfo : IBusinessEntityInfo
        {
            public DateTime BED { get; set; }

            public DateTime? EED { get; set; }

            public SupplierRatesByZone SupplierRatesByZone { get; set; }
        }
    }
}