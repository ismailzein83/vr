using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.BP.Arguments;
using Vanrise.BEBridge.Entities;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Security.Entities;

namespace Vanrise.BEBridge.Business
{
    public class BEReceiveDefinitionBPExtentedSettings : BPDefinitionExtendedSettings
    {
        public override bool CanRunBPInstance(IBPDefinitionCanRunBPInstanceContext context)
        {
            context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
            SourceBESyncProcessInput beBridgeProcessInput = context.IntanceToRun.InputArgument.CastWithValidate<SourceBESyncProcessInput>("context.IntanceToRun.InputArgument");
            foreach (var startedInstance in context.GetStartedBPInstances())
            {
                SourceBESyncProcessInput processInput = startedInstance.InputArgument.CastWithValidate<SourceBESyncProcessInput>("context.IntanceToRun.InputArgument");
                if (processInput != null)
                {
                    foreach (var beReceiveDefinitionId in processInput.BEReceiveDefinitionIds)
                    {
                        if (beBridgeProcessInput.BEReceiveDefinitionIds.Contains(beReceiveDefinitionId))
                        {
                            BEReceiveDefinition beReceiveDefinition = new BEReceiveDefinitionManager().GetBEReceiveDefinition(beReceiveDefinitionId);
                            context.Reason = String.Format("Another {0} instance is running", beReceiveDefinition.Name);
                            return false;
                        }
                    }
                }
            }

            return true;
        }
        public override RequiredPermissionSettings GetViewInstanceRequiredPermissions(IBPDefinitionGetViewInstanceRequiredPermissionsContext context)
        {
            var sourceBESyncProcessInput = context.InputArg.CastWithValidate<SourceBESyncProcessInput>("context.InputArg");
            return new BEReceiveDefinitionManager().GetViewInstanceRequiredPermissions(sourceBESyncProcessInput.BEReceiveDefinitionIds);
        }

        public override bool DoesUserHaveViewAccess(IBPDefinitionDoesUserHaveViewAccessContext context)
        {
            return new BEReceiveDefinitionManager().DoesUserHaveViewAccess(context.UserId);
        }

        public override bool DoesUserHaveStartAccess(IBPDefinitionDoesUserHaveStartAccessContext context)
        {
            return new BEReceiveDefinitionManager().DoesUserHaveStartNewInstanceAccess(context.UserId);
        }

        public override bool DoesUserHaveScheduleTaskAccess(IBPDefinitionDoesUserHaveScheduleTaskContext context)
        {
            return new BEReceiveDefinitionManager().DoesUserHaveStartNewInstanceAccess(context.UserId);
        }
        public override bool DoesUserHaveStartSpecificInstanceAccess(IBPDefinitionDoesUserHaveStartSpecificInstanceAccessContext context)
        {
            var sourceBESyncProcessInput = context.InputArg.CastWithValidate<SourceBESyncProcessInput>("context.InputArg");
            return new BEReceiveDefinitionManager().DoesUserHaveStartSpecificInstanceAccess(context.DefinitionContext.UserId, sourceBESyncProcessInput.BEReceiveDefinitionIds);
        }

        public override bool DoesUserHaveScheduleSpecificTaskAccess(IBPDefinitionDoesUserHaveScheduleSpecificTaskAccessContext context)
        {
            var sourceBESyncProcessInput = context.InputArg.CastWithValidate<SourceBESyncProcessInput>("context.InputArg");
            return new BEReceiveDefinitionManager().DoesUserHaveStartSpecificInstanceAccess(context.DefinitionContext.UserId, sourceBESyncProcessInput.BEReceiveDefinitionIds);
        }
    }
}
