using TOne.WhS.RouteSync.BP.Arguments;
using TOne.WhS.Routing.BP.Arguments;
using TOne.WhS.TOneV1Transition.BP.Arguments;
using Vanrise.Common;

namespace TOne.WhS.TOneV1Transition.Business
{
    public class MigrateAndRouteBuildBPDefinitionSettings : Vanrise.BusinessProcess.Business.DefaultBPDefinitionExtendedSettings
    {
        public override bool CanRunBPInstance(Vanrise.BusinessProcess.Entities.IBPDefinitionCanRunBPInstanceContext context)
        {
            context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
            MigrateAndRouteBuildInput migrateAndRouteBuildInputArg = context.IntanceToRun.InputArgument.CastWithValidate<MigrateAndRouteBuildInput>("context.IntanceToRun.InputArgument");

            foreach (var startedBPInstance in context.GetStartedBPInstances())
            {
                MigrateAndRouteBuildInput startedMigrateAndRouteBuildArg = startedBPInstance.InputArgument as MigrateAndRouteBuildInput;
                if (startedMigrateAndRouteBuildArg != null)
                {
                    context.Reason = "Another Migrate and Route Build instance is running";
                    return false;
                }

                RoutingProcessInput startedBPInstanceRoutingArg = startedBPInstance.InputArgument as RoutingProcessInput;
                if (startedBPInstanceRoutingArg != null)
                {
                    context.Reason = "Route Build instance is running";
                    return false;
                }

                PartialRoutingProcessInput startedBPInstancePartialRoutingArg = startedBPInstance.InputArgument as PartialRoutingProcessInput;
                if (startedBPInstancePartialRoutingArg != null)
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

            return true;
        }
    }
}