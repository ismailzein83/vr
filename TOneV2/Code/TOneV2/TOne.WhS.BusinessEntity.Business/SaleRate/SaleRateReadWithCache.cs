using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleRateReadWithCache : ISaleRateReader
    {
        #region Fields / Constructors

        private DateTime _effectiveOn;
        private SaleRatesByOwner _allSaleRatesByOwner;

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

        private SaleRatesByZone GetCachedSaleRates(Entities.SalePriceListOwnerType ownerType, int ownerId)
        {
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleRateCacheManager>();
            return cacheManager.GetOrCreateObject(String.Format("GetSaleRates_{0}_{1}_{2:MM/dd/yy}", ownerType, ownerId, _effectiveOn.Date), () =>
            {
                SaleRatesByZone saleRatesByZone = new SaleRatesByZone();

                ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
                List<SaleRate> saleRates = dataManager.GetEffectiveSaleRates(ownerType, ownerId, this._effectiveOn);

                foreach (SaleRate saleRate in saleRates)
                {
                    var cachedRate = cacheManager.CacheAndGetRate(saleRate);

                    SaleRatePriceList saleRatePriceList;

                    if (!saleRatesByZone.TryGetValue(cachedRate.ZoneId, out saleRatePriceList))
                    {
                        saleRatePriceList = new SaleRatePriceList();
                        saleRatesByZone.Add(cachedRate.ZoneId, saleRatePriceList);
                    }

                    if (cachedRate.RateTypeId.HasValue)
                    {
                        if (saleRatePriceList.RatesByRateType == null)
                            saleRatePriceList.RatesByRateType = new Dictionary<int, SaleRate>();
                        saleRatePriceList.RatesByRateType.Add(cachedRate.RateTypeId.Value, cachedRate);
                    }
                    else
                        saleRatePriceList.Rate = cachedRate;
                }

                return saleRatesByZone;
            });
        }
        #endregion
    }
}
