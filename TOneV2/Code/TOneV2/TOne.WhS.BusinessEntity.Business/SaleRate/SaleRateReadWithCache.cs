using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Caching.Runtime;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleRateReadWithCache : ISaleRateReader
    {
        #region Fields / Constructors

        private DateTime _effectiveOn;

        public SaleRateReadWithCache(DateTime effectiveOn)
        {
            this._effectiveOn = effectiveOn;
        }

        #endregion

        #region Public Methods

        public SaleRatesByZone GetZoneRates(Entities.SalePriceListOwnerType ownerType, int ownerId)
        {
            return GetCachedSaleRates(ownerType, ownerId);
        }

        #endregion

        #region Private Methods

        private struct GetCachedSaleRatesCacheName
        {
            public DateTime EffectiveOn { get; set; }
        }
        private SaleRatesByZone GetCachedSaleRates(Entities.SalePriceListOwnerType ownerType, int ownerId)
        {
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleRateCacheManager>();
            var cacheName = new GetCachedSaleRatesCacheName
            {
                EffectiveOn = _effectiveOn.Date
            };
            var ratesByOwner = cacheManager.GetOrCreateObject(cacheName, () =>
            {                
                ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
                List<SaleRate> saleRates = dataManager.GetEffectiveSaleRates(_effectiveOn);

                SaleRatesByOwner result = new SaleRatesByOwner
                {
                    SaleRatesByCustomer = new Dictionary<int, SaleRatesByZone>(),
                    SaleRatesByProduct = new Dictionary<int, SaleRatesByZone>()
                };
                var priceLists = new SalePriceListManager().GetCachedSalePriceLists();
                foreach (SaleRate saleRate in saleRates)
                {
                    var cachedRate = cacheManager.CacheAndGetRate(saleRate);
                    SalePriceList priceList = priceLists.GetRecord(cachedRate.PriceListId);
                    if (priceList == null)
                        throw new NullReferenceException(String.Format("priceList '{0}'", cachedRate.PriceListId));
                   var saleRatesByOwner = priceList.OwnerType == SalePriceListOwnerType.Customer ? result.SaleRatesByCustomer : result.SaleRatesByProduct;
                    var saleRateByZone = saleRatesByOwner.GetOrCreateItem(priceList.OwnerId);

                    var zoneRates = saleRateByZone.GetOrCreateItem(cachedRate.ZoneId);

                    if (cachedRate.RateTypeId.HasValue)
                    {
                        if (zoneRates.RatesByRateType == null)
                            zoneRates.RatesByRateType = new Dictionary<int, SaleRate>();
                        zoneRates.RatesByRateType.Add(cachedRate.RateTypeId.Value, cachedRate);
                    }
                    else
                        zoneRates.Rate = cachedRate;
                }

                return result;
            });
            return (ownerType == SalePriceListOwnerType.Customer ? ratesByOwner.SaleRatesByCustomer : ratesByOwner.SaleRatesByProduct).GetRecord(ownerId);
        }
        #endregion
    }
}