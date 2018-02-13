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
        public override bool CanRunBPInstance(IBPDefinitionCanRunBPInstanceContext context)
        {
            context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
            DAProfCalcGenerateAlertInput daProfCalcGenerateAlertInput = context.IntanceToRun.InputArgument.CastWithValidate<DAProfCalcGenerateAlertInput>("context.IntanceToRun.InputArgument");
            daProfCalcGenerateAlertInput.MaxDAProfCalcAnalysisPeriod.ThrowIfNull("daProfCalcGenerateAlertInput.MaxDAProfCalcAnalysisPeriod");

            int maxDAProfCalcAnalysisPeriod = daProfCalcGenerateAlertInput.MaxDAProfCalcAnalysisPeriod.GetPeriodInMinutes();
            int minDAProfCalcAnalysisPeriod = daProfCalcGenerateAlertInput.MinDAProfCalcAnalysisPeriod != null ? daProfCalcGenerateAlertInput.MinDAProfCalcAnalysisPeriod.GetPeriodInMinutes() : 0;

            foreach (var startedBPInstance in context.GetStartedBPInstances())
            {
                DAProfCalcGenerateAlertInput startedBPInstanceDAProfCalcGenerateAlertInputArg = startedBPInstance.InputArgument as DAProfCalcGenerateAlertInput;
                if (startedBPInstanceDAProfCalcGenerateAlertInputArg != null && daProfCalcGenerateAlertInput.AlertRuleTypeId == startedBPInstanceDAProfCalcGenerateAlertInputArg.AlertRuleTypeId)
                {
                    int maxDAProfCalcAnalysisPeriodStartedInstance = startedBPInstanceDAProfCalcGenerateAlertInputArg.MaxDAProfCalcAnalysisPeriod.GetPeriodInMinutes();
                    int minDAProfCalcAnalysisPeriodStartedInstance = startedBPInstanceDAProfCalcGenerateAlertInputArg.MinDAProfCalcAnalysisPeriod != null ? startedBPInstanceDAProfCalcGenerateAlertInputArg.MinDAProfCalcAnalysisPeriod.GetPeriodInMinutes() : 0;

                    if (maxDAProfCalcAnalysisPeriod > minDAProfCalcAnalysisPeriodStartedInstance && maxDAProfCalcAnalysisPeriodStartedInstance > minDAProfCalcAnalysisPeriod)
                    {
                        context.Reason = "Another Data Analysis Profiling and Calculation Generate Alert instance of the same rule type is running during an overlapped analysis period";
                        return false;
                    }
                }
            }

            return true;
        }
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
