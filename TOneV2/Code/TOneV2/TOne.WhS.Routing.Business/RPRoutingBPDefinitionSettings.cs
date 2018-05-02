using System;
using Vanrise.Common;
using TOne.WhS.Routing.BP.Arguments;
using TOne.WhS.RouteSync.BP.Arguments;

namespace TOne.WhS.Routing.Business
{
    public class RPRoutingBPDefinitionSettings : Vanrise.BusinessProcess.Business.DefaultBPDefinitionExtendedSettings
    {
        public override bool CanRunBPInstance(Vanrise.BusinessProcess.Entities.IBPDefinitionCanRunBPInstanceContext context)
        {
            context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
            RPRoutingProcessInput routingProcessInputArg = context.IntanceToRun.InputArgument.CastWithValidate<RPRoutingProcessInput>("context.IntanceToRun.InputArgument");

            foreach (var startedBPInstance in context.GetStartedBPInstances())
            {
                RPRoutingProcessInput startedBPInstanceRPRoutingArg = startedBPInstance.InputArgument as RPRoutingProcessInput;
                if (startedBPInstanceRPRoutingArg != null && routingProcessInputArg.RoutingDatabaseType == startedBPInstanceRPRoutingArg.RoutingDatabaseType)
                {
                    context.Reason = "Another Product Cost Generation instance of same type is running";
                    return false;
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
