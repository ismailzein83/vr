using System;
using Vanrise.Common;
using TOne.WhS.RouteSync.BP.Arguments;
using TOne.WhS.Routing.BP.Arguments;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.RouteSync.Business
{
    public class RouteSyncProcessBPDefinitionSettings : Vanrise.BusinessProcess.Business.DefaultBPDefinitionExtendedSettings
    {
        public override bool CanRunBPInstance(Vanrise.BusinessProcess.Entities.IBPDefinitionCanRunBPInstanceContext context)
        {
            context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
            RouteSyncProcessInput routeSyncProcessInputArg = context.IntanceToRun.InputArgument.CastWithValidate<RouteSyncProcessInput>("context.IntanceToRun.InputArgument");

            foreach (var startedBPInstance in context.GetStartedBPInstances())
            {
                RouteSyncProcessInput startedBPInstanceRouteSyncArg = startedBPInstance.InputArgument as RouteSyncProcessInput;
                if (startedBPInstanceRouteSyncArg != null)
                {
                    context.Reason = "Another Route Sync instance is running";
                    return false;
                }

                RoutingProcessInput startedBPInstanceRoutingArg = startedBPInstance.InputArgument as RoutingProcessInput;
                if (startedBPInstanceRoutingArg != null && startedBPInstanceRoutingArg.RoutingDatabaseType == RoutingDatabaseType.Current)
                {
                    context.Reason = "Route Build instance of type Current is running";
                    return false;
                }
            }

            return true;
        }
    }
}