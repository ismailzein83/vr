using System;
using Vanrise.Common;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Notification.Entities;
using Vanrise.Notification.BP.Arguments;

namespace Vanrise.Notification.Business
{
    public class BalanceAlertThresholdUpdateBPDefinitionSettings : DefaultBPDefinitionExtendedSettings
    {
        public override bool CanRunBPInstance(IBPDefinitionCanRunBPInstanceContext context)
        {
            context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
            BalanceAlertThresholdUpdateProcessInput balanceAlertThresholdUpdateProcessInput = context.IntanceToRun.InputArgument.CastWithValidate<BalanceAlertThresholdUpdateProcessInput>("context.IntanceToRun.InputArgument");

            foreach (var startedBPInstance in context.GetStartedBPInstances())
            {
                BalanceAlertThresholdUpdateProcessInput startedBPInstancebalanceAlertThresholdUpdateArg = startedBPInstance.InputArgument as BalanceAlertThresholdUpdateProcessInput;
                if (startedBPInstancebalanceAlertThresholdUpdateArg != null
                    && balanceAlertThresholdUpdateProcessInput.AlertRuleTypeId == startedBPInstancebalanceAlertThresholdUpdateArg.AlertRuleTypeId)
                {
                    context.Reason = "Another Balance Alert Threshold Update instance for same Rule Type is running";
                    return false;
                }
            }

            return true;
        }
    }
}