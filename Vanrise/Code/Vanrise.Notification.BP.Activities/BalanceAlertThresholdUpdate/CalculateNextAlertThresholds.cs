using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;
using Vanrise.Queueing;
using Vanrise.Common;
using System.Linq;

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


                                VRBalanceAlertRuleCreateRuleTargetContext vrBalanceAlertRuleCreateRuleTargetContext = new VRBalanceAlertRuleCreateRuleTargetContext
                                {
                                    EntityBalanceInfo = entityBalanceInfo,
                                    RuleTypeSettings = inputArgument.RuleTypeSettings
                                };

                                long? finalAlertRuleId = null;

                                decimal? finalNextThreshold = null;

                                GenericRuleTarget ruleTarget = inputArgument.RuleTypeSettings.Behavior.CreateRuleTarget(vrBalanceAlertRuleCreateRuleTargetContext);
                                VRAlertRule matchedRule = vrBalanceAlertRuleManager.GetMatchRule(inputArgument.AlertTypeId, ruleTarget);                                

                                if (matchedRule != null)
                                {
                                    finalAlertRuleId = matchedRule.VRAlertRuleId;

                                    matchedRule.Settings.ThrowIfNull("matchedRule.Settings", matchedRule.VRAlertRuleId);
                                    VRBalanceAlertRuleSettings balanceAlertRuleSettings = matchedRule.Settings.ExtendedSettings.CastWithValidate<VRBalanceAlertRuleSettings>("matchedRule.Settings.ExtendedSettings", matchedRule.VRAlertRuleId);

                                    List<Decimal> thresholds = new List<Decimal>();
                                    for (int i = 0; i < balanceAlertRuleSettings.ThresholdActions.Count; i++)
                                    {
                                        VRBalanceAlertThresholdAction balanceAlertThresholdAction = balanceAlertRuleSettings.ThresholdActions[i];
                                        VRBalanceAlertThresholdContext vrBalanceAlertThresholdContext = new VRBalanceAlertThresholdContext { EntityBalanceInfo = entityBalanceInfo, AlertRuleTypeId = inputArgument.AlertTypeId };
                                        decimal roundedRuleThresholdValue = CalculateNextAlertThresholds.ThresholdRounding(balanceAlertThresholdAction.Threshold.GetThreshold(vrBalanceAlertThresholdContext));
                                        thresholds.Add(roundedRuleThresholdValue);
                                    }
                                    foreach (var threshold in thresholds.OrderByDescending(itm => itm))
                                    {
                                        if (entityBalanceInfo.LastExecutedAlertThreshold.HasValue && entityBalanceInfo.LastExecutedAlertThreshold.Value <= threshold)
                                            continue;

                                        if(!currentNextThreshold.HasValue)
                                        {
                                            finalNextThreshold = threshold;
                                            break;
                                        }

                                        if (threshold < entityBalanceInfo.CurrentBalance)
                                        {
                                            finalNextThreshold = threshold;
                                            break;
                                        }

                                        if (finalAlertRuleId == currentAlertRuleId && currentNextThreshold.HasValue)
                                        {
                                            if (threshold <= currentNextThreshold.Value)
                                            {
                                                finalNextThreshold = threshold;
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (finalAlertRuleId != currentAlertRuleId || finalNextThreshold != currentNextThreshold)
                                {
                                    totalAccountsUpdated++;
                                    VRBalanceUpdateRuleInfoPayload updateRuleInfoPayload = new VRBalanceUpdateRuleInfoPayload
                                    {
                                        EntityBalanceInfo = entityBalanceInfo,
                                        AlertRuleId = finalAlertRuleId,
                                        NextAlertThreshold = finalNextThreshold
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


        internal static Decimal ThresholdRounding(Decimal threshold)
        {
            return Decimal.Round(threshold, 2);
        }
    }

}
