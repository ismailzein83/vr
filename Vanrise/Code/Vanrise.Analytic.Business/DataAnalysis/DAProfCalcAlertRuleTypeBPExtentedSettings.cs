using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.BP.Arguments;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Security.Entities;

namespace Vanrise.Analytic.Business
{
    public class DAProfCalcAlertRuleTypeBPExtentedSettings : BPDefinitionExtendedSettings
    {
        public override RequiredPermissionSettings GetViewInstanceRequiredPermissions(IBPDefinitionGetViewInstanceRequiredPermissionsContext context)
        {
            var daprofCalcGenerateAlertInput = context.InputArg.CastWithValidate<DAProfCalcGenerateAlertInput>("context.InputArg");
            return new DAProfCalcNotificationManager().GetViewInstanceRequiredPermissions(daprofCalcGenerateAlertInput.AlertRuleTypeId);
        }

        public override bool DoesUserHaveViewAccess(IBPDefinitionDoesUserHaveViewAccessContext context)
        {
            return new DAProfCalcNotificationManager().DoesUserHaveViewAccess(context.UserId);
        }

        public override bool DoesUserHaveStartAccess(IBPDefinitionDoesUserHaveStartAccessContext context)
        {
            return new DAProfCalcNotificationManager().DoesUserHaveStartNewInstanceAccess(context.UserId);
        }

        public override bool DoesUserHaveScheduleTaskAccess(IBPDefinitionDoesUserHaveScheduleTaskContext context)
        {
            return new DAProfCalcNotificationManager().DoesUserHaveStartNewInstanceAccess(context.UserId);
        }
        public override bool DoesUserHaveStartSpecificInstanceAccess(IBPDefinitionDoesUserHaveStartSpecificInstanceAccessContext context)
        {
            var daprofCalcGenerateAlertInput = context.InputArg.CastWithValidate<DAProfCalcGenerateAlertInput>("context.InputArg");
            return new DAProfCalcNotificationManager().DoesUserHaveStartSpecificInstanceAccess(context.DefinitionContext.UserId, daprofCalcGenerateAlertInput.AlertRuleTypeId);
        }

        public override bool DoesUserHaveScheduleSpecificTaskAccess(IBPDefinitionDoesUserHaveScheduleSpecificTaskAccessContext context)
        {
            var daprofCalcGenerateAlertInput = context.InputArg.CastWithValidate<DAProfCalcGenerateAlertInput>("context.InputArg");
            return new DAProfCalcNotificationManager().DoesUserHaveStartSpecificInstanceAccess(context.DefinitionContext.UserId, daprofCalcGenerateAlertInput.AlertRuleTypeId);
        }
    }
}
