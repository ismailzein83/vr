using System;
using Vanrise.Common;
using TOne.WhS.Routing.BP.Arguments;

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
    }
}
