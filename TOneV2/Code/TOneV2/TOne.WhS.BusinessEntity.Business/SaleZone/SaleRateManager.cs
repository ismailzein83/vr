﻿using System;
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
    public class SaleRateManager
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
                int currencyId = customerZoneRate.Rate.CurrencyId.HasValue ? customerZoneRate.Rate.CurrencyId.Value : customerZoneRate.PriceList.CurrencyId;

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

        #endregion

        #region Private Mappers

        private SaleRateDetail SaleRateDetailMapper(SaleRate saleRate)
        {
            SaleZoneManager sz = new SaleZoneManager();
            CurrencyManager currencyManager = new CurrencyManager();

            int currencyId;
            if (saleRate.CurrencyId.HasValue)
                currencyId = saleRate.CurrencyId.Value;
            else
            {
                SalePriceListManager salePriceListManager = new SalePriceListManager();
                SalePriceList priceList = salePriceListManager.GetPriceList(saleRate.PriceListId);
                currencyId = priceList.CurrencyId;
            }

            SaleRateDetail saleRateDetail = new SaleRateDetail();
            saleRateDetail.Entity = saleRate;
            saleRateDetail.ZoneName = sz.GetSaleZone(saleRate.ZoneId).Name;
            saleRateDetail.CurrencyName = currencyManager.GetCurrencySymbol(currencyId);
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
