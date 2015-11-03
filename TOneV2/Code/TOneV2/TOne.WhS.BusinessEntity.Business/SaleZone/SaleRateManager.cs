using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleRateManager
    {
        public List<SaleRate> GetRates(DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            throw new NotImplementedException();
        }

        public List<SaleRate> GetSaleRatesByCustomerZoneIds(SalePriceListOwnerType ownerType, int customerId, List<long> customerZoneIds, DateTime? effectiveOn)
        {
            ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
            return dataManager.GetSaleRatesByCustomerZoneIds(ownerType, customerId, customerZoneIds, effectiveOn);
        }

        public List<SaleRate> GetSaleRates(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn)
        {
            throw new NotImplementedException();
        }

        public CallSale GetCallSale(int customerId, long saleZoneId, int durationInSeconds, DateTime effectiveOn)
        {
            CallSale callSale = null;
            CustomerZoneRates customerZoneRates = GetCustomerZoneRates(customerId, effectiveOn);
            if(customerZoneRates != null && customerZoneRates.RatesByZone != null)
            {
                SaleRate saleRate;
                if(customerZoneRates.RatesByZone.TryGetValue(saleZoneId, out saleRate))
                {
                    int currencyId;
                    if (saleRate.CurrencyId.HasValue)
                        currencyId = saleRate.CurrencyId.Value;
                    else
                    {
                        SalePriceListManager salePriceListManager = new SalePriceListManager();
                        SalePriceList salePriceList = salePriceListManager.GetPriceList(saleRate.PriceListId);
                        if (salePriceList == null)
                            throw new Exception(String.Format("SalePriceList ID '{0}' not found", saleRate.PriceListId));
                        currencyId = salePriceList.CurrencyId;
                    }
                    SalePricingRuleManager salePricingRuleManager = new SalePricingRuleManager();
                    SalePricingRulesInput salePricingRulesInput = new SalePricingRulesInput
                    {
                        CustomerId = customerId,
                        SaleZoneId = saleZoneId,
                        SellingProductId = customerZoneRates.SellingProductId,
                        DurationInSeconds = durationInSeconds,
                        Rate = saleRate,
                        EffectiveOn = effectiveOn
                    };
                    var pricingRulesResult = salePricingRuleManager.ApplyPricingRules(salePricingRulesInput);

                    callSale = new CallSale
                    {
                        RateValue = pricingRulesResult != null ? pricingRulesResult.Rate : saleRate.NormalRate,
                        TotalNet = pricingRulesResult != null ? pricingRulesResult.TotalAmount : saleRate.NormalRate * (durationInSeconds / 60),
                        CurrencyId = currencyId
                    };
                }
            }
            return callSale;
        }

        private CustomerZoneRates GetCustomerZoneRates(int customerId, DateTime effectiveOn)
        {
            string cacheName = String.Format("GetCustomerZoneRates_{0}_{1}", customerId, effectiveOn.Date);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleRateCacheManager>().GetOrCreateObject(cacheName,
                () =>
                {
                    CustomerZoneRates customerZoneRates = null;
                    CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                    var customerAccount = carrierAccountManager.GetCarrierAccount(customerId);
                    if (customerAccount == null || customerAccount.CustomerSettings == null)
                        return null;
                    CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();
                    var customerSellingProduct = customerSellingProductManager.GetEffectiveSellingProduct(customerId, effectiveOn, false);
                    if (customerSellingProduct == null)
                        return null;
                   
                    var customerRates = GetSaleRates(SalePriceListOwnerType.Customer, customerId, effectiveOn);
                    Dictionary<long, SaleRate> customerRatesByZone = customerRates != null ? customerRates.ToDictionary(itm => itm.ZoneId, itm => itm) : new Dictionary<long, SaleRate>();
                    var sellingProductRates = GetSaleRates(SalePriceListOwnerType.SellingProduct, customerSellingProduct.SellingProductId, effectiveOn);
                    Dictionary<long, SaleRate> sellingProductRatesByZone = sellingProductRates != null ? sellingProductRates.ToDictionary(itm => itm.ZoneId, itm => itm) : new Dictionary<long, SaleRate>();
                    
                    CustomerZoneManager customerZoneManager = new CustomerZoneManager();
                    var customerSaleZones = customerZoneManager.GetCustomerSaleZones(customerId, effectiveOn, false);
                    if (customerSaleZones == null)
                        return null;

                    customerZoneRates = new CustomerZoneRates
                    {
                        CustomerId = customerId,
                        SellingProductId = customerSellingProduct.SellingProductId,
                        RatesByZone = new Dictionary<long, SaleRate>()
                    };
                    foreach (var customerZone in customerSaleZones)
                    {
                        SaleRate saleRate;
                        if(customerRatesByZone.TryGetValue(customerZone.SaleZoneId, out saleRate) || sellingProductRatesByZone.TryGetValue(customerZone.SaleZoneId, out saleRate))
                        {
                            customerZoneRates.RatesByZone.Add(customerZone.SaleZoneId, saleRate);
                        }
                    }
                    return customerZoneRates;
                });
        }

        private class CustomerZoneRates
        {
            public int CustomerId { get; set; }

            public int SellingProductId { get; set; }

            public Dictionary<long, SaleRate> RatesByZone { get; set; }
        }
    }
}
