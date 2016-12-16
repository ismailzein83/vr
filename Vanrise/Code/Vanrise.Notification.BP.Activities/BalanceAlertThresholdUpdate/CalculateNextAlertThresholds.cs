using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Notification.Entities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using Vanrise.GenericData.Entities;
using Vanrise.Notification.Business;
using Vanrise.Entities;

namespace Vanrise.Notification.BP.Activities.BalanceAlertThresholdUpdate
{
    public class CalculateNextAlertThresholdsInput
    {
        public VRBalanceAlertRuleTypeSettings RuleTypeSettings { get; set; }
        public Guid AlertTypeId { get; set; }
        public BaseQueue<VREntityBalanceInfoBatch> InputQueue { get; set; }
        public BaseQueue<VRBalanceUpdateRuleInfoPayloadBatch> OutputQueue { get; set; }
    }

    public sealed class CalculateNextAlertThresholds : DependentAsyncActivity<CalculateNextAlertThresholdsInput>
    {
        [RequiredArgument]
        public InArgument<VRBalanceAlertRuleTypeSettings> RuleTypeSettings { get; set; }
        [RequiredArgument]
        public InArgument<Guid> AlertTypeId { get; set; }
        [RequiredArgument]
        public InArgument<BaseQueue<VREntityBalanceInfoBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<VRBalanceUpdateRuleInfoPayloadBatch>> OutputQueue { get; set; }

        protected override void DoWork(CalculateNextAlertThresholdsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            int totalAccountsUpdated = 0;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue(
                        (vrEntityBalanceInfoBatch) =>
                        {
                            VRBalanceUpdateRuleInfoPayloadBatch updateRuleInfoPayloadBatch = new VRBalanceUpdateRuleInfoPayloadBatch();
                            VRBalanceAlertRuleManager vrBalanceAlertRuleManager = new VRBalanceAlertRuleManager();
                            foreach (IVREntityBalanceInfo entityBalanceInfo in vrEntityBalanceInfoBatch.BalanceInfos)
                            {
                                decimal? currentNextThreshold = entityBalanceInfo.NextAlertThreshold;
                                int? currentAlertRuleId = entityBalanceInfo.AlertRuleId;

                                decimal? finalNextThreshold = default(decimal?);
                                int? thresholdIndex = default(int?);
                                long? finalAlertRuleId;


                                VRBalanceAlertRuleCreateRuleTargetContext vrBalanceAlertRuleCreateRuleTargetContext = new VRBalanceAlertRuleCreateRuleTargetContext
                                {
                                    EntityBalanceInfo = entityBalanceInfo,
                                    RuleTypeSettings = inputArgument.RuleTypeSettings
                                };

                                GenericRuleTarget ruleTareget = inputArgument.RuleTypeSettings.Behavior.CreateRuleTarget(vrBalanceAlertRuleCreateRuleTargetContext);
                                VRAlertRule matchedRule = vrBalanceAlertRuleManager.GetMatchRule(inputArgument.AlertTypeId, ruleTareget);

                                finalAlertRuleId = matchedRule != null ? (long?)matchedRule.VRAlertRuleId : null;

                                if (matchedRule != null)
                                {
                                    VRBalanceAlertRuleSettings balanceAlertRuleSettings = matchedRule.Settings.ExtendedSettings as VRBalanceAlertRuleSettings;

                                    for (int i = 0; i < balanceAlertRuleSettings.ThresholdActions.Count; i++)
                                    {
                                        VRBalanceAlertThresholdAction balanceAlertThresholdAction = balanceAlertRuleSettings.ThresholdActions[i];
                                        VRBalanceAlertThresholdContext vrBalanceAlertThresholdContext = new VRBalanceAlertThresholdContext { EntityBalanceInfo = entityBalanceInfo };
                                        decimal ruleThresholdValue = balanceAlertThresholdAction.Threshold.GetThreshold(vrBalanceAlertThresholdContext);
                                        if (currentNextThreshold != ruleThresholdValue  && entityBalanceInfo.CurrentBalance < ruleThresholdValue)
                                        {
                                            finalNextThreshold = ruleThresholdValue;
                                            thresholdIndex = i;
                                            break;
                                        }
                                    }
                                }

                                if (finalAlertRuleId != currentAlertRuleId || finalNextThreshold != currentNextThreshold)
                                {
                                    totalAccountsUpdated++;
                                    VRBalanceUpdateRuleInfoPayload updateRuleInfoPayload = new VRBalanceUpdateRuleInfoPayload
                                    {
                                        AlertRuleId = finalAlertRuleId,
                                        EntityBalanceInfo = entityBalanceInfo,
                                        NextAlertThreshold = finalNextThreshold,
                                        ThresholdActionIndex = thresholdIndex
                                    };
                                    updateRuleInfoPayloadBatch.Items.Add(updateRuleInfoPayload);
                                }
                            }
                            if (updateRuleInfoPayloadBatch.Items.Count > 0)
                                inputArgument.OutputQueue.Enqueue(updateRuleInfoPayloadBatch);
                        });
                } while (!ShouldStop(handle) && hasItems);
            });
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "{0} total Balance Entity threshold update.", totalAccountsUpdated);
        }

        protected override CalculateNextAlertThresholdsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new CalculateNextAlertThresholdsInput
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
                RuleTypeSettings = this.RuleTypeSettings.Get(context),
                AlertTypeId = this.AlertTypeId.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<VRBalanceUpdateRuleInfoPayloadBatch>());
            base.OnBeforeExecute(context, handle);
        }

    }

}
