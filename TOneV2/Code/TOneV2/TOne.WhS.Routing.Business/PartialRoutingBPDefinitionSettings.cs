using System;
using Vanrise.Common;
using TOne.WhS.Routing.BP.Arguments;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public class PartialRoutingBPDefinitionSettings : Vanrise.BusinessProcess.Business.DefaultBPDefinitionExtendedSettings
    {
        public override bool CanRunBPInstance(Vanrise.BusinessProcess.Entities.IBPDefinitionCanRunBPInstanceContext context)
        {
            context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
            PartialRoutingProcessInput partialRoutingProcessInputArg = context.IntanceToRun.InputArgument.CastWithValidate<PartialRoutingProcessInput>("context.IntanceToRun.InputArgument");

            foreach (var startedBPInstance in context.GetStartedBPInstances())
            {
                PartialRoutingProcessInput startedBPInstancePartialRoutingArg = startedBPInstance.InputArgument as PartialRoutingProcessInput;
                if (startedBPInstancePartialRoutingArg != null)
                {
                    context.Reason = "Another Partial Route Build instance is running";
                    return false;
                }


                RoutingProcessInput startedBPInstanceRoutingArg = startedBPInstance.InputArgument as RoutingProcessInput;
                if (startedBPInstanceRoutingArg != null && startedBPInstanceRoutingArg.RoutingDatabaseType == Entities.RoutingDatabaseType.Current)
                {
                    context.Reason = "Route Build instance of type Current is running";
                    return false;
                }
            }

            return true;
        }

        public override bool ShouldCreateScheduledInstance(Vanrise.BusinessProcess.Entities.IBPDefinitionShouldCreateScheduledInstanceContext context)
        {
            DateTime effectiveDate = DateTime.Now;

            RoutingDatabase routingDatabase = new RoutingDatabaseManager().GetLatestRoutingDatabase(RoutingProcessType.CustomerRoute, RoutingDatabaseType.Current);
            if (routingDatabase == null)
                return false;

            PartialRoutingProcessInput inputArg = context.BaseProcessInputArgument.CastWithValidate<PartialRoutingProcessInput>("context.BaseProcessInputArgument");

            RouteRuleManager routeRuleManager = new RouteRuleManager();
            var routeRulesChangedForProcessing = routeRuleManager.GetRulesChangedForProcessing();
            if (routeRulesChangedForProcessing != null && routeRulesChangedForProcessing.Count > 0)
                return true;

            var routeRulesChanged = routeRuleManager.GetRulesChanged();
            if (routeRulesChanged != null && routeRulesChanged.Count > 0)
                return true;

            RouteOptionRuleManager routeOptionRuleManager = new RouteOptionRuleManager();
            var routeOptionRulesChangedForProcessing = routeOptionRuleManager.GetRulesChangedForProcessing();
            if (routeOptionRulesChangedForProcessing != null && routeOptionRulesChangedForProcessing.Count > 0)
                return true;

            var routeOptionRulesChanged = routeOptionRuleManager.GetRulesChanged();
            if (routeOptionRulesChanged != null && routeOptionRulesChanged.Count > 0)
                return true;

            IRoutingEntityDetailsDataManager routingEntityDetailsDataManager = RoutingDataManagerFactory.GetDataManager<IRoutingEntityDetailsDataManager>();
            routingEntityDetailsDataManager.RoutingDatabase = routingDatabase;

            RoutingEntityDetails routingEntityDetails = routingEntityDetailsDataManager.GetRoutingEntityDetails(RoutingEntityType.PartialRouteInfo);
            routingEntityDetails.ThrowIfNull("routingEntityDetails");

            PartialRouteInfo partialRouteInfo = routingEntityDetails.RoutingEntityInfo.CastWithValidate<PartialRouteInfo>("partialRouteInfo");
            if (partialRouteInfo.NextOpenOrCloseRuleTime.HasValue && partialRouteInfo.NextOpenOrCloseRuleTime < effectiveDate)
                return true;

            RoutingEntityDetails beRoutingEntityDetails = routingEntityDetailsDataManager.GetRoutingEntityDetails(RoutingEntityType.BERouteInfo);
            beRoutingEntityDetails.ThrowIfNull("beRoutingEntityDetails");

            BERouteInfo beRouteInfo = beRoutingEntityDetails.RoutingEntityInfo.CastWithValidate<BERouteInfo>("beRouteInfo");
            beRouteInfo.ThrowIfNull("beRouteInfo");
            beRouteInfo.SaleRateRouteInfo.ThrowIfNull("beRouteInfo.SaleRateRouteInfo");
            beRouteInfo.SupplierRateRouteInfo.ThrowIfNull("beRouteInfo.SupplierRateRouteInfo");

            if (beRouteInfo.SaleRateRouteInfo.LatestVersionNumber > partialRouteInfo.LatestSaleRateVersionNumber ||
                beRouteInfo.SupplierRateRouteInfo.LatestVersionNumber > partialRouteInfo.LatestCostRateVersionNumber)
                return true;

            return false;
        }
    }
}
