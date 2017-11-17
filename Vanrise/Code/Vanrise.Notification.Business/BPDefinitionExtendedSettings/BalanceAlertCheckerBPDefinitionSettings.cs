using System;
using Vanrise.Common;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Notification.Entities;
using Vanrise.Notification.BP.Arguments;

namespace Vanrise.Notification.Business
{
    public class BalanceAlertCheckerBPDefinitionSettings : DefaultBPDefinitionExtendedSettings
    {
        public override bool CanRunBPInstance(IBPDefinitionCanRunBPInstanceContext context)
        {
            context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
            BalanceAlertCheckerProcessInput balanceAlertCheckerProcessInput = context.IntanceToRun.InputArgument.CastWithValidate<BalanceAlertCheckerProcessInput>("context.IntanceToRun.InputArgument");

            foreach (var startedBPInstance in context.GetStartedBPInstances())
            {
                BalanceAlertCheckerProcessInput startedBPInstanceBalanceAlertCheckerInputArg = startedBPInstance.InputArgument as BalanceAlertCheckerProcessInput;
                if (startedBPInstanceBalanceAlertCheckerInputArg != null
                    && balanceAlertCheckerProcessInput.AlertRuleTypeId == startedBPInstanceBalanceAlertCheckerInputArg.AlertRuleTypeId)
                {
                    context.Reason = "Another Balance Alert Checker instance for same Rule Type is running";
                    return false;
                }
            }

            return true;
        }

        public override bool ShouldCreateScheduledInstance(IBPDefinitionShouldCreateScheduledInstanceContext context)
        {
            BalanceAlertCheckerProcessInput balanceAlertCheckerProcessInput = context.BaseProcessInputArgument.CastWithValidate<BalanceAlertCheckerProcessInput>("context.IntanceToRun.InputArgument");
            VRBalanceAlertRuleTypeSettings vrBalanceAlertRuleTypeSettings = new VRAlertRuleTypeManager().GetVRAlertRuleTypeSettings<VRBalanceAlertRuleTypeSettings>(balanceAlertCheckerProcessInput.AlertRuleTypeId);

            VRBalanceAlertRuleHasLiveBalancesUpdateDataContext vrBalanceAlertRuleHasLiveBalancesUpdateDataContext = new Entities.VRBalanceAlertRuleHasLiveBalancesUpdateDataContext()
            {
                RuleTypeSettings = vrBalanceAlertRuleTypeSettings
            };

            bool hasLiveBalancesUpdateData = vrBalanceAlertRuleTypeSettings.Behavior.HasLiveBalancesUpdateData(vrBalanceAlertRuleHasLiveBalancesUpdateDataContext);
            if (hasLiveBalancesUpdateData)
                return true;

            return false;
        }
    }
}