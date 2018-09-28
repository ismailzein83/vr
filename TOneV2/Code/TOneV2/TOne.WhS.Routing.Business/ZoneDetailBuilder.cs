using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Transformation;

namespace TOne.WhS.Routing.Business
{
    public class ZoneDetailBuilder
    {
        #region Public Methods

        public void BuildCustomerZoneDetails(IEnumerable<RoutingCustomerInfo> customerInfos, DateTime? effectiveOn, bool isEffectiveInFuture, int versionNumber, Func<bool> shouldStop, Action<CustomerZoneDetail> onCustomerZoneDetailAvailable)
        {
            if (customerInfos == null)
                return;

            CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            DealDefinitionManager dealDefinitionManager = new DealDefinitionManager();
            RPRouteManager rpRouteManager = new RPRouteManager();
            DataTransformer dataTransformer = new DataTransformer();
            var currencyExchangeRateManager = new Vanrise.Common.Business.CurrencyExchangeRateManager();
            var routingConfigManager = new TOne.WhS.Routing.Business.ConfigManager();

            DateTime effectiveOnValue = effectiveOn.HasValue ? effectiveOn.Value : DateTime.Now;

            int systemCurrencyId = new Vanrise.Common.Business.ConfigManager().GetSystemCurrencyId();

            List<RoutingCustomerInfoDetails> customerInfoDetails = new List<RoutingCustomerInfoDetails>();

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

            //SaleZone Services variables
            SaleEntityServiceLocator customerServiceLocator = null;
            bool isZoneServicesExplicitOnCustomer = ConfigurationManager.AppSettings["TOneWhS_Routing_ZoneServicesExplicitOnCustomer"] == "true";
            if (isZoneServicesExplicitOnCustomer)
                customerServiceLocator = new SaleEntityServiceLocator(new SaleEntityServiceReadAllNoCache(customerInfoDetails, effectiveOn, isEffectiveInFuture));

            SaleEntityZoneRateLocator customerZoneRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadAllNoCache(customerInfoDetails, effectiveOn, isEffectiveInFuture));
            SaleEntityZoneRoutingProductLocator customerZoneRoutingProductLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadAllNoCache(customerInfos, effectiveOn, isEffectiveInFuture));

            Guid customerTransformationId = routingConfigManager.GetCustomerTransformationId();
            IncludedRulesConfiguration includedRulesConfiguration = routingConfigManager.GetIncludedRulesConfiguration();

            Dictionary<DealZoneGroup, RoutingCustomerZones> routingCustomerZonesByDealZoneGroup = new Dictionary<DealZoneGroup, RoutingCustomerZones>();

            foreach (RoutingCustomerInfoDetails customerInfo in customerInfoDetails)
            {
                if (shouldStop != null && shouldStop())
                    return;

                int sellingNumberPlanId = carrierAccountManager.GetSellingNumberPlanId(customerInfo.CustomerId, CarrierAccountType.Customer);
                IEnumerable<SaleZone> customerSaleZones = saleZoneManager.GetCustomerSaleZones(customerInfo.CustomerId, effectiveOn, isEffectiveInFuture);
                if (customerSaleZones == null)
                    continue;

                foreach (var customerZone in customerSaleZones)
                {
                    var dealSaleZoneGroupWithoutRate = dealDefinitionManager.GetAccountSaleZoneGroup(customerInfo.CustomerId, customerZone.SaleZoneId, effectiveOnValue);
                    if (dealSaleZoneGroupWithoutRate != null)
                    {
                        DealZoneGroup currentDealZoneGroup = new DealZoneGroup()
                        {
                            DealId = dealSaleZoneGroupWithoutRate.DealId,
                            ZoneGroupNb = dealSaleZoneGroupWithoutRate.DealSaleZoneGroupNb
                        };
                        RoutingCustomerZones routingCustomerZones = routingCustomerZonesByDealZoneGroup.GetOrCreateItem(currentDealZoneGroup, () => { return new RoutingCustomerZones(customerInfo); });
                        routingCustomerZones.SaleZones.Add(customerZone);
                        continue;
                    }

                    var customerZoneDetailWithoutDealInput = new CustomerZoneDetailWithoutDealInput(customerTransformationId, customerInfo, customerZone, effectiveOn, effectiveOnValue, isEffectiveInFuture,
                        includedRulesConfiguration, systemCurrencyId, isZoneServicesExplicitOnCustomer, dataTransformer, versionNumber, null, customerZoneRateLocator, customerZoneRoutingProductLocator,
                        customerServiceLocator, rpRouteManager, currencyExchangeRateManager, onCustomerZoneDetailAvailable);

                    BuildCustomerZoneDetailWithoutDeal(customerZoneDetailWithoutDealInput);
                }
            }

