using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Transformation;

namespace TOne.WhS.Routing.Business
{
    public class ZoneDetailBuilder
    {
        #region Public Methods

        public void BuildCustomerZoneDetails(IEnumerable<RoutingCustomerInfo> customerInfos, DateTime? effectiveOn, bool isEffectiveInFuture, int versionNumber, Action<CustomerZoneDetail> onCustomerZoneDetailAvailable)
        {
            if (customerInfos == null)
                return;

            CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();
            List<RoutingCustomerInfoDetails> customerInfoDetails = new List<RoutingCustomerInfoDetails>();

            //SaleZone Services variables
            bool isZoneServicesExplicitOnCustomer = ConfigurationManager.AppSettings["TOneWhS_Routing_ZoneServicesExplicitOnCustomer"] == "true";
            SaleEntityServiceLocator customerServiceLocator = null;
            HashSet<int> saleZoneServices = null;

            var carrierAccountManager = new CarrierAccountManager();

            foreach (RoutingCustomerInfo customerInfo in customerInfos)
            {
                int? sellingProductId = carrierAccountManager.GetSellingProductId(customerInfo.CustomerId);

                if (!sellingProductId.HasValue)
                    continue;

                RoutingCustomerInfoDetails item = new RoutingCustomerInfoDetails()
                {
                    CustomerId = customerInfo.CustomerId,
                    SellingProductId = sellingProductId.Value
                };

                customerInfoDetails.Add(item);
            }

            SaleEntityZoneRoutingProductLocator customerZoneRoutingProductLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadAllNoCache(customerInfos, effectiveOn, isEffectiveInFuture));
            SaleEntityZoneRateLocator customerZoneRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadAllNoCache(customerInfoDetails, effectiveOn, isEffectiveInFuture));

            if (isZoneServicesExplicitOnCustomer)
            {
                customerServiceLocator = new SaleEntityServiceLocator(new SaleEntityServiceReadAllNoCache(customerInfoDetails, effectiveOn, isEffectiveInFuture));
            }

            var saleZoneManager = new SaleZoneManager();

            Vanrise.Common.Business.CurrencyExchangeRateManager currencyExchangeRateManager = new Vanrise.Common.Business.CurrencyExchangeRateManager();

            DateTime currencyEffectiveDate = effectiveOn.HasValue ? effectiveOn.Value : DateTime.Now;

            Vanrise.Common.Business.ConfigManager commonConfigManager = new Vanrise.Common.Business.ConfigManager();
            int systemCurrencyId = commonConfigManager.GetSystemCurrencyId();

            TOne.WhS.Routing.Business.ConfigManager routingConfigManager = new TOne.WhS.Routing.Business.ConfigManager();
            Guid customerTransformationId = routingConfigManager.GetCustomerTransformationId();

            IncludedRulesConfiguration includedRulesConfiguration = routingConfigManager.GetIncludedRulesConfiguration();

            DataTransformer dataTransformer = new DataTransformer();

            RPRouteManager rpRouteManager = new RPRouteManager();

