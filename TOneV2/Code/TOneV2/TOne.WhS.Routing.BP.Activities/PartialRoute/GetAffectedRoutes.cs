using System;
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
        public OutArgument<List<CustomerRoute>> AffectedCustomerRoutes { get; set; }
        [RequiredArgument]
        public OutArgument<bool> ShouldTriggerFullRouteProcess { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var routingDatabase = this.RoutingDatabase.Get(context);

            ICustomerRouteDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICustomerRouteDataManager>();
            dataManager.RoutingDatabase = routingDatabase;

            AffectedRouteRules affectedRouteRules = this.AffectedRouteRules.Get(context);

            bool shouldTriggerFullRouteProcess = false;
            List<AffectedRoutes> affectedRoutesList = new List<AffectedRoutes>();

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

            if (shouldTriggerFullRouteProcess)
            {
                this.ShouldTriggerFullRouteProcess.Set(context, true);
                context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Warning, "Full Route Build will be triggered", null);
                return;
            }

            List<CustomerRoute> affectedCustomerRoutes = null;
            if (affectedRoutesList.Count > 0)
            {
                int customerRouteTotalCount = dataManager.GetTotalCount();
                int partialRoutesPercentageLimit = new ConfigManager().GetPartialRoutesPercentageLimit();
                int partialRoutesNumberLimit = partialRoutesPercentageLimit * customerRouteTotalCount / 100;

                bool maximumExceeded;

                affectedCustomerRoutes = dataManager.GetAffectedCustomerRoutes(affectedRoutesList, partialRoutesNumberLimit, out maximumExceeded);

                if (maximumExceeded)
                {
                    this.ShouldTriggerFullRouteProcess.Set(context, true);
                    context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Warning, "Loading Affected Routes is done. Number of Affected Routes '{0}' exceeds Maximum Number of Routes for Partial Routing '{1}'. Full Route Build will be triggered", affectedCustomerRoutes.Count, partialRoutesNumberLimit);
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
    }
}