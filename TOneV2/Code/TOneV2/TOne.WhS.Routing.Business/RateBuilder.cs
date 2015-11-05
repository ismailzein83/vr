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

        public void BuildCustomerZoneDetails(int customerId, DateTime? effectiveOn, bool isEffectiveInFuture, Action<CustomerZoneDetail> onCustomerZoneDetailAvailable)
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
                    var customerZoneRoutingProduct = customerZoneRoutingProductLocator.GetCustomerZoneRoutingProduct(customerId, customerSellingProduct.SellingProductId, customerZone.SaleZoneId);
                    CustomerZoneDetail customerZoneDetail = new CustomerZoneDetail
                    {
                        CustomerId = customerId,
                        RoutingProductId = customerZoneRoutingProduct != null ? customerZoneRoutingProduct.RoutingProductId : 0,
                        RoutingProductSource = customerZoneRoutingProduct != null ? customerZoneRoutingProduct.Source : default(CustomerZoneRoutingProductSource),
                        SellingProductId = customerSellingProduct.SellingProductId,
                        SaleZoneId = customerZone.SaleZoneId,
                        EffectiveRateValue= rateValue,
                        RateSource = customerZoneRate.Source
                    };

                    onCustomerZoneDetailAvailable(customerZoneDetail);
                }
            }
        }

        public void BuildSupplierZoneDetails(DateTime? effectiveOn, bool isEffectiveInFuture, Action<SupplierZoneDetail> onSupplierZoneDetailAvailable)
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
                    SupplierZoneDetail supplierZoneDetail = new SupplierZoneDetail
                    {
                        SupplierId = priceList.SupplierId,
                        SupplierZoneId = supplierRate.ZoneId,
                        EffectiveRateValue = rateValue
                    };
                    onSupplierZoneDetailAvailable(supplierZoneDetail);
                }
            }
        }

        #endregion
    }
}
