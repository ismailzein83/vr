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
            Guid customerTransformationId = routingConfigManager.GetCustomerTransformationId();

            DataTransformer dataTransformer = new DataTransformer();
            var carrierAccountManager = new CarrierAccountManager();

            foreach (RoutingCustomerInfoDetails customerInfo in customerInfoDetails)
            {
                int sellingNumberPlanId = carrierAccountManager.GetSellingNumberPlanId(customerInfo.CustomerId, CarrierAccountType.Customer);
                var customerSaleZones = customerZoneManager.GetCustomerSaleZones(customerInfo.CustomerId, sellingNumberPlanId, effectiveOn.HasValue ? effectiveOn.Value : DateTime.Now, isEffectiveInFuture);
                if (customerSaleZones == null)
                    continue;

                foreach (var customerZone in customerSaleZones)
                {
                    SaleEntityZoneRate customerZoneRate = customerZoneRateLocator.GetCustomerZoneRate(customerInfo.CustomerId, customerInfo.SellingProductId, customerZone.SaleZoneId);

                    if (customerZoneRate != null && customerZoneRate.Rate != null)
                    {
                        SaleEntityService customerService = customerServiceLocator.GetCustomerZoneService(customerInfo.CustomerId, customerInfo.SellingProductId, customerZone.SaleZoneId);

                        if (customerService == null)
                            throw new NullReferenceException(string.Format("customerService. Customer ID: {0} having Selling Product ID: {1} does not contain default services.", customerInfo.CustomerId, customerInfo.SellingProductId));

                        if (customerService.Services == null)
                            throw new NullReferenceException(string.Format("customerService.Services. Customer ID: {0} having Selling Product ID: {1} does not contain default services.", customerInfo.CustomerId, customerInfo.SellingProductId));

                        if (customerService.Services.Count == 0)
                            throw new Exception(string.Format("customerService.Services count is 0. Customer ID: {0} having Selling Product ID: {1} does not contain default services.", customerInfo.CustomerId, customerInfo.SellingProductId));

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
                            CustomerServiceIds = customerService != null ? new HashSet<int>(customerService.Services.Select(itm => itm.ServiceId)) : null
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
            SupplierZoneServiceLocator supplierZoneServiceLocator = new SupplierZoneServiceLocator(new SupplierZoneServiceReadAllNoCache(supplierInfos, effectiveOn, isEffectiveInFuture));

            Vanrise.Common.Business.ConfigManager commonConfigManager = new Vanrise.Common.Business.ConfigManager();
            int systemCurrencyId = commonConfigManager.GetSystemCurrencyId();

            TOne.WhS.Routing.Business.ConfigManager routingConfigManager = new TOne.WhS.Routing.Business.ConfigManager();
            Guid supplierTransformationId = routingConfigManager.GetSupplierTransformationId();

            SupplierPriceListManager supplierPriceListManager = new SupplierPriceListManager();
            Vanrise.Common.Business.CurrencyExchangeRateManager currencyExchangeRateManager = new Vanrise.Common.Business.CurrencyExchangeRateManager();

            DateTime effectiveDate = effectiveOn.HasValue ? effectiveOn.Value : DateTime.Now;

            ZoneServiceConfigManager zoneServiceConfigManager = new ZoneServiceConfigManager();
            DataTransformer dataTransformer = new DataTransformer();

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
                        SupplierEntityService supplierEntityService = supplierZoneServiceLocator.GetSupplierZoneServices(supplierInfo.SupplierId, supplierZone.SupplierZoneId);
                        List<ZoneService> exactSupplierZoneServices = supplierEntityService.Services;
                        IEnumerable<ZoneServiceConfig> supplierZoneServicesWithChildren = zoneServiceConfigManager.GetDistinctZoneServiceConfigsWithChildren(exactSupplierZoneServices.Select(itm => itm.ServiceId));

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
                            EffectiveRateValue = rateValue,
                            SupplierServiceIds = supplierZoneServicesWithChildren != null ? new HashSet<int>(supplierZoneServicesWithChildren.Select(itm => itm.ZoneServiceConfigId)) : null,
                            ExactSupplierServiceIds = exactSupplierZoneServices != null ? new HashSet<int>(exactSupplierZoneServices.Select(itm => itm.ServiceId)) : null,
                        };
                        if (supplierZoneDetail.ExactSupplierServiceIds != null)
                            supplierZoneDetail.SupplierServiceWeight = exactSupplierZoneServices != null ? zoneServiceConfigManager.GetAllZoneServicesByIds(supplierZoneDetail.ExactSupplierServiceIds).Sum(itm => itm.Settings.Weight) : 0;
                        
                        onSupplierZoneDetailAvailable(supplierZoneDetail);
                    }
                }
            }
        }

        #endregion
    }
}
