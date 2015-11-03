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

        public void BuildCustomerZoneRates(int customerId, DateTime? effectiveOn, bool isEffectiveInFuture, Action<CustomerZoneRate> onCustomerZoneRateAvailable)
        {
            CustomerZoneManager customerZoneManager = new CustomerZoneManager();
            var customerSaleZones = customerZoneManager.GetCustomerSaleZones(customerId, effectiveOn.HasValue ? effectiveOn.Value : DateTime.Now, isEffectiveInFuture);
            if (customerSaleZones == null)
                return;

            SaleRatesByOwner ratesByOwner = GetRatesByOwner(effectiveOn, isEffectiveInFuture);

            SaleRatesByZone customerRates;
            if (!ratesByOwner.RatesByCustomer.TryGetValue(customerId, out customerRates))
                return;

            SalePriceListManager salePriceListManager = new SalePriceListManager();

            CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();
            CustomerSellingProduct customerSellingProduct = customerSellingProductManager.GetEffectiveSellingProduct(customerId, effectiveOn, isEffectiveInFuture);
            SaleRatesByZone sellingProductRates = null;
            if (customerSellingProduct != null)
                ratesByOwner.RatesBySellingProduct.TryGetValue(customerSellingProduct.SellingProductId, out sellingProductRates);

            Vanrise.Common.CurrencyExchangeRateManager currencyExchangeRateManager = new Vanrise.Common.CurrencyExchangeRateManager();
            SalePricingRuleManager salePricingRuleManager = new SalePricingRuleManager();
            foreach (var customerZone in customerSaleZones)
            {
                bool isSellingProductRate = false;
                SaleRate zoneRate;
                if (!customerRates.TryGetValue(customerZone.SaleZoneId, out zoneRate))
                {
                    if (sellingProductRates != null)
                    {
                        if (sellingProductRates.TryGetValue(customerZone.SaleZoneId, out zoneRate))
                            isSellingProductRate = true;
                    }
                }
                if (zoneRate != null)
                {
                    var priceList = salePriceListManager.GetPriceList(zoneRate.PriceListId);
                    int currencyId = zoneRate.CurrencyId.HasValue ? zoneRate.CurrencyId.Value : priceList.CurrencyId;
                    SalePricingRulesInput salePricingRulesInput = new SalePricingRulesInput
                    {
                        CustomerId = customerId,
                        SaleZoneId = customerZone.SaleZoneId,
                        SellingProductId = customerSellingProduct.SellingProductId,
                        Rate = zoneRate,
                        EffectiveOn = effectiveOn,
                        IsEffectiveInFuture = isEffectiveInFuture
                    };
                    var pricingRulesResult = salePricingRuleManager.ApplyPricingRules(salePricingRulesInput);

                    var rateValue = pricingRulesResult != null ? pricingRulesResult.Rate : zoneRate.NormalRate;
                    rateValue = currencyExchangeRateManager.ConvertValueToCurrency(rateValue, currencyId, effectiveOn.HasValue ? effectiveOn.Value : DateTime.Now);
                    CustomerZoneRate customerZoneRate = new CustomerZoneRate
                    {
                        CustomerId = customerId,
                        RoutingProductId = zoneRate.RoutingProductId,
                        SellingProductId = isSellingProductRate ? customerSellingProduct.SellingProductId : (int?)null,
                        SaleZoneId = customerZone.SaleZoneId,
                        Rate = rateValue
                    };
                    
                    onCustomerZoneRateAvailable(customerZoneRate);
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
                        Rate = rateValue
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
