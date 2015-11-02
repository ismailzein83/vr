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
       
        public List<SupplierRate> GetSupplierRatesEffectiveAfter(int supplierId,DateTime minimumDate)
        {
            ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
            return dataManager.GetSupplierRates(supplierId,minimumDate);
        }

        public List<SupplierRate> GetRates(DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            throw new NotImplementedException();
        }

        public CallCost GetCallCost(int supplierId, long supplierZoneId, int durationInSeconds, DateTime effectiveOn)
        {
            CallCost callSale = null;
            Dictionary<long, SupplierRate> ratesByZone = GetSupplierZoneRates(supplierId, effectiveOn);
            if(ratesByZone != null)
            {
                SupplierRate supplierRate;
                if(ratesByZone.TryGetValue(supplierZoneId, out supplierRate))
                {
                    int currencyId;
                    if (supplierRate.CurrencyId.HasValue)
                        currencyId = supplierRate.CurrencyId.Value;
                    else
                    {
                        SupplierPriceListManager supplierPriceListManager = new SupplierPriceListManager();
                        SupplierPriceList supplierPriceList = supplierPriceListManager.GetPriceList(supplierRate.PriceListId);
                        if (supplierPriceList == null)
                            throw new Exception(String.Format("SupplierPriceList ID '{0}' not found", supplierRate.PriceListId));
                        currencyId = supplierPriceList.CurrencyId;
                    }
                    PurchasePricingRuleManager purchasePricingRuleManager = new PurchasePricingRuleManager();
                    PurchasePricingRulesInput purchasePricingRulesInput = new PurchasePricingRulesInput
                    {
                        SupplierId = supplierId,
                        SupplierZoneId = supplierZoneId,
                        Rate = supplierRate,
                        DurationInSeconds = durationInSeconds,
                        EffectiveOn = effectiveOn
                    };
                    var pricingRulesResult = purchasePricingRuleManager.ApplyPricingRules(purchasePricingRulesInput);
                    callSale = new CallCost
                    {
                        RateValue = pricingRulesResult != null ? pricingRulesResult.Rate : supplierRate.NormalRate,
                        TotalNet = pricingRulesResult != null ? pricingRulesResult.TotalAmount : supplierRate.NormalRate * (durationInSeconds / 60),
                        CurrencyId = currencyId
                    };
                }
            }
            return callSale;
        }

        private Dictionary<long, SupplierRate> GetSupplierZoneRates(int supplierId, DateTime effectiveOn)
        {
            string cacheName = String.Format("GetSupplierZoneRates_{0}_{1}", supplierId, effectiveOn.Date);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<SupplierRateCacheManager>().GetOrCreateObject(cacheName,
                () =>
                {
                    Dictionary<long, SupplierRate> supplierRates = null;
                    throw new NotImplementedException();
                    return supplierRates;
                });
        }
    }
}
