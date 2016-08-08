using System;
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

        public Dictionary<long, SaleRate> GetCachedSaleRatesInBetweenPeriod(DateTime fromTime, DateTime tillTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleRateCacheManager>().GetOrCreateObject("GetSaleRatesInBetweenPeriod",
               () =>
               {
                   ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
                   IEnumerable<SaleRate> saleRates = dataManager.GetSaleRatesInBetweenPeriod(fromTime, tillTime);
                   return saleRates.ToDictionary(cn => cn.SaleRateId, cn => cn);
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

        public CallSale GetCallSale(int customerId, long saleZoneId, int durationInSeconds, DateTime effectiveOn)
        {
            CallSale callSale = null;

            CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();
            CustomerSellingProduct customerSellingProduct = customerSellingProductManager.GetEffectiveSellingProduct(customerId, effectiveOn, false);
            if (customerSellingProduct == null)
                return null;
            SaleEntityZoneRateLocator customerZoneRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadWithCache(effectiveOn));
            var customerZoneRate = customerZoneRateLocator.GetCustomerZoneRate(customerId, customerSellingProduct.SellingProductId, saleZoneId);

            if (customerZoneRate != null)
            {
                int currencyId = GetCurrencyId(customerZoneRate.Rate);

                SalePricingRuleManager salePricingRuleManager = new SalePricingRuleManager();
                SalePricingRulesInput salePricingRulesInput = new SalePricingRulesInput
                {
                    CustomerId = customerId,
                    SaleZoneId = saleZoneId,
                    SellingProductId = customerSellingProduct.SellingProductId,
                    DurationInSeconds = durationInSeconds,
                    Rate = customerZoneRate.Rate,
                    EffectiveOn = effectiveOn
                };
                var pricingRulesResult = salePricingRuleManager.ApplyPricingRules(salePricingRulesInput);

                callSale = new CallSale
                {
                    RateValue = pricingRulesResult.Rate,
                    TotalNet = pricingRulesResult.TotalAmount,
                    CurrencyId = currencyId,
                    EffectiveDurationInSeconds = pricingRulesResult.EffectiveDurationInSeconds,
                    ExtraChargeValue = pricingRulesResult.ExtraChargeValue,
                    RateType = pricingRulesResult.RateType,

                };
            }
            return callSale;
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

        #region Private Mappers

        private SaleRateDetail SaleRateDetailMapper(SaleRate saleRate)
        {
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            CurrencyManager currencyManager = new CurrencyManager();
            var rateTypeManager = new Vanrise.Business.RateTypeManager();

            int currencyId = GetCurrencyId(saleRate);

            SaleRateDetail saleRateDetail = new SaleRateDetail();
            saleRateDetail.Entity = saleRate;
            saleRateDetail.ZoneName = saleZoneManager.GetSaleZoneName(saleRate.ZoneId);
            saleRateDetail.CurrencyName = currencyManager.GetCurrencySymbol(currencyId);
            if (saleRate.RateTypeId.HasValue)
                saleRateDetail.RateTypeName = rateTypeManager.GetRateTypeName(saleRate.RateTypeId.Value);
            return saleRateDetail;
        }

        #endregion

        #region Private Classes

        private class SaleRateRequestHandler : BigDataRequestHandler<SaleRateQuery, SaleRate, SaleRateDetail>
        {
            public override SaleRateDetail EntityDetailMapper(SaleRate entity)
            {
                SaleRateManager manager = new SaleRateManager();
                return manager.SaleRateDetailMapper(entity);
            }

            public override IEnumerable<SaleRate> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SaleRateQuery> input)
            {
                ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
                return dataManager.GetFilteredSaleRates(input.Query);
            }
        }

        #endregion
    }
}
