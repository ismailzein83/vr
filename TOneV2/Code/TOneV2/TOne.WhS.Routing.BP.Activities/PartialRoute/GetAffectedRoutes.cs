using System;
using System.Linq;
using System.Activities;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.Routing.BP.Activities
{
    public sealed class GetAffectedRoutes : CodeActivity
    {
        [RequiredArgument]
        public InArgument<BERouteInfo> BERouteInfo { get; set; }

        [RequiredArgument]
        public InArgument<PartialRouteInfo> PartialRouteInfo { get; set; }

        [RequiredArgument]
        public InArgument<RoutingDatabase> RoutingDatabase { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public InArgument<AffectedRouteRules> AffectedRouteRules { get; set; }

        [RequiredArgument]
        public InArgument<AffectedRouteOptionRules> AffectedRouteOptionRules { get; set; }

        [RequiredArgument]
        public OutArgument<List<CustomerRouteData>> AffectedCustomerRoutes { get; set; }

        [RequiredArgument]
        public OutArgument<bool> ShouldTriggerFullRouteProcess { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            DateTime effectiveDate = this.EffectiveDate.Get(context);
            var routingDatabase = this.RoutingDatabase.Get(context);
            BERouteInfo beRouteInfo = this.BERouteInfo.Get(context);
            PartialRouteInfo partialRouteInfo = this.PartialRouteInfo.Get(context);

            ICustomerRouteDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICustomerRouteDataManager>();
            dataManager.RoutingDatabase = routingDatabase;

            AffectedRouteRules affectedRouteRules = this.AffectedRouteRules.Get(context);
            AffectedRouteOptionRules affectedRouteOptionRules = this.AffectedRouteOptionRules.Get(context);

            bool shouldTriggerFullRouteProcess = false;
            List<AffectedRoutes> affectedRoutesList = new List<AffectedRoutes>();
            List<AffectedRouteOptions> affectedRouteOptionsList = new List<AffectedRouteOptions>();

            if (affectedRouteRules != null)
            {
                BuildAffectedRoutes(affectedRouteRules.AddedRouteRules, affectedRoutesList, effectiveDate, out shouldTriggerFullRouteProcess);

                if (!shouldTriggerFullRouteProcess)
                    BuildAffectedRoutes(affectedRouteRules.UpdatedRouteRules, affectedRoutesList, effectiveDate, out shouldTriggerFullRouteProcess);

                if (!shouldTriggerFullRouteProcess)
                    BuildAffectedRoutes(affectedRouteRules.OpenedRouteRules, affectedRoutesList, effectiveDate, out shouldTriggerFullRouteProcess);

                if (!shouldTriggerFullRouteProcess)
                    BuildAffectedRoutes(affectedRouteRules.ClosedRouteRules, affectedRoutesList, effectiveDate, out shouldTriggerFullRouteProcess);
            }

            if (!shouldTriggerFullRouteProcess && affectedRouteOptionRules != null)
            {
                BuildAffectedRouteOptions(affectedRouteOptionRules.AddedRouteOptionRules, affectedRouteOptionsList, effectiveDate, out shouldTriggerFullRouteProcess);

                if (!shouldTriggerFullRouteProcess)
                    BuildAffectedRouteOptions(affectedRouteOptionRules.UpdatedRouteOptionRules, affectedRouteOptionsList, effectiveDate, out shouldTriggerFullRouteProcess);

                if (!shouldTriggerFullRouteProcess)
                    BuildAffectedRouteOptions(affectedRouteOptionRules.OpenedRouteOptionRules, affectedRouteOptionsList, effectiveDate, out shouldTriggerFullRouteProcess);

                if (!shouldTriggerFullRouteProcess)
                    BuildAffectedRouteOptions(affectedRouteOptionRules.ClosedRouteOptionRules, affectedRouteOptionsList, effectiveDate, out shouldTriggerFullRouteProcess);
            }

            if (!shouldTriggerFullRouteProcess)
            {
                if (beRouteInfo.SaleRateRouteInfo.LatestVersionNumber > partialRouteInfo.LatestSaleRateVersionNumber)
                {
                    ICustomerZoneDetailsDataManager customerZoneDetailsDataManager = RoutingDataManagerFactory.GetDataManager<ICustomerZoneDetailsDataManager>();
                    customerZoneDetailsDataManager.RoutingDatabase = routingDatabase;
                    List<CustomerZoneDetail> customerZoneDetails = customerZoneDetailsDataManager.GetCustomerZoneDetailsAfterVersionNumber(partialRouteInfo.LatestSaleRateVersionNumber);
                    UpdateAffectedRouteList(affectedRoutesList, customerZoneDetails);
                }

                if (beRouteInfo.SupplierRateRouteInfo.LatestVersionNumber > partialRouteInfo.LatestCostRateVersionNumber)
                {
                    ISupplierZoneDetailsDataManager supplierZoneDetailsDataManager = RoutingDataManagerFactory.GetDataManager<ISupplierZoneDetailsDataManager>();
                    supplierZoneDetailsDataManager.RoutingDatabase = routingDatabase;
                    List<SupplierZoneDetail> supplierZoneDetails = supplierZoneDetailsDataManager.GetSupplierZoneDetailsAfterVersionNumber(partialRouteInfo.LatestCostRateVersionNumber);
                    UpdateAffectedRouteList(affectedRouteOptionsList, supplierZoneDetails);
                }
            }

            if (shouldTriggerFullRouteProcess)
            {
                this.ShouldTriggerFullRouteProcess.Set(context, true);
                context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Warning, "Full Route Build will be triggered", null);
                return;
            }

            List<CustomerRouteData> affectedCustomerRoutes = null;
            if (affectedRoutesList.Count > 0 || affectedRouteOptionsList.Count > 0)
            {
                long customerRouteTotalCount = dataManager.GetTotalCount();
                int partialRoutesPercentageLimit = new TOne.WhS.Routing.Business.ConfigManager().GetPartialRoutesPercentageLimit();
                long partialRoutesNumberLimit = partialRoutesPercentageLimit * customerRouteTotalCount / 100;

                bool maximumExceeded;

                affectedCustomerRoutes = dataManager.GetAffectedCustomerRoutes(affectedRoutesList, affectedRouteOptionsList, partialRoutesNumberLimit, out maximumExceeded);

                if (maximumExceeded)
                {
                    this.ShouldTriggerFullRouteProcess.Set(context, true);
                    context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Warning, "Number of Affected Routes exceeds Maximum Number of Routes for Partial Routing '{0}'. Full Route Build will be triggered", partialRoutesNumberLimit);
                    return;
                }
            }

            if (affectedCustomerRoutes == null || affectedCustomerRoutes.Count == 0)
            {
                context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Warning, "No Affected Routes found.");
                return;
            }

            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Loading Affected Routes is done. Number of Affected Routes: {0}", affectedCustomerRoutes.Count);

            this.AffectedCustomerRoutes.Set(context, affectedCustomerRoutes);
            this.ShouldTriggerFullRouteProcess.Set(context, shouldTriggerFullRouteProcess);
        }

        private void UpdateAffectedRouteList(List<AffectedRoutes> affectedRoutesList, List<CustomerZoneDetail> customerZoneDetails)
        {
            if (customerZoneDetails == null || customerZoneDetails.Count == 0)
                return;

            Dictionary<int, List<long>> customerZoneDetailsDcit = new Dictionary<int, List<long>>();
            foreach (CustomerZoneDetail customerZoneDetail in customerZoneDetails)
            {
                List<long> zones = customerZoneDetailsDcit.GetOrCreateItem(customerZoneDetail.CustomerId);
                zones.Add(customerZoneDetail.SaleZoneId);
            }

            foreach (var kvp in customerZoneDetailsDcit)
            {
                AffectedRoutes affectedRoutes = new AffectedRoutes()
                {
                    CustomerIds = new List<int>() { kvp.Key },
                    ZoneIds = kvp.Value

                };
                affectedRoutesList.Add(affectedRoutes);
            }
        }

        private void UpdateAffectedRouteList(List<AffectedRouteOptions> affectedRouteOptionsList, List<SupplierZoneDetail> supplierZoneDetails)
        {
            if (supplierZoneDetails == null || supplierZoneDetails.Count == 0)
                return;

            Dictionary<int, List<long>> supplierwithZonesDict = new Dictionary<int, List<long>>();
            foreach (SupplierZoneDetail supplierZoneDetail in supplierZoneDetails)
            {
                List<long> zones = supplierwithZonesDict.GetOrCreateItem(supplierZoneDetail.SupplierId);
                zones.Add(supplierZoneDetail.SupplierZoneId);
            }

            foreach (var kvp in supplierwithZonesDict)
            {
                SupplierWithZones supplierWithZones = new SupplierWithZones()
                {
                    SupplierId = kvp.Key,
                    SupplierZoneIds = kvp.Value
                };
                AffectedRouteOptions affectedRouteOptions = new AffectedRouteOptions() { SupplierWithZones = new List<SupplierWithZones>() { supplierWithZones } };
                affectedRouteOptionsList.Add(affectedRouteOptions);
            }
        }

        private void BuildAffectedRoutes(List<RouteRule> routeRules, List<AffectedRoutes> affectedRoutesList, DateTime effectiveDate, out bool shouldTriggerFullRouteProcess)
        {
            shouldTriggerFullRouteProcess = false;
            if (routeRules != null)
            {
                foreach (RouteRule routeRule in routeRules)
                {
                    RouteRuleCriteria criteria = routeRule.Criteria as RouteRuleCriteria;
                    bool isCustomerGeneric;
                    IEnumerable<int> affectedCustomers = GetAffectedCustomers(criteria, out isCustomerGeneric);

                    IEnumerable<CodeCriteria> affectedCodes;
                    IEnumerable<long> affectedZones;

                    bool areCodesAndZonesGeneric;
                    GetAffectedCodesAndZones(criteria, effectiveDate, out affectedCodes, out affectedZones, out areCodesAndZonesGeneric);

                    if (isCustomerGeneric && areCodesAndZonesGeneric)
                    {
                        shouldTriggerFullRouteProcess = true;
                        return;
                    }

                    var routingExcludedDestinationData = (criteria != null && criteria.ExcludedDestinations != null) ? criteria.ExcludedDestinations.GetRoutingExcludedDestinationData() : null;

                    affectedRoutesList.Add(new Entities.AffectedRoutes()
                    {
                        Codes = affectedCodes,
                        CustomerIds = affectedCustomers,
                        RoutingExcludedDestinationData = routingExcludedDestinationData,
                        ZoneIds = affectedZones
                    });
                }
            }
        }

        private void BuildAffectedRouteOptions(List<RouteOptionRule> routeOptionRules, List<AffectedRouteOptions> affectedRouteOptionsList, DateTime effectiveDate, out bool shouldTriggerFullRouteProcess)
        {
            shouldTriggerFullRouteProcess = false;
            if (routeOptionRules != null)
            {
                foreach (RouteOptionRule routeOptionRule in routeOptionRules)
                {
                    RouteOptionRuleCriteria criteria = routeOptionRule.Criteria as RouteOptionRuleCriteria;
                    bool isCustomerGeneric;
                    IEnumerable<int> affectedCustomers = GetAffectedCustomers(criteria, out isCustomerGeneric);

                    IEnumerable<CodeCriteria> affectedCodes;
                    IEnumerable<long> affectedZones;

                    bool areCodesAndZonesGeneric;
                    GetAffectedCodesAndZones(criteria, effectiveDate, out affectedCodes, out affectedZones, out areCodesAndZonesGeneric);

                    bool areSupplierWithZonesGeneric;
                    IEnumerable<SupplierWithZones> supplierWithZones = GetAffectedSupplierZones(criteria, out areSupplierWithZonesGeneric);

                    if (isCustomerGeneric && areCodesAndZonesGeneric && areSupplierWithZonesGeneric)
                    {
                        shouldTriggerFullRouteProcess = true;
                        return;
                    }

                    var routingExcludedDestinationData = (criteria != null && criteria.ExcludedDestinations != null) ? criteria.ExcludedDestinations.GetRoutingExcludedDestinationData() : null;

                    affectedRouteOptionsList.Add(new Entities.AffectedRouteOptions()
                    {
                        Codes = affectedCodes,
                        CustomerIds = affectedCustomers,
                        RoutingExcludedDestinationData = routingExcludedDestinationData,
                        ZoneIds = affectedZones,
                        SupplierWithZones = supplierWithZones
                    });
                }
            }
        }

        private IEnumerable<int> GetAffectedCustomers(RouteRuleCriteria criteria, out bool isCustomerGeneric)
        {
            isCustomerGeneric = false;

            CustomerGroupSettings customerGroupSettings = criteria.GetCustomerGroupSettings();
            if (customerGroupSettings == null)
            {
                isCustomerGeneric = true;
                return null;
            }

            return customerGroupSettings.GetCustomerIds(GetCustomerGroupContext());
        }

        private void GetAffectedCodesAndZones(RouteRuleCriteria criteria, DateTime effectiveDate, out IEnumerable<CodeCriteria> affectedCodes, out IEnumerable<long> affectedZones, out bool areCodesAndZonesGeneric)
        {
            affectedCodes = null;
            affectedZones = null;
            areCodesAndZonesGeneric = false;

            CodeCriteriaGroupSettings codeCriteriaGroupSettings = criteria.GetCodeCriteriaGroupSettings();
            SaleZoneGroupSettings saleZoneGroupSettings = criteria.GetSaleZoneGroupSettings();
            CountryCriteriaGroupSettings countryCriteriaGroupSettings = criteria.GetCountryCriteriaGroupSettings();

            if (codeCriteriaGroupSettings == null && saleZoneGroupSettings == null && countryCriteriaGroupSettings == null)
            {
                areCodesAndZonesGeneric = true;
                return;
            }

            if (codeCriteriaGroupSettings != null)
            {
                affectedCodes = codeCriteriaGroupSettings.GetCodeCriterias(GetCodeCriteriaGroupContext());
            }
            else if (saleZoneGroupSettings != null)
            {
                affectedZones = saleZoneGroupSettings.GetZoneIds(GetSaleZoneGroupContext());
            }
            else if (countryCriteriaGroupSettings != null)
            {
                IEnumerable<int> affectedCountries = countryCriteriaGroupSettings.GetCountryIds(GetCountryCriteriaGroupContext());

                if (affectedCountries != null)
                {
                    List<long> zoneIds = new List<long>();

                    IEnumerable<SellingNumberPlan> sellingNumberPlans = new SellingNumberPlanManager().GetAllSellingNumberPlans();
                    if (sellingNumberPlans != null)
                    {
                        SaleZoneManager saleZoneManager = new SaleZoneManager();
                        foreach (SellingNumberPlan sellingNumberPlan in sellingNumberPlans)
                        {
                            var saleZones = saleZoneManager.GetSaleZonesByCountryIds(sellingNumberPlan.SellingNumberPlanId, affectedCountries, effectiveDate, false);
                            if (saleZones == null)
                                continue;

                            zoneIds.AddRange(saleZones.Select(itm => itm.SaleZoneId));
                        }
                    }
                    affectedZones = zoneIds.Count > 0 ? zoneIds : null;
                }
            }
        }

        private IEnumerable<int> GetAffectedCustomers(RouteOptionRuleCriteria criteria, out bool isCustomerGeneric)
        {
            isCustomerGeneric = false;

            CustomerGroupSettings customerGroupSettings = criteria.CustomerGroupSettings;
            if (customerGroupSettings == null)
            {
                isCustomerGeneric = true;
                return null;
            }

            return customerGroupSettings.GetCustomerIds(GetCustomerGroupContext());
        }

        private void GetAffectedCodesAndZones(RouteOptionRuleCriteria criteria, DateTime effectiveDate, out IEnumerable<CodeCriteria> affectedCodes, out IEnumerable<long> affectedZones, out bool areCodesAndZonesGeneric)
        {
            affectedCodes = null;
            affectedZones = null;
            areCodesAndZonesGeneric = false;

            CodeCriteriaGroupSettings codeCriteriaGroupSettings = criteria.CodeCriteriaGroupSettings;
            SaleZoneGroupSettings saleZoneGroupSettings = criteria.SaleZoneGroupSettings;
            CountryCriteriaGroupSettings countryCriteriaGroupSettings = criteria.CountryCriteriaGroupSettings;

            if (codeCriteriaGroupSettings == null && saleZoneGroupSettings == null && countryCriteriaGroupSettings == null)
            {
                areCodesAndZonesGeneric = true;
                return;
            }

            if (codeCriteriaGroupSettings != null)
            {
                affectedCodes = codeCriteriaGroupSettings.GetCodeCriterias(GetCodeCriteriaGroupContext());
            }
            else if (saleZoneGroupSettings != null)
            {
                affectedZones = saleZoneGroupSettings.GetZoneIds(GetSaleZoneGroupContext());
            }
            else if (countryCriteriaGroupSettings != null)
            {
                IEnumerable<int> affectedCountries = countryCriteriaGroupSettings.GetCountryIds(GetCountryCriteriaGroupContext());

                if (affectedCountries != null)
                {
                    List<long> zoneIds = new List<long>();

                    IEnumerable<SellingNumberPlan> sellingNumberPlans = new SellingNumberPlanManager().GetAllSellingNumberPlans();
                    if (sellingNumberPlans != null)
                    {
                        SaleZoneManager saleZoneManager = new SaleZoneManager();
                        foreach (SellingNumberPlan sellingNumberPlan in sellingNumberPlans)
                        {
                            var saleZones = saleZoneManager.GetSaleZonesByCountryIds(sellingNumberPlan.SellingNumberPlanId, affectedCountries, effectiveDate, false);
                            if (saleZones == null)
                                continue;

                            zoneIds.AddRange(saleZones.Select(itm => itm.SaleZoneId));
                        }
                    }
                    affectedZones = zoneIds.Count > 0 ? zoneIds : null;
                }
            }
        }

        private IEnumerable<SupplierWithZones> GetAffectedSupplierZones(RouteOptionRuleCriteria criteria, out bool areSupplierWithZonesGeneric)
        {
            areSupplierWithZonesGeneric = false;
            SuppliersWithZonesGroupSettings suppliersWithZonesGroupSettings = criteria.SuppliersWithZonesGroupSettings;
            if (suppliersWithZonesGroupSettings == null)
            {
                areSupplierWithZonesGeneric = true;
                return null;
            }
            return suppliersWithZonesGroupSettings.GetSuppliersWithZones(GetSuppliersWithZonesGroupContext());
        }

        ICountryCriteriaGroupContext GetCountryCriteriaGroupContext()
        {
            CountryCriteriaGroupContext countryCriteriaGroupContext = new CountryCriteriaGroupContext();
            return countryCriteriaGroupContext;
        }

        ISaleZoneGroupContext GetSaleZoneGroupContext()
        {
            ISaleZoneGroupContext saleZoneGroupContext = ContextFactory.CreateContext<ISaleZoneGroupContext>();
            return saleZoneGroupContext;
        }

        ICustomerGroupContext GetCustomerGroupContext()
        {
            ICustomerGroupContext customerGroupContext = ContextFactory.CreateContext<ICustomerGroupContext>();
            return customerGroupContext;
        }

        ICodeCriteriaGroupContext GetCodeCriteriaGroupContext()
        {
            ICodeCriteriaGroupContext codeCriteriaGroupContext = ContextFactory.CreateContext<ICodeCriteriaGroupContext>();
            return codeCriteriaGroupContext;
        }

        ISuppliersWithZonesGroupContext GetSuppliersWithZonesGroupContext()
        {
            ISuppliersWithZonesGroupContext suppliersWithZonesGroupContext = ContextFactory.CreateContext<ISuppliersWithZonesGroupContext>();
            return suppliersWithZonesGroupContext;
        }
    }
}