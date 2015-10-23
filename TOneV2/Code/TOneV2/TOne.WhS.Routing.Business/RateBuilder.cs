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
            CustomerZones customerZones = customerZoneManager.GetCustomerZones(customerId, effectiveOn, isEffectiveInFuture);

            SalePriceListRatesByOwner ratesByOwner = GetRatesByOwner(effectiveOn, isEffectiveInFuture);

            SalePriceListRatesByZone customerRates;
            ratesByOwner.RatesByCustomers.TryGetValue(customerId, out customerRates);

            CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();
            CustomerSellingProduct customerSellingProduct = customerSellingProductManager.GetEffectiveSellingProduct(customerId, effectiveOn, isEffectiveInFuture);
            SalePriceListRatesByZone sellingProductRates = null;
            if (customerSellingProduct != null)
                ratesByOwner.RatesBySellingProduct.TryGetValue(customerSellingProduct.SellingProductId, out sellingProductRates);

            SalePricingRuleManager salePricingRuleManager = new SalePricingRuleManager();
            foreach (var customerZone in customerZones.Zones)
            {
                bool isSellingProductRate = false;
                SalePriceListRate zoneRate;
                if (!customerRates.TryGetValue(customerZone.ZoneId, out zoneRate))
                {
                    if (sellingProductRates != null)
                    {
                        if (sellingProductRates.TryGetValue(customerZone.ZoneId, out zoneRate))
                            isSellingProductRate = true;
                    }
                }
                if (zoneRate != null)
                {
                    SalePricingRulesInput salePricingRulesInput = new SalePricingRulesInput
                    {
                        CustomerId = customerId,
                        SaleZoneId = customerZone.ZoneId,
                        Rate = zoneRate,
                        EffectiveOn = effectiveOn,
                        IsEffectiveInFuture = isEffectiveInFuture
                    };
                    var pricingRulesResult = salePricingRuleManager.ApplyPricingRules(salePricingRulesInput);

                    CustomerZoneRate customerZoneRate = new CustomerZoneRate
                    {
                        CustomerId = customerId,
                        RoutingProductId = zoneRate.RoutingProductId,
                        SellingProductId = isSellingProductRate ? customerSellingProduct.SellingProductId : (int?)null,
                        SaleZoneId = customerZone.ZoneId,
                        Rate = pricingRulesResult != null ? pricingRulesResult.Rate : zoneRate.NormalRate
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
            PurchasePricingRuleManager purchasePricingRuleManager = new PurchasePricingRuleManager();
            if (supplierRates != null)
            {
                foreach (var supplierRate in supplierRates)
                {
                    var priceList = supplierPriceListManager.GetPriceList(supplierRate.PriceListId);
                    PurchasePricingRulesInput purchasePricingRulesInput = new PurchasePricingRulesInput
                    {
                        SupplierId = priceList.SupplierId,
                        SupplierZoneId = supplierRate.ZoneId,
                        Rate = supplierRate,
                        EffectiveOn = effectiveOn,
                        IsEffectiveInFuture = isEffectiveInFuture
                    };
                    var pricingRulesResult = purchasePricingRuleManager.ApplyPricingRules(purchasePricingRulesInput);

                    SupplierZoneRate supplierZoneRate = new SupplierZoneRate
                    {
                        SupplierId = priceList.SupplierId,
                        SupplierZoneId = supplierRate.ZoneId,
                        Rate = pricingRulesResult != null ? pricingRulesResult.Rate : supplierRate.NormalRate
                    };
                    onSupplierZoneRateAvailable(supplierZoneRate);
                }
            }
        }

        #endregion

        #region Private Methods

        SalePriceListRatesByOwner GetRatesByOwner(DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            SaleRateManager saleRateManager = new SaleRateManager();
            var rates = saleRateManager.GetRates(effectiveOn, isEffectiveInFuture);
            throw new NotImplementedException();
        }


        #endregion

        #region Private Classes

        private class SalePriceListRatesByOwner
        {
            public Dictionary<int, SalePriceListRatesByZone> RatesByCustomers { get; set; }

            public Dictionary<int, SalePriceListRatesByZone> RatesBySellingProduct { get; set; }
        }

        private class SalePriceListRatesByZone : Dictionary<long, SalePriceListRate>
        {

        }

        #endregion
    }
}
