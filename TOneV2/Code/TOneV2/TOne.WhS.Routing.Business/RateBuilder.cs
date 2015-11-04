using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public class RateBuilder
    {
        #region Public Methods

        public void BuildCustomerZoneInfo(int customerId, DateTime? effectiveOn, bool isEffectiveInFuture, Action<CustomerZoneInfo> onCustomerZoneInfoAvailable)
        {
            CustomerZoneManager customerZoneManager = new CustomerZoneManager();
            var customerSaleZones = customerZoneManager.GetCustomerSaleZones(customerId, effectiveOn.HasValue ? effectiveOn.Value : DateTime.Now, isEffectiveInFuture);
            if (customerSaleZones == null)
                return;

            CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();
            CustomerSellingProduct customerSellingProduct = customerSellingProductManager.GetEffectiveSellingProduct(customerId, effectiveOn, isEffectiveInFuture);
            if (customerSellingProduct == null)
                return;

            Vanrise.Common.CurrencyExchangeRateManager currencyExchangeRateManager = new Vanrise.Common.CurrencyExchangeRateManager();
            CustomerZoneRoutingProductLocator customerZoneRoutingProductLocator = new CustomerZoneRoutingProductLocator(new SaleEntityRoutingProductReadAllNoCache(effectiveOn, isEffectiveInFuture));
            CustomerZoneRateLocator customerZoneRateLocator = new CustomerZoneRateLocator(new SaleRateReadAllNoCache(effectiveOn, isEffectiveInFuture));

            SalePricingRuleManager salePricingRuleManager = new SalePricingRuleManager();
            foreach (var customerZone in customerSaleZones)
            {
                CustomerZoneRate customerZoneRate = customerZoneRateLocator.GetCustomerZoneRate(customerId, customerSellingProduct.SellingProductId, customerZone.SaleZoneId);
                
                if (customerZoneRate != null)
                {
                    int currencyId = customerZoneRate.Rate.CurrencyId.HasValue ? customerZoneRate.Rate.CurrencyId.Value : customerZoneRate.PriceList.CurrencyId;
                    SalePricingRulesInput salePricingRulesInput = new SalePricingRulesInput
                    {
                        CustomerId = customerId,
                        SaleZoneId = customerZone.SaleZoneId,
                        SellingProductId = customerSellingProduct.SellingProductId,
                        Rate = customerZoneRate.Rate,
                        EffectiveOn = effectiveOn,
                        IsEffectiveInFuture = isEffectiveInFuture
                    };
                    var pricingRulesResult = salePricingRuleManager.ApplyPricingRules(salePricingRulesInput);

                    var rateValue = pricingRulesResult != null ? pricingRulesResult.Rate : customerZoneRate.Rate.NormalRate;
                    rateValue = currencyExchangeRateManager.ConvertValueToCurrency(rateValue, currencyId, effectiveOn.HasValue ? effectiveOn.Value : DateTime.Now);
                    CustomerZoneInfo customerZoneInfo = new CustomerZoneInfo
                    {
                        CustomerId = customerId,
                        RoutingProduct = customerZoneRoutingProductLocator.GetCustomerZoneRoutingProduct(customerId, customerSellingProduct.SellingProductId, customerZone.SaleZoneId),
                        SellingProductId = customerSellingProduct.SellingProductId,
                        SaleZoneId = customerZone.SaleZoneId,
                        Rate = customerZoneRate, 
                        EffectiveRateValue= rateValue
                    };

                    onCustomerZoneInfoAvailable(customerZoneInfo);
                }
            }
        }

        public void BuildSupplierZoneRates(DateTime? effectiveOn, bool isEffectiveInFuture, Action<SupplierZoneRate> onSupplierZoneRateAvailable)
        {
            SupplierRateManager supplierRateManager = new SupplierRateManager();
            var supplierRates = supplierRateManager.GetRates(effectiveOn, isEffectiveInFuture);
            SupplierPriceListManager supplierPriceListManager = new SupplierPriceListManager();
            Vanrise.Common.CurrencyExchangeRateManager currencyExchangeRateManager = new Vanrise.Common.CurrencyExchangeRateManager();
            PurchasePricingRuleManager purchasePricingRuleManager = new PurchasePricingRuleManager();
            if (supplierRates != null)
            {
                foreach (var supplierRate in supplierRates)
                {
                    var priceList = supplierPriceListManager.GetPriceList(supplierRate.PriceListId);
                    int currencyId = supplierRate.CurrencyId.HasValue ? supplierRate.CurrencyId.Value : priceList.CurrencyId;
                    PurchasePricingRulesInput purchasePricingRulesInput = new PurchasePricingRulesInput
                    {
                        SupplierId = priceList.SupplierId,
                        SupplierZoneId = supplierRate.ZoneId,
                        Rate = supplierRate,
                        EffectiveOn = effectiveOn,
                        IsEffectiveInFuture = isEffectiveInFuture
                    };
                    var pricingRulesResult = purchasePricingRuleManager.ApplyPricingRules(purchasePricingRulesInput);

                    var rateValue = pricingRulesResult != null ? pricingRulesResult.Rate : supplierRate.NormalRate;
                    rateValue = currencyExchangeRateManager.ConvertValueToCurrency(rateValue, currencyId, effectiveOn.HasValue ? effectiveOn.Value : DateTime.Now);
                    SupplierZoneRate supplierZoneRate = new SupplierZoneRate
                    {
                        SupplierId = priceList.SupplierId,
                        SupplierZoneId = supplierRate.ZoneId,
                        EffectiveRateValue = rateValue
                    };
                    onSupplierZoneRateAvailable(supplierZoneRate);
                }
            }
        }

        #endregion

        #region Private Methods

        SaleRatesByOwner GetRatesByOwner(DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            SaleRateManager saleRateManager = new SaleRateManager();
            var rates = saleRateManager.GetRates(effectiveOn, isEffectiveInFuture);
            throw new NotImplementedException();
        }


        #endregion

        #region Private Classes

        private class SaleRatesByOwner
        {
            public Dictionary<int, SaleRatesByZone> RatesByCustomer { get; set; }

            public Dictionary<int, SaleRatesByZone> RatesBySellingProduct { get; set; }
        }

        private class SaleRatesByZone : Dictionary<long, SaleRate>
        {

        }

        #endregion
    }
}