            foreach (RoutingCustomerInfoDetails customerInfo in customerInfoDetails)
            {
                int sellingNumberPlanId = carrierAccountManager.GetSellingNumberPlanId(customerInfo.CustomerId, CarrierAccountType.Customer);
                IEnumerable<SaleZone> customerSaleZones = saleZoneManager.GetCustomerSaleZones(customerInfo.CustomerId, effectiveOn, isEffectiveInFuture);

                if (customerSaleZones == null)
                    continue;

                foreach (var customerZone in customerSaleZones)
                {
                    SaleEntityZoneRate customerZoneRate = customerZoneRateLocator.GetCustomerZoneRate(customerInfo.CustomerId, customerInfo.SellingProductId, customerZone.SaleZoneId);

                    if (customerZoneRate != null && customerZoneRate.Rate != null)
                    {
                        var customerZoneRoutingProduct = customerZoneRoutingProductLocator.GetCustomerZoneRoutingProduct(customerInfo.CustomerId, customerInfo.SellingProductId, customerZone.SaleZoneId);

                        if (isZoneServicesExplicitOnCustomer)
                        {
                            SaleEntityService customerService = customerServiceLocator.GetCustomerZoneService(customerInfo.CustomerId, customerInfo.SellingProductId, customerZone.SaleZoneId);

                            //if (customerService == null)
                            //    throw new NullReferenceException(string.Format("customerService. Customer ID: {0} having Selling Product ID: {1} does not contain default services.", customerInfo.CustomerId, customerInfo.SellingProductId));

                            //if (customerService.Services == null)
                            //    throw new NullReferenceException(string.Format("customerService.Services. Customer ID: {0} having Selling Product ID: {1} does not contain default services.", customerInfo.CustomerId, customerInfo.SellingProductId));

                            //if (customerService.Services.Count == 0)
                            //    throw new Exception(string.Format("customerService.Services count is 0. Customer ID: {0} having Selling Product ID: {1} does not contain default services.", customerInfo.CustomerId, customerInfo.SellingProductId));

                            //saleZoneServices = customerService.Services.Select(itm => itm.ServiceId).ToHashSet();
                            if (customerService != null && customerService.Services != null && customerService.Services.Count > 0)
                                saleZoneServices = customerService.Services.Select(itm => itm.ServiceId).ToHashSet();
                        }
                        else
                        {
                            if (customerZoneRoutingProduct == null)
                                throw new NullReferenceException(string.Format("customerZoneRoutingProduct for CustomerId: {0}", customerInfo.CustomerId));

                            saleZoneServices = rpRouteManager.GetSaleZoneServices(customerZoneRoutingProduct.RoutingProductId, customerZone.SaleZoneId);

                            if (saleZoneServices == null || saleZoneServices.Count() == 0)
                                throw new NullReferenceException(string.Format("saleZoneServices for RoutingProductId: {0}, saleZoneId: {1}", customerZoneRoutingProduct.RoutingProductId, customerZone.SaleZoneId));
                        }

                        var output = dataTransformer.ExecuteDataTransformation(customerTransformationId, (context) =>
                        {
                            context.SetRecordValue("CustomerId", customerInfo.CustomerId);
                            context.SetRecordValue("SaleZoneId", customerZone.SaleZoneId);
                            context.SetRecordValue("NormalRate", customerZoneRate.Rate);
                            context.SetRecordValue("OtherRates", customerZoneRate.RatesByRateType);
                            context.SetRecordValue("EffectiveDate", effectiveOn);
                            context.SetRecordValue("IsEffectiveInFuture", isEffectiveInFuture);
                            context.SetRecordValue("IncludedRulesConfiguration", includedRulesConfiguration);
                        });

                        decimal rateValue = output.GetRecordValue("EffectiveRate");
                        int currencyId = output.GetRecordValue("SaleCurrencyId");

                        //rateValue = decimal.Round(currencyExchangeRateManager.ConvertValueToCurrency(rateValue, currencyId, systemCurrencyId, currencyEffectiveDate), 8);
                        rateValue = currencyExchangeRateManager.ConvertValueToCurrency(rateValue, currencyId, systemCurrencyId, currencyEffectiveDate);

                        CustomerZoneDetail customerZoneDetail = new CustomerZoneDetail
                        {
                            CustomerId = customerInfo.CustomerId,
                            RoutingProductId = customerZoneRoutingProduct != null ? customerZoneRoutingProduct.RoutingProductId : 0,
                            RoutingProductSource = customerZoneRoutingProduct != null ? customerZoneRoutingProduct.Source : default(SaleEntityZoneRoutingProductSource),
                            SellingProductId = customerInfo.SellingProductId,
                            SaleZoneId = customerZone.SaleZoneId,
                            EffectiveRateValue = rateValue,
                            RateSource = customerZoneRate.Source,
                            SaleZoneServiceIds = saleZoneServices,
                            VersionNumber = versionNumber
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
        public void BuildSupplierZoneDetails(DateTime? effectiveOn, bool isEffectiveInFuture, IEnumerable<RoutingSupplierInfo> supplierInfos, int versionNumber, Action<SupplierZoneDetail> onSupplierZoneDetailAvailable)
        {
            SupplierRateManager supplierRateManager = new SupplierRateManager();
            SupplierZoneRateLocator supplierZoneRateLocator = new SupplierZoneRateLocator(new SupplierRateReadAllNoCache(supplierInfos, effectiveOn, isEffectiveInFuture));
            SupplierZoneServiceLocator supplierZoneServiceLocator = new SupplierZoneServiceLocator(new SupplierZoneServiceReadAllNoCache(supplierInfos, effectiveOn, isEffectiveInFuture));

            Vanrise.Common.Business.ConfigManager commonConfigManager = new Vanrise.Common.Business.ConfigManager();
            int systemCurrencyId = commonConfigManager.GetSystemCurrencyId();

            TOne.WhS.Routing.Business.ConfigManager routingConfigManager = new TOne.WhS.Routing.Business.ConfigManager();
            Guid supplierTransformationId = routingConfigManager.GetSupplierTransformationId();
            IncludedRulesConfiguration includedRulesConfiguration = routingConfigManager.GetIncludedRulesConfiguration();

            SupplierPriceListManager supplierPriceListManager = new SupplierPriceListManager();
            Vanrise.Common.Business.CurrencyExchangeRateManager currencyExchangeRateManager = new Vanrise.Common.Business.CurrencyExchangeRateManager();

            DateTime currencyEffectiveDate = effectiveOn.HasValue ? effectiveOn.Value : DateTime.Now;

            ZoneServiceConfigManager zoneServiceConfigManager = new ZoneServiceConfigManager();
            DataTransformer dataTransformer = new DataTransformer();

            SupplierZoneManager supplierZoneManager = new SupplierZoneManager();
            foreach (RoutingSupplierInfo supplierInfo in supplierInfos)
            {
                List<SupplierZone> supplierZones = supplierZoneManager.GetEffectiveSupplierZones(supplierInfo.SupplierId, effectiveOn, isEffectiveInFuture);
                if (supplierZones == null)
                    continue;

                foreach (var supplierZone in supplierZones)
                {
                    SupplierZoneRate supplierZoneRate = supplierZoneRateLocator.GetSupplierZoneRate(supplierInfo.SupplierId, supplierZone.SupplierZoneId, effectiveOn);

                    if (supplierZoneRate != null && supplierZoneRate.Rate != null)
                    {
                        SupplierEntityService supplierEntityService = supplierZoneServiceLocator.GetSupplierZoneServices(supplierInfo.SupplierId, supplierZone.SupplierZoneId, effectiveOn);
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
                            context.SetRecordValue("IncludedRulesConfiguration", includedRulesConfiguration);
                        });

                        SupplierRate supplierRate = output.GetRecordValue("SupplierRate");
                        supplierRate.ThrowIfNull("supplierRate", supplierZone.SupplierZoneId);

                        int currencyId = output.GetRecordValue("SupplierCurrencyId");
                        decimal rateValue = output.GetRecordValue("EffectiveRate");

                        //rateValue = decimal.Round(currencyExchangeRateManager.ConvertValueToCurrency(rateValue, currencyId, systemCurrencyId, currencyEffectiveDate), 8);
                        rateValue = currencyExchangeRateManager.ConvertValueToCurrency(rateValue, currencyId, systemCurrencyId, currencyEffectiveDate);

                        SupplierZoneDetail supplierZoneDetail = new SupplierZoneDetail
                        {
                            SupplierId = supplierInfo.SupplierId,
                            SupplierZoneId = supplierZone.SupplierZoneId,
                            EffectiveRateValue = rateValue,
                            SupplierServiceIds = supplierZoneServicesWithChildren != null ? new HashSet<int>(supplierZoneServicesWithChildren.Select(itm => itm.ZoneServiceConfigId)) : null,
                            ExactSupplierServiceIds = exactSupplierZoneServices != null ? new HashSet<int>(exactSupplierZoneServices.Select(itm => itm.ServiceId)) : null,
                            SupplierRateId = supplierRate.SupplierRateId,
                            SupplierRateEED = supplierRate.EED,
                            VersionNumber = versionNumber
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
