using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.BP.Arguments;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Security.Entities;

namespace Vanrise.BEBridge.Business
{
    public class BEReceiveDefinitionBPExtentedSettings : BPDefinitionExtendedSettings
    {
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
