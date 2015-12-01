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
    public class ZoneDetailBuilder
    {
        #region Public Methods

        public void BuildCustomerZoneDetails(IEnumerable<RoutingCustomerInfo> customerInfos, DateTime? effectiveOn, bool isEffectiveInFuture, Action<CustomerZoneDetail> onCustomerZoneDetailAvailable)
        {
            //int customerId = 1;

            SaleEntityZoneRoutingProductLocator customerZoneRoutingProductLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadAllNoCache(customerInfos, effectiveOn, isEffectiveInFuture));
            SaleEntityZoneRateLocator customerZoneRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadAllNoCache(customerInfos, effectiveOn, isEffectiveInFuture));

            CustomerZoneManager customerZoneManager = new CustomerZoneManager();
            CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();

            Vanrise.Common.Business.CurrencyExchangeRateManager currencyExchangeRateManager = new Vanrise.Common.Business.CurrencyExchangeRateManager();

            foreach (RoutingCustomerInfo customerInfo in customerInfos)
            {
                var customerSaleZones = customerZoneManager.GetCustomerSaleZones(customerInfo.CustomerId, effectiveOn.HasValue ? effectiveOn.Value : DateTime.Now, isEffectiveInFuture);
                if (customerSaleZones == null)
                    continue;

                CustomerSellingProduct customerSellingProduct = customerSellingProductManager.GetEffectiveSellingProduct(customerInfo.CustomerId, effectiveOn, isEffectiveInFuture);
                if (customerSellingProduct == null)
                    continue;
                SalePricingRuleManager salePricingRuleManager = new SalePricingRuleManager();
                foreach (var customerZone in customerSaleZones)
                {
                    SaleEntityZoneRate customerZoneRate = customerZoneRateLocator.GetCustomerZoneRate(customerInfo.CustomerId, customerSellingProduct.SellingProductId, customerZone.SaleZoneId);

                    if (customerZoneRate != null)
                    {
                        int currencyId = customerZoneRate.Rate.CurrencyId.HasValue ? customerZoneRate.Rate.CurrencyId.Value : customerZoneRate.PriceList.CurrencyId;
                        SalePricingRulesInput salePricingRulesInput = new SalePricingRulesInput
                        {
                            CustomerId = customerInfo.CustomerId,
                            SaleZoneId = customerZone.SaleZoneId,
                            SellingProductId = customerSellingProduct.SellingProductId,
                            Rate = customerZoneRate.Rate,
                            EffectiveOn = effectiveOn,
                            IsEffectiveInFuture = isEffectiveInFuture
                        };
                        //TODO: Check with Samer (Null Reference)
                        //var pricingRulesResult = salePricingRuleManager.ApplyPricingRules(salePricingRulesInput);

                        var rateValue = customerZoneRate.Rate.NormalRate; //pricingRulesResult != null ? pricingRulesResult.Rate : customerZoneRate.Rate.NormalRate;
                        rateValue = currencyExchangeRateManager.ConvertValueToCurrency(rateValue, currencyId, effectiveOn.HasValue ? effectiveOn.Value : DateTime.Now);
                        var customerZoneRoutingProduct = customerZoneRoutingProductLocator.GetCustomerZoneRoutingProduct(customerInfo.CustomerId, customerSellingProduct.SellingProductId, customerZone.SaleZoneId);
                        CustomerZoneDetail customerZoneDetail = new CustomerZoneDetail
                        {
                            CustomerId = customerInfo.CustomerId,
                            RoutingProductId = customerZoneRoutingProduct != null ? customerZoneRoutingProduct.RoutingProductId : 0,
                            RoutingProductSource = customerZoneRoutingProduct != null ? customerZoneRoutingProduct.Source : default(SaleEntityZoneRoutingProductSource),
                            SellingProductId = customerSellingProduct.SellingProductId,
                            SaleZoneId = customerZone.SaleZoneId,
                            EffectiveRateValue = rateValue,
                            RateSource = customerZoneRate.Source
                        };

                        onCustomerZoneDetailAvailable(customerZoneDetail);
                    }
                }
            }
        }

        /// <summary>
        /// Build supplier zone details with rates and apply pricing rules if exist.
        /// </summary>
        /// <param name="effectiveOn">Effective date for rates</param>
        /// <param name="isEffectiveInFuture">True if building process for Future.</param>
        /// <param name="onSupplierZoneDetailAvailable">Action that evalute a Supplier Zone Detail</param>
        public void BuildSupplierZoneDetails(DateTime? effectiveOn, bool isEffectiveInFuture, IEnumerable<RoutingSupplierInfo> supplierInfos, Action<SupplierZoneDetail> onSupplierZoneDetailAvailable)
        {
            SupplierRateManager supplierRateManager = new SupplierRateManager();
            var supplierRates = supplierRateManager.GetRates(effectiveOn, isEffectiveInFuture, supplierInfos);
            SupplierPriceListManager supplierPriceListManager = new SupplierPriceListManager();
            Vanrise.Common.Business.CurrencyExchangeRateManager currencyExchangeRateManager = new Vanrise.Common.Business.CurrencyExchangeRateManager();
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
