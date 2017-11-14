﻿using System;
using System.Activities;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    public sealed class GetAffectedRoutes : CodeActivity
    {
        [RequiredArgument]
        public InArgument<RoutingDatabase> RoutingDatabase { get; set; }
        [RequiredArgument]
        public InArgument<AffectedRouteRules> AffectedRouteRules { get; set; }
        [RequiredArgument]
        public InArgument<AffectedRouteOptionRules> AffectedRouteOptionRules { get; set; }
        [RequiredArgument]
        public OutArgument<HashSet<CustomerRouteDefinition>> AffectedCustomerRoutes { get; set; }
        [RequiredArgument]
        public OutArgument<bool> ShouldTriggerFullRouteProcess { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var routingDatabase = this.RoutingDatabase.Get(context);

            ICustomerRouteDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICustomerRouteDataManager>();
            dataManager.RoutingDatabase = routingDatabase;

            AffectedRouteRules affectedRouteRules = this.AffectedRouteRules.Get(context);
            AffectedRouteOptionRules affectedRouteOptionRules = this.AffectedRouteOptionRules.Get(context);

            bool shouldTriggerFullRouteProcess = false;
            List<AffectedRoutes> affectedRoutesList = new List<AffectedRoutes>();
            List<AffectedRouteOptions> affectedRouteOptionsList = new List<AffectedRouteOptions>();

            if (affectedRouteRules != null)
            {
                BuildAffectedRoutes(affectedRouteRules.AddedRouteRules, affectedRoutesList, out shouldTriggerFullRouteProcess);

                if (!shouldTriggerFullRouteProcess)
                    BuildAffectedRoutes(affectedRouteRules.UpdatedRouteRules, affectedRoutesList, out shouldTriggerFullRouteProcess);

                if (!shouldTriggerFullRouteProcess)
                    BuildAffectedRoutes(affectedRouteRules.OpenedRouteRules, affectedRoutesList, out shouldTriggerFullRouteProcess);

                if (!shouldTriggerFullRouteProcess)
                    BuildAffectedRoutes(affectedRouteRules.ClosedRouteRules, affectedRoutesList, out shouldTriggerFullRouteProcess);
            }

            if (!shouldTriggerFullRouteProcess && affectedRouteOptionRules != null)
            {
                BuildAffectedRouteOptions(affectedRouteOptionRules.AddedRouteOptionRules, affectedRouteOptionsList, out shouldTriggerFullRouteProcess);

                if (!shouldTriggerFullRouteProcess)
                    BuildAffectedRouteOptions(affectedRouteOptionRules.UpdatedRouteOptionRules, affectedRouteOptionsList, out shouldTriggerFullRouteProcess);

                if (!shouldTriggerFullRouteProcess)
                    BuildAffectedRouteOptions(affectedRouteOptionRules.OpenedRouteOptionRules, affectedRouteOptionsList, out shouldTriggerFullRouteProcess);

                if (!shouldTriggerFullRouteProcess)
                    BuildAffectedRouteOptions(affectedRouteOptionRules.ClosedRouteOptionRules, affectedRouteOptionsList, out shouldTriggerFullRouteProcess);
            }

            if (shouldTriggerFullRouteProcess)
            {
                this.ShouldTriggerFullRouteProcess.Set(context, true);
                context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Warning, "Full Route Build will be triggered", null);
                return;
            }

            HashSet<CustomerRouteDefinition> affectedCustomerRoutes = null;
            if (affectedRoutesList.Count > 0 || affectedRouteOptionsList.Count > 0)
            {
                long customerRouteTotalCount = dataManager.GetTotalCount();
                int partialRoutesPercentageLimit = new ConfigManager().GetPartialRoutesPercentageLimit();
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

        private void BuildAffectedRouteOptions(List<RouteOptionRule> routeOptionRules, List<AffectedRouteOptions> affectedRouteOptionsList, out bool shouldTriggerFullRouteProcess)
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
                    GetAffectedCodesAndZones(criteria, out affectedCodes, out affectedZones, out areCodesAndZonesGeneric);

                    bool areSupplierWithZonesGeneric;
                    IEnumerable<SupplierWithZones> supplierWithZones = GetAffectedSupplierZones(criteria, out areSupplierWithZonesGeneric);

                    if (isCustomerGeneric && areCodesAndZonesGeneric && areSupplierWithZonesGeneric)
                    {
                        shouldTriggerFullRouteProcess = true;
                        return;
                    }

                    AffectedRouteOptions affectedRouteOptions = new Entities.AffectedRouteOptions() { Codes = affectedCodes, CustomerIds = affectedCustomers, ExcludedCodes = criteria.ExcludedCodes, ZoneIds = affectedZones, SupplierWithZones = supplierWithZones };
                    affectedRouteOptionsList.Add(affectedRouteOptions);
                }
            }
        }

        private void BuildAffectedRoutes(List<RouteRule> routeRules, List<AffectedRoutes> affectedRoutesList, out bool shouldTriggerFullRouteProcess)
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
                    GetAffectedCodesAndZones(criteria, out affectedCodes, out affectedZones, out areCodesAndZonesGeneric);

                    if (isCustomerGeneric && areCodesAndZonesGeneric)
                    {
                        shouldTriggerFullRouteProcess = true;
                        return;
                    }
                    AffectedRoutes affectedRoutes = new Entities.AffectedRoutes() { Codes = affectedCodes, CustomerIds = affectedCustomers, ExcludedCodes = criteria.ExcludedCodes, ZoneIds = affectedZones };
                    affectedRoutesList.Add(affectedRoutes);
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

        private void GetAffectedCodesAndZones(RouteRuleCriteria criteria, out IEnumerable<CodeCriteria> affectedCodes, out IEnumerable<long> affectedZones, out bool areCodesAndZonesGeneric)
        {
            affectedCodes = null;
            affectedZones = null;
            areCodesAndZonesGeneric = false;

            CodeCriteriaGroupSettings codeCriteriaGroupSettings = criteria.GetCodeCriteriaGroupSettings();
            SaleZoneGroupSettings saleZoneGroupSettings = criteria.GetSaleZoneGroupSettings();

            if (codeCriteriaGroupSettings == null && saleZoneGroupSettings == null)
            {
                areCodesAndZonesGeneric = true;
                return;
            }

            if (codeCriteriaGroupSettings != null)
            {
                affectedCodes = codeCriteriaGroupSettings.GetCodeCriterias(GetCodeCriteriaGroupContext());
            }
            else
            {
                affectedZones = saleZoneGroupSettings.GetZoneIds(GetSaleZoneGroupContext());
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

        private void GetAffectedCodesAndZones(RouteOptionRuleCriteria criteria, out IEnumerable<CodeCriteria> affectedCodes, out IEnumerable<long> affectedZones, out bool areCodesAndZonesGeneric)
        {
            affectedCodes = null;
            affectedZones = null;
            areCodesAndZonesGeneric = false;

            CodeCriteriaGroupSettings codeCriteriaGroupSettings = criteria.CodeCriteriaGroupSettings;
            SaleZoneGroupSettings saleZoneGroupSettings = criteria.SaleZoneGroupSettings;

            if (codeCriteriaGroupSettings == null && saleZoneGroupSettings == null)
            {
                areCodesAndZonesGeneric = true;
                return;
            }

            if (codeCriteriaGroupSettings != null)
            {
                affectedCodes = codeCriteriaGroupSettings.GetCodeCriterias(GetCodeCriteriaGroupContext());
            }
            else
            {
                affectedZones = saleZoneGroupSettings.GetZoneIds(GetSaleZoneGroupContext());
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