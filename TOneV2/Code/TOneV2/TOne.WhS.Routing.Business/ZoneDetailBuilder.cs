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
                    SellingProductId = customerSellingProduct.SellingProductId
                };

                customerInfoDetails.Add(item);
            }

            SaleEntityZoneRoutingProductLocator customerZoneRoutingProductLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadAllNoCache(customerInfos, effectiveOn, isEffectiveInFuture));
            SaleEntityZoneRateLocator customerZoneRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadAllNoCache(customerInfoDetails, effectiveOn, isEffectiveInFuture));
            SaleEntityServiceLocator customerServiceLocator = new SaleEntityServiceLocator(new SaleEntityServiceReadAllNoCache(customerInfoDetails, effectiveOn, isEffectiveInFuture));

            CustomerZoneManager customerZoneManager = new CustomerZoneManager();


            Vanrise.Common.Business.CurrencyExchangeRateManager currencyExchangeRateManager = new Vanrise.Common.Business.CurrencyExchangeRateManager();

            DateTime effectiveDate = effectiveOn.HasValue ? effectiveOn.Value : DateTime.Now;

            Vanrise.Common.Business.ConfigManager commonConfigManager = new Vanrise.Common.Business.ConfigManager();
            int systemCurrencyId = commonConfigManager.GetSystemCurrencyId();

            TOne.WhS.Routing.Business.ConfigManager routingConfigManager = new TOne.WhS.Routing.Business.ConfigManager();
            int customerTransformationId = routingConfigManager.GetCustomerTransformationId();

            DataTransformer dataTransformer = new DataTransformer();

            foreach (RoutingCustomerInfoDetails customerInfo in customerInfoDetails)
            {
                var customerSaleZones = customerZoneManager.GetCustomerSaleZones(customerInfo.CustomerId, effectiveOn.HasValue ? effectiveOn.Value : DateTime.Now, isEffectiveInFuture);
                if (customerSaleZones == null)
                    continue;

                foreach (var customerZone in customerSaleZones)
                {
                    SaleEntityZoneRate customerZoneRate = customerZoneRateLocator.GetCustomerZoneRate(customerInfo.CustomerId, customerInfo.SellingProductId, customerZone.SaleZoneId);
                    SaleEntityService customerEntityService = customerServiceLocator.GetCustomerZoneService(customerInfo.CustomerId, customerInfo.SellingProductId, customerZone.SaleZoneId);

                    if (customerZoneRate != null && customerZoneRate.Rate != null)
                    {
                        var output = dataTransformer.ExecuteDataTransformation(customerTransformationId, (context) =>
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

                        rateValue = decimal.Round(currencyExchangeRateManager.ConvertValueToCurrency(rateValue, currencyId, systemCurrencyId, effectiveDate), 8);
                        var customerZoneRoutingProduct = customerZoneRoutingProductLocator.GetCustomerZoneRoutingProduct(customerInfo.CustomerId, customerInfo.SellingProductId, customerZone.SaleZoneId);

                        CustomerZoneDetail customerZoneDetail = new CustomerZoneDetail
                        {
                            CustomerId = customerInfo.CustomerId,
                            RoutingProductId = customerZoneRoutingProduct != null ? customerZoneRoutingProduct.RoutingProductId : 0,
                            RoutingProductSource = customerZoneRoutingProduct != null ? customerZoneRoutingProduct.Source : default(SaleEntityZoneRoutingProductSource),
                            SellingProductId = customerInfo.SellingProductId,
                            SaleZoneId = customerZone.SaleZoneId,
                            EffectiveRateValue = rateValue,
                            RateSource = customerZoneRate.Source,
                            SaleEntityServiceIds = customerEntityService != null ? new HashSet<int>(customerEntityService.Services.Select(itm => itm.ServiceId)) : null
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
            SupplierZoneRateLocator supplierZoneRateLocator = new SupplierZoneRateLocator(new SupplierRateReadAllNoCache(supplierInfos, effectiveOn, isEffectiveInFuture));

            Vanrise.Common.Business.ConfigManager commonConfigManager = new Vanrise.Common.Business.ConfigManager();
            int systemCurrencyId = commonConfigManager.GetSystemCurrencyId();

            TOne.WhS.Routing.Business.ConfigManager routingConfigManager = new TOne.WhS.Routing.Business.ConfigManager();
            int supplierTransformationId = routingConfigManager.GetSupplierTransformationId();

            DataTransformer dataTransformer = new DataTransformer();

            SupplierPriceListManager supplierPriceListManager = new SupplierPriceListManager();
            Vanrise.Common.Business.CurrencyExchangeRateManager currencyExchangeRateManager = new Vanrise.Common.Business.CurrencyExchangeRateManager();

            DateTime effectiveDate = effectiveOn.HasValue ? effectiveOn.Value : DateTime.Now;

            SupplierZoneManager supplierZoneManager = new SupplierZoneManager();
            foreach (RoutingSupplierInfo supplierInfo in supplierInfos)
            {
                List<SupplierZone> supplierZones = supplierZoneManager.GetSupplierZones(supplierInfo.SupplierId, effectiveDate);
                if (supplierZones == null)
                    continue;

                foreach (var supplierZone in supplierZones)
                {
                    SupplierZoneRate supplierZoneRate = supplierZoneRateLocator.GetSupplierZoneRate(supplierInfo.SupplierId, supplierZone.SupplierZoneId);

                    if (supplierZoneRate != null && supplierZoneRate.Rate != null)
                    {
                        var output = dataTransformer.ExecuteDataTransformation(supplierTransformationId, (context) =>
                        {
                            context.SetRecordValue("SupplierId", supplierInfo.SupplierId);
                            context.SetRecordValue("SupplierZoneId", supplierZone.SupplierZoneId);
                            context.SetRecordValue("NormalRate", supplierZoneRate.Rate);
                            context.SetRecordValue("OtherRates", supplierZoneRate.RatesByRateType);
                            context.SetRecordValue("EffectiveDate", effectiveOn);
                            context.SetRecordValue("IsEffectiveInFuture", isEffectiveInFuture);
                        });

                        decimal rateValue = output.GetRecordValue("EffectiveRate");
                        int currencyId = output.GetRecordValue("SupplierCurrencyId");
                        rateValue = decimal.Round(currencyExchangeRateManager.ConvertValueToCurrency(rateValue, currencyId, systemCurrencyId, effectiveDate), 8);

                        SupplierZoneDetail supplierZoneDetail = new SupplierZoneDetail
                        {
                            SupplierId = supplierInfo.SupplierId,
                            SupplierZoneId = supplierZone.SupplierZoneId,
                            EffectiveRateValue = rateValue
                        };

                        onSupplierZoneDetailAvailable(supplierZoneDetail);
                    }
                }
            }
        }

        #endregion
    }
}
