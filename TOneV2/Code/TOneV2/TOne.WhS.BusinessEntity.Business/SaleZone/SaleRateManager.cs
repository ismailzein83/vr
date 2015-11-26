using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleRateManager
    {
        public List<SaleRate> GetRates(DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            throw new NotImplementedException();
        }

        public List<SaleRate> GetSaleRates(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn)
        {
            throw new NotImplementedException();
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
        private SaleRateDetail SaleRateDetailMapper(SaleRate saleRate){
            SaleZoneManager sz = new SaleZoneManager();
            SaleRateDetail saleRateDetail = new SaleRateDetail();
            saleRateDetail.Entity = saleRate;
            saleRateDetail.ZoneName = sz.GetSaleZone(saleRate.ZoneId).Name;
            return saleRateDetail;
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
                    RateValue = pricingRulesResult != null ? pricingRulesResult.Rate : customerZoneRate.Rate.NormalRate,
                    TotalNet = pricingRulesResult != null ? pricingRulesResult.TotalAmount : customerZoneRate.Rate.NormalRate * (durationInSeconds / 60),
                    CurrencyId = currencyId
                };
            }
            return callSale;
        }
    }
}
