using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierRateManager
    {

        public Vanrise.Entities.IDataRetrievalResult<SupplierRateDetail> GetFilteredSupplierRates(Vanrise.Entities.DataRetrievalInput<SupplierRateQuery> input)
        {
            ISupplierRateDataManager manager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, manager.GetFilteredSupplierRates(input));
        }

        public List<SupplierRate> GetSupplierRatesEffectiveAfter(int supplierId, DateTime minimumDate)
        {
            ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
            return dataManager.GetSupplierRates(supplierId, minimumDate);
        }

        public List<SupplierRate> GetRates(DateTime? effectiveOn, bool isEffectiveInFuture, IEnumerable<RoutingSupplierInfo> supplierInfos)
        {
            ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
            return dataManager.GetEffectiveSupplierRatesBySuppliers(effectiveOn, isEffectiveInFuture, supplierInfos);
        }

        public CallCost GetCallCost(int supplierId, long supplierZoneId, int durationInSeconds, DateTime effectiveOn)
        {
            CallCost callSale = null;
            SupplierZoneRateLocator supplierZoneRateLocator = new SupplierZoneRateLocator(new SupplierRateReadWithCache(effectiveOn));

            SupplierZoneRate supplierZoneRate = supplierZoneRateLocator.GetSupplierZoneRate(supplierId, supplierZoneId);
            if (supplierZoneRate != null)
            {
                int currencyId = supplierZoneRate.Rate.CurrencyId.HasValue ? supplierZoneRate.Rate.CurrencyId.Value : supplierZoneRate.PriceList.CurrencyId;
                PurchasePricingRuleManager purchasePricingRuleManager = new PurchasePricingRuleManager();
                PurchasePricingRulesInput purchasePricingRulesInput = new PurchasePricingRulesInput
                {
                    SupplierId = supplierId,
                    SupplierZoneId = supplierZoneId,
                    Rate = supplierZoneRate.Rate,
                    DurationInSeconds = durationInSeconds,
                    EffectiveOn = effectiveOn
                };
                var pricingRulesResult = purchasePricingRuleManager.ApplyPricingRules(purchasePricingRulesInput);
                callSale = new CallCost
                {
                    RateValue = pricingRulesResult != null ? pricingRulesResult.Rate : supplierZoneRate.Rate.NormalRate,
                    TotalNet = pricingRulesResult != null ? pricingRulesResult.TotalAmount : supplierZoneRate.Rate.NormalRate * (durationInSeconds / 60),
                    CurrencyId = currencyId
                };
            }
            return callSale;
        }

    }
}
