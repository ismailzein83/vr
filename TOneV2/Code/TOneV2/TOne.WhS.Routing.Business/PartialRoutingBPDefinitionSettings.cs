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
            PartialRoutingProcessInput inputArg = context.BaseProcessInputArgument.CastWithValidate<PartialRoutingProcessInput>("context.BaseProcessInputArgument");

            var routeRulesChanged = new RouteRuleManager().GetRulesChanged();
            if (routeRulesChanged != null && routeRulesChanged.Count > 0)
                return true;

            var routeOptionRulesChanged = new RouteOptionRuleManager().GetRulesChanged();
            if (routeOptionRulesChanged != null && routeOptionRulesChanged.Count > 0)
                return true;

            RoutingDatabase routingDatabase = new RoutingDatabaseManager().GetLatestRoutingDatabase(RoutingProcessType.CustomerRoute, RoutingDatabaseType.Current);
            IPartialRouteInfoDataManager partialRouteInfoDataManager = RoutingDataManagerFactory.GetDataManager<IPartialRouteInfoDataManager>();
            partialRouteInfoDataManager.RoutingDatabase = routingDatabase;
            PartialRouteInfo partialRouteInfo = partialRouteInfoDataManager.GetPartialRouteInfo();

            DateTime now = DateTime.Now;
            if (partialRouteInfo != null && partialRouteInfo.NextOpenOrCloseTime.HasValue && partialRouteInfo.NextOpenOrCloseTime < now)
                return true;

            return false;
        }
    }
}
