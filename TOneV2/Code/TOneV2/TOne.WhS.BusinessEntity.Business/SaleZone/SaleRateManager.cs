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
    public class SaleRateManager
    {

        #region Public Methods
        public List<SaleRate> GetRates(DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            throw new NotImplementedException();
        }
        public List<SaleRate> GetSaleRates(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn)
        {
            throw new NotImplementedException();
        }


        public List<SaleRate> GetSaleRatesEffectiveAfter(int sellingNumberPlanId, DateTime minimumDate)
        {
            ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
            return dataManager.GetSaleRatesEffectiveAfter(sellingNumberPlanId, minimumDate);
        }

        public Vanrise.Entities.IDataRetrievalResult<SaleRateDetail> GetFilteredSaleRates(Vanrise.Entities.DataRetrievalInput<SaleRateQuery> input)
        {
            ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
            Vanrise.Entities.BigResult<SaleRate> saleRates = dataManager.GetSaleRateFilteredFromTemp(input);
            BigResult<SaleRateDetail> customerRouteDetailResult = new BigResult<SaleRateDetail>()
            {
                ResultKey = saleRates.ResultKey,
                TotalCount = saleRates.TotalCount,
                Data = saleRates.Data.MapRecords(SaleRateDetailMapper)
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, customerRouteDetailResult);
        }
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
            saleRateDetail.CurrencyName = currencyManager.GetCurrencyName(currencyId);
            return saleRateDetail;
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
        #endregion

    }
}
