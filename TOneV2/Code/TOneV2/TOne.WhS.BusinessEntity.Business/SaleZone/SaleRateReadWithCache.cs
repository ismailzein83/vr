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

        private SaleRatesByZone GetCachedSaleRates(Entities.SalePriceListOwnerType ownerType, int ownerId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleRateCacheManager>().GetOrCreateObject(String.Format("GetSaleRates_{0}_{1}_{2:MM/dd/yy}", ownerType, ownerId, _effectiveOn.Date), () =>
            {
                ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
                List<SaleRate> saleRates = dataManager.GetEffectiveSaleRates(ownerType, ownerId, this._effectiveOn);

                SaleRatesByZone saleRatesByZone = new SaleRatesByZone();

                foreach (SaleRate saleRate in saleRates)
                {
                    SaleRatePriceList saleRatePriceList;

                    if (!saleRatesByZone.TryGetValue(saleRate.ZoneId, out saleRatePriceList))
                    {
                        saleRatePriceList = new SaleRatePriceList();
                        saleRatePriceList.RatesByRateType = new Dictionary<int, SaleRate>();
                        saleRatesByZone.Add(saleRate.ZoneId, saleRatePriceList);
                    }

                    if (saleRate.RateTypeId.HasValue)
                        saleRatePriceList.RatesByRateType.Add(saleRate.RateTypeId.Value, saleRate);
                    else
                        saleRatePriceList.Rate = saleRate;
                }

                return saleRatesByZone;
            });
        }

        #endregion
    }
}
