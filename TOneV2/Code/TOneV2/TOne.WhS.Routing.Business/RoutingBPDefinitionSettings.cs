using System;
using Vanrise.Common;
using TOne.WhS.Routing.BP.Arguments;
using TOne.WhS.RouteSync.BP.Arguments;

namespace TOne.WhS.Routing.Business
{
    public class RoutingBPDefinitionSettings : Vanrise.BusinessProcess.Business.DefaultBPDefinitionExtendedSettings
    {
        public override bool CanRunBPInstance(Vanrise.BusinessProcess.Entities.IBPDefinitionCanRunBPInstanceContext context)
        {
            context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
            RoutingProcessInput routingProcessInputArg = context.IntanceToRun.InputArgument.CastWithValidate<RoutingProcessInput>("context.IntanceToRun.InputArgument");

            foreach (var startedBPInstance in context.GetStartedBPInstances())
            {
                RoutingProcessInput startedBPInstanceRoutingArg = startedBPInstance.InputArgument as RoutingProcessInput;
                if (startedBPInstanceRoutingArg != null && routingProcessInputArg.RoutingDatabaseType == startedBPInstanceRoutingArg.RoutingDatabaseType)
                {
                    context.Reason = "Another Route Build instance of same type is running";
                    return false;
                }

                if (routingProcessInputArg.RoutingDatabaseType == Entities.RoutingDatabaseType.Current)
                {
                    PartialRoutingProcessInput startedBPInstancePartialRoutingArg = startedBPInstance.InputArgument as PartialRoutingProcessInput;
                    if (startedBPInstancePartialRoutingArg != null && (!context.IntanceToRun.ParentProcessID.HasValue || context.IntanceToRun.ParentProcessID.Value != startedBPInstance.ProcessInstanceID))
                    {
                        context.Reason = "Partial Route Build instance is running";
                        return false;
                    }

                    RouteSyncProcessInput startedBPInstanceRouteSyncArg = startedBPInstance.InputArgument as RouteSyncProcessInput;
                    if (startedBPInstanceRouteSyncArg != null)
                    {
                        context.Reason = "Route Sync instance is running";
                        return false;
                    }
                }
            }

            return true;
        }

        public override bool CanCancelBPInstance(Vanrise.BusinessProcess.Entities.IBPDefinitionCanCancelBPInstanceContext context)
        {
            return true;
        }
    }
}
