using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleRateManager : ISaleEntityZoneRateManager
    {
        #region Public Methods
        public List<SaleRate> GetRates(DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            throw new NotImplementedException();
        }

        public List<SaleRate> GetSaleRatesEffectiveAfter(int sellingNumberPlanId, DateTime minimumDate)
        {
            ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
            return dataManager.GetSaleRatesEffectiveAfter(sellingNumberPlanId, minimumDate);
        }

        public IEnumerable<SaleRate> GetSaleRatesEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate)
        {
            ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
            return dataManager.GetSaleRatesEffectiveAfter(ownerType, ownerId, minimumDate);
        }

        public Vanrise.Entities.IDataRetrievalResult<SaleRateDetail> GetFilteredSaleRates(Vanrise.Entities.DataRetrievalInput<SaleRateQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new SaleRateRequestHandler());
        }
        public RateChangeType GetSaleRateChange(DateTime fromTime, DateTime tillTime, long rateId)
        {
            Dictionary<long, SaleRate> saleRates = GetCachedSaleRatesInBetweenPeriod(fromTime, tillTime);
            SaleRate rate;
            if (saleRates != null)
            {
                saleRates.TryGetValue(rateId, out rate);
                if (rate != null) return rate.RateChange;
            }
            else saleRates = new Dictionary<long, SaleRate>();
            rate = GetSaleRateByRateId(rateId);
            if (rate != null)
            {
                saleRates[rate.SaleRateId] = rate;
                return rate.RateChange;
            }
            throw new ArgumentNullException(String.Format("SaleRate: {0}", rateId));
        }
        private SaleRate GetSaleRateByRateId(long rateId)
        {
            ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
            return dataManager.GetSaleRateById(rateId);
        }
        public Dictionary<long, SaleRate> GetCachedSaleRatesInBetweenPeriod(DateTime fromTime, DateTime tillTime)
        {
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleRateCacheManager>();
            return cacheManager.GetOrCreateObject("GetSaleRatesInBetweenPeriod",
               () =>
               {
                   ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
                   IEnumerable<SaleRate> saleRates = dataManager.GetSaleRatesInBetweenPeriod(fromTime, tillTime);
                   return saleRates.ToDictionary(cn => cn.SaleRateId, cn => cacheManager.CacheAndGetRate(cn));
               });
        }

        public SaleEntityZoneRate GetCachedCustomerZoneRate(int customerId, long saleZoneId, DateTime effectiveOn)
        {
            CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();
            CustomerSellingProduct customerSellingProduct = customerSellingProductManager.GetEffectiveSellingProduct(customerId, effectiveOn, false);
            if (customerSellingProduct == null)
                return null;
            SaleEntityZoneRateLocator customerZoneRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadWithCache(effectiveOn));
            return customerZoneRateLocator.GetCustomerZoneRate(customerId, customerSellingProduct.SellingProductId, saleZoneId);
        }

        public IEnumerable<SaleRate> GetExistingRatesByZoneIds(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<long> zoneIds, DateTime minEED)
        {
            ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
            return dataManager.GetExistingRatesByZoneIds(ownerType, ownerId, zoneIds, minEED);
        }

        public int GetSaleRateTypeId()
        {
            return Vanrise.Common.Business.TypeManager.Instance.GetTypeId(this.GetSaleRateType());
        }

        public Type GetSaleRateType()
        {
            return this.GetType();
        }

        public long ReserveIdRange(int numberOfIds)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(GetSaleRateType(), numberOfIds, out startingId);
            return startingId;
        }

        public int GetCurrencyId(SaleRate saleRate)
        {
            if (saleRate == null)
                throw new ArgumentNullException("saleRate");
            if (saleRate.CurrencyId.HasValue)
                return saleRate.CurrencyId.Value;
            var salePriceListManager = new SalePriceListManager();
            SalePriceList salePriceList = salePriceListManager.GetPriceList(saleRate.PriceListId);
            if (salePriceList == null)
                throw new NullReferenceException(String.Format("salePriceList (Id: {0}) does not exist for saleRate (Id: {1})", saleRate.SaleRateId, salePriceList.PriceListId));
            return salePriceList.CurrencyId;
        }

        #endregion

        #region Private Classes

        private class SaleRateRequestHandler : BigDataRequestHandler<SaleRateQuery, SaleRate, SaleRateDetail>
        {
            #region Fields

            private SaleZoneManager _saleZoneManager = new SaleZoneManager();
            private CurrencyManager _currencyManager = new CurrencyManager();
            private Vanrise.Common.Business.RateTypeManager _rateTypeManager = new Vanrise.Common.Business.RateTypeManager();
            private CurrencyExchangeRateManager _currencyExchangeRateManager = new CurrencyExchangeRateManager();
            private SaleRateManager _saleRateManager = new SaleRateManager();

            #endregion

            public override IEnumerable<SaleRate> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SaleRateQuery> input)
            {
                ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
                return dataManager.GetFilteredSaleRates(input.Query);
            }

            protected override BigResult<SaleRateDetail> AllRecordsToBigResult(DataRetrievalInput<SaleRateQuery> input, IEnumerable<SaleRate> allRecords)
            {
                DateTime? rateConversionEffectiveDate = null;
                if (input.Query.CurrencyId.HasValue)
                    rateConversionEffectiveDate = (input.Query.EffectiveOn.HasValue) ? input.Query.EffectiveOn.Value : DateTime.Now;
                return allRecords.ToBigResult(input, null, x => ConvertRateToCurrency(EntityDetailMapper(x), input.Query.CurrencyId, rateConversionEffectiveDate));
            }

            public override SaleRateDetail EntityDetailMapper(SaleRate entity)
            {
                SaleRateDetail saleRateDetail = new SaleRateDetail();

                saleRateDetail.Entity = entity;
                saleRateDetail.ZoneName = _saleZoneManager.GetSaleZoneName(entity.ZoneId);

                if (entity.RateTypeId.HasValue)
                    saleRateDetail.RateTypeName = _rateTypeManager.GetRateTypeName(entity.RateTypeId.Value);

                return saleRateDetail;
            }

            private SaleRateDetail ConvertRateToCurrency(SaleRateDetail saleRateDetail, int? targetCurrencyId, DateTime? rateConversionEffectiveDate)
            {
                int currencyId = _saleRateManager.GetCurrencyId(saleRateDetail.Entity);

                if (targetCurrencyId.HasValue) // If true, then rateConversionEffectiveDate != null
                {
                    saleRateDetail.ConvertedRate = _currencyExchangeRateManager.ConvertValueToCurrency(saleRateDetail.Entity.NormalRate, currencyId, targetCurrencyId.Value, rateConversionEffectiveDate.Value);
                    saleRateDetail.CurrencyName = _currencyManager.GetCurrencySymbol(targetCurrencyId.Value);
                }
                else
                {
                    saleRateDetail.ConvertedRate = saleRateDetail.Entity.NormalRate;
                    saleRateDetail.CurrencyName = _currencyManager.GetCurrencySymbol(currencyId);
                }

                return saleRateDetail;
            }
        }

        #endregion
    }
}
