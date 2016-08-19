using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Transformation;

namespace TOne.WhS.Routing.Business
{
    public class ZoneDetailBuilder
    {
        #region Public Methods

        public void BuildCustomerZoneDetails(IEnumerable<RoutingCustomerInfo> customerInfos, DateTime? effectiveOn, bool isEffectiveInFuture, Action<CustomerZoneDetail> onCustomerZoneDetailAvailable)
        {
            if (customerInfos == null)
                return;
            CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();
            List<RoutingCustomerInfoDetails> customerInfoDetails = new List<RoutingCustomerInfoDetails>();

            foreach (RoutingCustomerInfo customerInfo in customerInfos)
            {
                CustomerSellingProduct customerSellingProduct = customerSellingProductManager.GetEffectiveSellingProduct(customerInfo.CustomerId, effectiveOn, isEffectiveInFuture);
                if (customerSellingProduct == null)
                    continue;

                RoutingCustomerInfoDetails item = new RoutingCustomerInfoDetails()
                {
                    CustomerId = customerInfo.CustomerId,
                    SellingProductId = customerSellingProduct.CustomerSellingProductId
                };

                customerInfoDetails.Add(item);
            }

            SaleEntityZoneRoutingProductLocator customerZoneRoutingProductLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadAllNoCache(customerInfos, effectiveOn, isEffectiveInFuture));
            SaleEntityZoneRateLocator customerZoneRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadAllNoCache(customerInfoDetails, effectiveOn, isEffectiveInFuture));

            CustomerZoneManager customerZoneManager = new CustomerZoneManager();


            Vanrise.Common.Business.CurrencyExchangeRateManager currencyExchangeRateManager = new Vanrise.Common.Business.CurrencyExchangeRateManager();

            DateTime effectiveDate = effectiveOn.HasValue ? effectiveOn.Value : DateTime.Now;

            SettingManager settingManager = new SettingManager();
            RouteTechnicalSettingData data = settingManager.GetSetting<RouteTechnicalSettingData>(Constants.RouteTechnicalSettings);
            CurrencySettingData systemCurrency = settingManager.GetSetting<CurrencySettingData>(Vanrise.Common.Business.Constants.BaseCurrencySettingType);

            DataTransformer dataTransformer = new DataTransformer();

            foreach (RoutingCustomerInfoDetails customerInfo in customerInfoDetails)
            {
                var customerSaleZones = customerZoneManager.GetCustomerSaleZones(customerInfo.CustomerId, effectiveOn.HasValue ? effectiveOn.Value : DateTime.Now, isEffectiveInFuture);
                if (customerSaleZones == null)
                    continue;

                foreach (var customerZone in customerSaleZones)
                {
                    SaleEntityZoneRate customerZoneRate = customerZoneRateLocator.GetCustomerZoneRate(customerInfo.CustomerId, customerInfo.SellingProductId, customerZone.SaleZoneId);

                    if (customerZoneRate != null && customerZoneRate.Rate != null)
                    {
                        var output = dataTransformer.ExecuteDataTransformation(data.RouteRuleDataTransformation.CustomerTransformationId, (context) =>
                        {
                            context.SetRecordValue("CustomerId", customerInfo.CustomerId);
                            context.SetRecordValue("SaleZoneId", customerZone.SaleZoneId);
                            context.SetRecordValue("NormalRate", customerZoneRate.Rate);
                            context.SetRecordValue("OtherRates", customerZoneRate.RatesByRateType);
                            context.SetRecordValue("EffectiveDate", effectiveOn);
                            context.SetRecordValue("IsEffectiveInFuture", isEffectiveInFuture);
                        });

                        decimal rateValue = output.GetRecordValue("EffectiveRate");
                        int currencyId = output.GetRecordValue("SaleCurrencyId");

                        rateValue = decimal.Round(currencyExchangeRateManager.ConvertValueToCurrency(rateValue, currencyId, systemCurrency.CurrencyId, effectiveDate), 8);
                        var customerZoneRoutingProduct = customerZoneRoutingProductLocator.GetCustomerZoneRoutingProduct(customerInfo.CustomerId, customerInfo.SellingProductId, customerZone.SaleZoneId);

                        CustomerZoneDetail customerZoneDetail = new CustomerZoneDetail
                        {
                            CustomerId = customerInfo.CustomerId,
                            RoutingProductId = customerZoneRoutingProduct != null ? customerZoneRoutingProduct.RoutingProductId : 0,
                            RoutingProductSource = customerZoneRoutingProduct != null ? customerZoneRoutingProduct.Source : default(SaleEntityZoneRoutingProductSource),
                            SellingProductId = customerInfo.SellingProductId,
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

            if (supplierRates != null)
            {
                SettingManager settingManager = new SettingManager();
                RouteTechnicalSettingData data = settingManager.GetSetting<RouteTechnicalSettingData>(Constants.RouteTechnicalSettings);
                CurrencySettingData systemCurrency = settingManager.GetSetting<CurrencySettingData>(Vanrise.Common.Business.Constants.BaseCurrencySettingType);

                DataTransformer dataTransformer = new DataTransformer();

                SupplierPriceListManager supplierPriceListManager = new SupplierPriceListManager();
                Vanrise.Common.Business.CurrencyExchangeRateManager currencyExchangeRateManager = new Vanrise.Common.Business.CurrencyExchangeRateManager();

                DateTime effectiveDate = effectiveOn.HasValue ? effectiveOn.Value : DateTime.Now;
                foreach (var supplierRate in supplierRates)
                {
                    var priceList = supplierPriceListManager.GetPriceList(supplierRate.PriceListId);

                    var output = dataTransformer.ExecuteDataTransformation(data.RouteRuleDataTransformation.SupplierTransformationId, (context) =>
                    {
                        context.SetRecordValue("SupplierId", priceList.SupplierId);
                        context.SetRecordValue("SupplierZoneId", supplierRate.ZoneId);
                        context.SetRecordValue("NormalRate", supplierRate);
                        context.SetRecordValue("EffectiveDate", effectiveOn);
                        context.SetRecordValue("IsEffectiveInFuture", isEffectiveInFuture);
                    });

                    decimal rateValue = output.GetRecordValue("EffectiveRate");
                    int currencyId = output.GetRecordValue("SupplierCurrencyId");
                    rateValue = decimal.Round(currencyExchangeRateManager.ConvertValueToCurrency(rateValue, currencyId, systemCurrency.CurrencyId, effectiveDate), 8);

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