            if (routingCustomerZonesByDealZoneGroup.Any())
            {
                IEnumerable<DealZoneGroup> affectedDealZoneGroup = routingCustomerZonesByDealZoneGroup.Keys;
                Dictionary<DealZoneGroup, DealProgress> dealProgresses = new DealProgressManager().GetDealProgresses(affectedDealZoneGroup.ToHashSet(), true);

                foreach (var kvp in routingCustomerZonesByDealZoneGroup)
                {
                    DealZoneGroup dealZoneGroup = kvp.Key;
                    RoutingCustomerZones routingCustomerZones = kvp.Value;
                    RoutingCustomerInfoDetails customerInfo = routingCustomerZones.CustomerInfo;

                    bool isDealZoneGroupCompleted;
                    DealSubstituteRate substituteRate = dealDefinitionManager.GetSubstitueRate(dealZoneGroup.DealId, dealZoneGroup.ZoneGroupNb, true, dealProgresses, out isDealZoneGroupCompleted);
                    int? dealId = !isDealZoneGroupCompleted ? dealZoneGroup.DealId : default(int?);

                    if (substituteRate != null)
                    {
                        foreach (var customerZone in routingCustomerZones.SaleZones)
                        {
                            SaleEntityZoneRoutingProduct customerZoneRoutingProduct = customerZoneRoutingProductLocator.GetCustomerZoneRoutingProduct(customerInfo.CustomerId,
                                customerInfo.SellingProductId, customerZone.SaleZoneId);

                            HashSet<int> saleZoneServices = GetSaleZoneServices(customerZoneRoutingProduct, customerInfo, isZoneServicesExplicitOnCustomer, customerServiceLocator,
                                customerZone, rpRouteManager);

                            decimal rate = currencyExchangeRateManager.ConvertValueToCurrency(substituteRate.Rate, substituteRate.CurrencyId,
                                systemCurrencyId, effectiveOnValue);

                            BuildCustomerZoneDetail(customerInfo, customerZoneRoutingProduct, customerZone, rate, SalePriceListOwnerType.Deal, saleZoneServices, versionNumber,
                                dealId, onCustomerZoneDetailAvailable);
                        }
                    }
                    else
                    {
                        foreach (var customerZone in routingCustomerZones.SaleZones)
                        {
                            var customerZoneDetailWithoutDealInput = new CustomerZoneDetailWithoutDealInput(customerTransformationId, customerInfo, customerZone, effectiveOn, effectiveOnValue, isEffectiveInFuture,
                                    includedRulesConfiguration, systemCurrencyId, isZoneServicesExplicitOnCustomer, dataTransformer, versionNumber, dealId, customerZoneRateLocator,
                                    customerZoneRoutingProductLocator, customerServiceLocator, rpRouteManager, currencyExchangeRateManager, onCustomerZoneDetailAvailable);

                            BuildCustomerZoneDetailWithoutDeal(customerZoneDetailWithoutDealInput);
                        }
                    }
                }
            }
        }

        private void BuildCustomerZoneDetailWithoutDeal(CustomerZoneDetailWithoutDealInput input)
        {
            RoutingCustomerInfoDetails customerInfo = input.CustomerInfo;
            SaleZone saleZone = input.SaleZone;

            SaleEntityZoneRate customerZoneRate = input.CustomerZoneRateLocator.GetCustomerZoneRate(customerInfo.CustomerId, customerInfo.SellingProductId,
                saleZone.SaleZoneId);

            if (customerZoneRate != null && customerZoneRate.Rate != null)
            {
                SaleEntityZoneRoutingProduct customerZoneRoutingProduct = input.CustomerZoneRoutingProductLocator.GetCustomerZoneRoutingProduct(customerInfo.CustomerId,
                    customerInfo.SellingProductId, saleZone.SaleZoneId);

                HashSet<int> saleZoneServices = GetSaleZoneServices(customerZoneRoutingProduct, customerInfo, input.IsZoneServicesExplicitOnCustomer,
                    input.CustomerServiceLocator, saleZone, input.RpRouteManager);

                var output = input.DataTransformer.ExecuteDataTransformation(input.CustomerTransformationId, (context) =>
                {
                    context.SetRecordValue("CustomerId", customerInfo.CustomerId);
                    context.SetRecordValue("SaleZoneId", saleZone.SaleZoneId);
                    context.SetRecordValue("NormalRate", customerZoneRate.Rate);
                    context.SetRecordValue("OtherRates", customerZoneRate.RatesByRateType);
                    context.SetRecordValue("EffectiveDate", input.EffectiveOn);
                    context.SetRecordValue("IsEffectiveInFuture", input.IsEffectiveInFuture);
                    context.SetRecordValue("IncludedRulesConfiguration", input.IncludedRulesConfiguration);
                });

                decimal rateValue = output.GetRecordValue("EffectiveRate");
                int currencyId = output.GetRecordValue("SaleCurrencyId");

                rateValue = input.CurrencyExchangeRateManager.ConvertValueToCurrency(rateValue, currencyId, input.SystemCurrencyId, input.EffectiveOnValue);

                BuildCustomerZoneDetail(customerInfo, customerZoneRoutingProduct, saleZone, rateValue, customerZoneRate.Source,
                    saleZoneServices, input.VersionNumber, input.DealId, input.OnCustomerZoneDetailAvailable);
            }
        }

        private HashSet<int> GetSaleZoneServices(SaleEntityZoneRoutingProduct customerZoneRoutingProduct, RoutingCustomerInfoDetails customerInfo,
            bool isZoneServicesExplicitOnCustomer, SaleEntityServiceLocator customerServiceLocator, SaleZone customerZone, RPRouteManager rpRouteManager)
        {
            HashSet<int> saleZoneServices = null;

            if (isZoneServicesExplicitOnCustomer)
            {
                SaleEntityService customerService = customerServiceLocator.GetCustomerZoneService(customerInfo.CustomerId, customerInfo.SellingProductId, customerZone.SaleZoneId);

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
            return saleZoneServices;
        }

        private void BuildCustomerZoneDetail(RoutingCustomerInfoDetails customerInfo, SaleEntityZoneRoutingProduct customerZoneRoutingProduct, SaleZone customerZone,
            decimal rateValue, SalePriceListOwnerType salePriceListOwnerType, HashSet<int> saleZoneServices, int versionNumber, int? dealId, Action<CustomerZoneDetail> onCustomerZoneDetailAvailable)
        {
            CustomerZoneDetail customerZoneDetail = new CustomerZoneDetail
            {
                CustomerId = customerInfo.CustomerId,
                RoutingProductId = customerZoneRoutingProduct != null ? customerZoneRoutingProduct.RoutingProductId : 0,
                RoutingProductSource = customerZoneRoutingProduct != null ? customerZoneRoutingProduct.Source : default(SaleEntityZoneRoutingProductSource),
                SellingProductId = customerInfo.SellingProductId,
                SaleZoneId = customerZone.SaleZoneId,
                EffectiveRateValue = rateValue,
                RateSource = salePriceListOwnerType,
                SaleZoneServiceIds = saleZoneServices,
                DealId = dealId,
                VersionNumber = versionNumber
            };

            onCustomerZoneDetailAvailable(customerZoneDetail);
        }


        /// <summary>
        /// Build supplier zone details with rates and apply pricing rules if exist.
        /// </summary>
        /// <param name="effectiveOn">Effective date for rates</param>
        /// <param name="isEffectiveInFuture">True if building process for Future.</param>
        /// <param name="onSupplierZoneDetailAvailable">Action that evalute a Supplier Zone Detail</param>
        public void BuildSupplierZoneDetails(DateTime? effectiveOn, bool isEffectiveInFuture, IEnumerable<RoutingSupplierInfo> supplierInfos, int versionNumber, Func<bool> shouldStop, Action<SupplierZoneDetail> onSupplierZoneDetailAvailable)
        {
            SupplierRateManager supplierRateManager = new SupplierRateManager();
            SupplierPriceListManager supplierPriceListManager = new SupplierPriceListManager();
            ZoneServiceConfigManager zoneServiceConfigManager = new ZoneServiceConfigManager();
            SupplierZoneManager supplierZoneManager = new SupplierZoneManager();
            DataTransformer dataTransformer = new DataTransformer();
            DealDefinitionManager dealDefinitionManager = new DealDefinitionManager();
            var currencyExchangeRateManager = new Vanrise.Common.Business.CurrencyExchangeRateManager();
            var routingConfigManager = new TOne.WhS.Routing.Business.ConfigManager();

            DateTime effectiveOnValue = effectiveOn.HasValue ? effectiveOn.Value : DateTime.Now;

            int systemCurrencyId = new Vanrise.Common.Business.ConfigManager().GetSystemCurrencyId();

            Guid supplierTransformationId = routingConfigManager.GetSupplierTransformationId();
            IncludedRulesConfiguration includedRulesConfiguration = routingConfigManager.GetIncludedRulesConfiguration();

            SupplierZoneRateLocator supplierZoneRateLocator = new SupplierZoneRateLocator(new SupplierRateReadAllNoCache(supplierInfos, effectiveOn, isEffectiveInFuture));
            SupplierZoneServiceLocator supplierZoneServiceLocator = new SupplierZoneServiceLocator(new SupplierZoneServiceReadAllNoCache(supplierInfos, effectiveOn, isEffectiveInFuture));

            Dictionary<DealZoneGroup, RoutingSupplierZones> routingSupplierZonesByDealZoneGroup = new Dictionary<DealZoneGroup, RoutingSupplierZones>();

            foreach (RoutingSupplierInfo supplierInfo in supplierInfos)
            {
                if (shouldStop != null && shouldStop())
                    return;

                List<SupplierZone> supplierZones = supplierZoneManager.GetEffectiveSupplierZones(supplierInfo.SupplierId, effectiveOn, isEffectiveInFuture);
                if (supplierZones == null)
                    continue;

                foreach (var supplierZone in supplierZones)
                {
                    var dealSupplierZoneGroupWithoutRate = dealDefinitionManager.GetAccountSupplierZoneGroup(supplierInfo.SupplierId, supplierZone.SupplierZoneId, effectiveOnValue);
                    if (dealSupplierZoneGroupWithoutRate != null)
                    {
                        DealZoneGroup currentDealZoneGroup = new DealZoneGroup() { DealId = dealSupplierZoneGroupWithoutRate.DealId, ZoneGroupNb = dealSupplierZoneGroupWithoutRate.DealSupplierZoneGroupNb };
                        RoutingSupplierZones routingSupplierZones = routingSupplierZonesByDealZoneGroup.GetOrCreateItem(currentDealZoneGroup, () => { return new RoutingSupplierZones(supplierInfo); });
                        routingSupplierZones.SupplierZones.Add(supplierZone);
                        continue;
                    }

                    var supplierZoneDetailWithoutDealInput = new SupplierZoneDetailWithoutDealInput(supplierTransformationId, supplierZoneRateLocator, supplierInfo, supplierZone,
                            effectiveOn, effectiveOnValue, null, versionNumber, isEffectiveInFuture, includedRulesConfiguration, systemCurrencyId, supplierZoneServiceLocator,
                            zoneServiceConfigManager, currencyExchangeRateManager, dataTransformer, onSupplierZoneDetailAvailable);

                    BuildSupplierZoneDetailWithoutDeal(supplierZoneDetailWithoutDealInput);
                }
            }

            if (routingSupplierZonesByDealZoneGroup.Any())
            {
                IEnumerable<DealZoneGroup> affectedDealZoneGroup = routingSupplierZonesByDealZoneGroup.Keys;
                Dictionary<DealZoneGroup, DealProgress> dealProgresses = new DealProgressManager().GetDealProgresses(affectedDealZoneGroup.ToHashSet(), false);

                foreach (var kvp in routingSupplierZonesByDealZoneGroup)
                {
                    DealZoneGroup dealZoneGroup = kvp.Key;
                    RoutingSupplierZones routingSupplierZones = kvp.Value;
                    RoutingSupplierInfo supplierInfo = routingSupplierZones.SupplierInfo;

                    bool isDealZoneGroupCompleted;
                    DealSubstituteRate substituteRate = dealDefinitionManager.GetSubstitueRate(dealZoneGroup.DealId, dealZoneGroup.ZoneGroupNb, false, dealProgresses, out isDealZoneGroupCompleted);
                    int? dealId = !isDealZoneGroupCompleted ? dealZoneGroup.DealId : default(int?);

                    if (substituteRate != null)
                    {
                        foreach (var supplierZone in routingSupplierZones.SupplierZones)
                        {
                            SupplierEntityService supplierEntityService = supplierZoneServiceLocator.GetSupplierZoneServices(supplierInfo.SupplierId, supplierZone.SupplierZoneId, effectiveOn);

                            List<ZoneService> exactSupplierZoneServices = supplierEntityService.Services;
                            IEnumerable<ZoneServiceConfig> supplierZoneServicesWithChildren = zoneServiceConfigManager.GetDistinctZoneServiceConfigsWithChildren(exactSupplierZoneServices.Select(itm => itm.ServiceId));

                            decimal rate = currencyExchangeRateManager.ConvertValueToCurrency(substituteRate.Rate, substituteRate.CurrencyId, systemCurrencyId, effectiveOnValue);

                            BuildSupplierZoneDetail(supplierInfo, supplierZone, rate, dealId, versionNumber, null, exactSupplierZoneServices, supplierZoneServicesWithChildren, zoneServiceConfigManager, onSupplierZoneDetailAvailable);
                        }
                    }
                    else
                    {
                        foreach (var supplierZone in routingSupplierZones.SupplierZones)
                        {
                            var buildNonDealSupplierZoneDetailInput = new SupplierZoneDetailWithoutDealInput(supplierTransformationId, supplierZoneRateLocator, supplierInfo, supplierZone,
                                    effectiveOn, effectiveOnValue, dealId, versionNumber, isEffectiveInFuture, includedRulesConfiguration, systemCurrencyId, supplierZoneServiceLocator,
                                    zoneServiceConfigManager, currencyExchangeRateManager, dataTransformer, onSupplierZoneDetailAvailable);

                            BuildSupplierZoneDetailWithoutDeal(buildNonDealSupplierZoneDetailInput);
                        }
                    }
                }
            }
        }

        private void BuildSupplierZoneDetailWithoutDeal(SupplierZoneDetailWithoutDealInput input)
        {
            SupplierZoneRate supplierZoneRate = input.SupplierZoneRateLocator.GetSupplierZoneRate(input.SupplierInfo.SupplierId, input.SupplierZone.SupplierZoneId, input.EffectiveOn);

            if (supplierZoneRate != null && supplierZoneRate.Rate != null)
            {
                SupplierEntityService supplierEntityService = input.SupplierZoneServiceLocator.GetSupplierZoneServices(input.SupplierInfo.SupplierId,
                    input.SupplierZone.SupplierZoneId, input.EffectiveOn);

                List<ZoneService> exactSupplierZoneServices = supplierEntityService.Services;
                IEnumerable<ZoneServiceConfig> supplierZoneServicesWithChildren = input.ZoneServiceConfigManager.GetDistinctZoneServiceConfigsWithChildren(exactSupplierZoneServices.Select(itm => itm.ServiceId));

                var output = input.DataTransformer.ExecuteDataTransformation(input.SupplierTransformationId, (context) =>
                {
                    context.SetRecordValue("SupplierId", input.SupplierInfo.SupplierId);
                    context.SetRecordValue("SupplierZoneId", input.SupplierZone.SupplierZoneId);
                    context.SetRecordValue("NormalRate", supplierZoneRate.Rate);
                    context.SetRecordValue("OtherRates", supplierZoneRate.RatesByRateType);
                    context.SetRecordValue("EffectiveDate", input.EffectiveOn);
                    context.SetRecordValue("IsEffectiveInFuture", input.IsEffectiveInFuture);
                    context.SetRecordValue("IncludedRulesConfiguration", input.IncludedRulesConfiguration);
                });

                SupplierRate supplierRate = output.GetRecordValue("SupplierRate");
                supplierRate.ThrowIfNull("supplierRate", input.SupplierZone.SupplierZoneId);

                int currencyId = output.GetRecordValue("SupplierCurrencyId");
                decimal rateValue = output.GetRecordValue("EffectiveRate");

                rateValue = input.CurrencyExchangeRateManager.ConvertValueToCurrency(rateValue, currencyId, input.SystemCurrencyId, input.EffectiveOnValue);

                BuildSupplierZoneDetail(input.SupplierInfo, input.SupplierZone, rateValue, input.DealId, input.VersionNumber, supplierRate, exactSupplierZoneServices, supplierZoneServicesWithChildren, input.ZoneServiceConfigManager, input.OnSupplierZoneDetailAvailable);
            }
        }

        private void BuildSupplierZoneDetail(RoutingSupplierInfo supplierInfo, SupplierZone supplierZone, decimal rateValue, int? dealId, int versionNumber,
            SupplierRate supplierRate, List<ZoneService> exactSupplierZoneServices, IEnumerable<ZoneServiceConfig> supplierZoneServicesWithChildren,
            ZoneServiceConfigManager zoneServiceConfigManager, Action<SupplierZoneDetail> onSupplierZoneDetailAvailable)
        {
            SupplierZoneDetail supplierZoneDetail = new SupplierZoneDetail
            {
                SupplierId = supplierInfo.SupplierId,
                SupplierZoneId = supplierZone.SupplierZoneId,
                EffectiveRateValue = rateValue,
                SupplierServiceIds = supplierZoneServicesWithChildren != null ? new HashSet<int>(supplierZoneServicesWithChildren.Select(itm => itm.ZoneServiceConfigId)) : null,
                ExactSupplierServiceIds = exactSupplierZoneServices != null ? new HashSet<int>(exactSupplierZoneServices.Select(itm => itm.ServiceId)) : null,
                SupplierRateId = supplierRate != null ? supplierRate.SupplierRateId : default(long?),
                SupplierRateEED = supplierRate != null ? supplierRate.EED : null,
                DealId = dealId,
                VersionNumber = versionNumber
            };

            if (supplierZoneDetail.ExactSupplierServiceIds != null)
                supplierZoneDetail.SupplierServiceWeight = exactSupplierZoneServices != null ? zoneServiceConfigManager.GetAllZoneServicesByIds(supplierZoneDetail.ExactSupplierServiceIds).Sum(itm => itm.Settings.Weight) : 0;

            onSupplierZoneDetailAvailable(supplierZoneDetail);
        }

        #endregion

        #region Private Classes

        private class RoutingCustomerZones
        {
            public RoutingCustomerZones(RoutingCustomerInfoDetails customerInfo)
            {
                this.CustomerInfo = customerInfo;
                SaleZones = new List<SaleZone>();
            }

            public RoutingCustomerInfoDetails CustomerInfo { get; set; }

            public List<SaleZone> SaleZones { get; set; }
        }

        private class RoutingSupplierZones
        {
            public RoutingSupplierZones(RoutingSupplierInfo supplierInfo)
            {
                this.SupplierInfo = supplierInfo;
                SupplierZones = new List<SupplierZone>();
            }

            public RoutingSupplierInfo SupplierInfo { get; set; }

            public List<SupplierZone> SupplierZones { get; set; }
        }

        private class CustomerZoneDetailWithoutDealInput
        {
            public Guid CustomerTransformationId { get; set; }
            public RoutingCustomerInfoDetails CustomerInfo { get; set; }
            public SaleZone SaleZone { get; set; }
            public DateTime? EffectiveOn { get; set; }
            public DateTime EffectiveOnValue { get; set; }
            public bool IsEffectiveInFuture { get; set; }
            public IncludedRulesConfiguration IncludedRulesConfiguration { get; set; }
            public int SystemCurrencyId { get; set; }
            public bool IsZoneServicesExplicitOnCustomer { get; set; }
            public DataTransformer DataTransformer { get; set; }
            public int VersionNumber { get; set; }
            public int? DealId { get; set; }
            public SaleEntityZoneRateLocator CustomerZoneRateLocator { get; set; }
            public SaleEntityZoneRoutingProductLocator CustomerZoneRoutingProductLocator { get; set; }
            public SaleEntityServiceLocator CustomerServiceLocator { get; set; }
            public RPRouteManager RpRouteManager { get; set; }
            public Vanrise.Common.Business.CurrencyExchangeRateManager CurrencyExchangeRateManager { get; set; }
            public Action<CustomerZoneDetail> OnCustomerZoneDetailAvailable { get; set; }

            public CustomerZoneDetailWithoutDealInput(Guid customerTransformationId, RoutingCustomerInfoDetails customerInfo, SaleZone saleZone, DateTime? effectiveOn,
                DateTime effectiveOnValue, bool isEffectiveInFuture, IncludedRulesConfiguration includedRulesConfiguration, int systemCurrencyId, bool isZoneServicesExplicitOnCustomer,
                DataTransformer dataTransformer, int versionNumber, int? dealId, SaleEntityZoneRateLocator customerZoneRateLocator, SaleEntityZoneRoutingProductLocator customerZoneRoutingProductLocator,
                SaleEntityServiceLocator customerServiceLocator, RPRouteManager rpRouteManager, Vanrise.Common.Business.CurrencyExchangeRateManager currencyExchangeRateManager,
                Action<CustomerZoneDetail> onCustomerZoneDetailAvailable)
            {
                this.CustomerTransformationId = customerTransformationId;
                this.CustomerInfo = customerInfo;
                this.SaleZone = saleZone;
                this.EffectiveOn = effectiveOn;
                this.EffectiveOnValue = effectiveOnValue;
                this.IsEffectiveInFuture = isEffectiveInFuture;
                this.IncludedRulesConfiguration = includedRulesConfiguration;
                this.SystemCurrencyId = systemCurrencyId;
                this.IsZoneServicesExplicitOnCustomer = isZoneServicesExplicitOnCustomer;
                this.DataTransformer = dataTransformer;
                this.VersionNumber = versionNumber;
                this.DealId = dealId;
                this.CustomerZoneRateLocator = customerZoneRateLocator;
                this.CustomerZoneRoutingProductLocator = customerZoneRoutingProductLocator;
                this.CustomerServiceLocator = customerServiceLocator;
                this.RpRouteManager = rpRouteManager;
                this.CurrencyExchangeRateManager = currencyExchangeRateManager;
                this.OnCustomerZoneDetailAvailable = onCustomerZoneDetailAvailable;
            }
        }

        private class SupplierZoneDetailWithoutDealInput
        {
            public Guid SupplierTransformationId { get; set; }
            public SupplierZoneRateLocator SupplierZoneRateLocator { get; set; }
            public RoutingSupplierInfo SupplierInfo { get; set; }
            public SupplierZone SupplierZone { get; set; }
            public DateTime? EffectiveOn { get; set; }
            public DateTime EffectiveOnValue { get; set; }
            public int? DealId { get; set; }
            public int VersionNumber { get; set; }
            public bool IsEffectiveInFuture { get; set; }
            public IncludedRulesConfiguration IncludedRulesConfiguration { get; set; }
            public int SystemCurrencyId { get; set; }
            public SupplierZoneServiceLocator SupplierZoneServiceLocator { get; set; }
            public ZoneServiceConfigManager ZoneServiceConfigManager { get; set; }
            public Vanrise.Common.Business.CurrencyExchangeRateManager CurrencyExchangeRateManager { get; set; }
            public DataTransformer DataTransformer { get; set; }
            public Action<SupplierZoneDetail> OnSupplierZoneDetailAvailable { get; set; }

            public SupplierZoneDetailWithoutDealInput(Guid supplierTransformationId, SupplierZoneRateLocator supplierZoneRateLocator, RoutingSupplierInfo supplierInfo, SupplierZone supplierZone,
                DateTime? effectiveOn, DateTime effectiveOnValue, int? dealId, int versionNumber, bool isEffectiveInFuture, IncludedRulesConfiguration includedRulesConfiguration, int systemCurrencyId,
                SupplierZoneServiceLocator supplierZoneServiceLocator, ZoneServiceConfigManager zoneServiceConfigManager, Vanrise.Common.Business.CurrencyExchangeRateManager currencyExchangeRateManager,
                DataTransformer dataTransformer, Action<SupplierZoneDetail> onSupplierZoneDetailAvailable)
            {
                this.SupplierTransformationId = supplierTransformationId;
                this.SupplierZoneRateLocator = supplierZoneRateLocator;
                this.SupplierInfo = supplierInfo;
                this.SupplierZone = supplierZone;
                this.EffectiveOn = effectiveOn;
                this.EffectiveOnValue = effectiveOnValue;
                this.DealId = dealId;
                this.VersionNumber = versionNumber;
                this.IsEffectiveInFuture = isEffectiveInFuture;
                this.IncludedRulesConfiguration = includedRulesConfiguration;
                this.SystemCurrencyId = systemCurrencyId;
                this.SupplierZoneServiceLocator = supplierZoneServiceLocator;
                this.ZoneServiceConfigManager = zoneServiceConfigManager;
                this.CurrencyExchangeRateManager = currencyExchangeRateManager;
                this.DataTransformer = dataTransformer;
                this.OnSupplierZoneDetailAvailable = onSupplierZoneDetailAvailable;
            }
        }

        #endregion
    }
}