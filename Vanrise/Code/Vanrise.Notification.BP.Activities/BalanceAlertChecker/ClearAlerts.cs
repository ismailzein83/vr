using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Notification.Entities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using Vanrise.Notification.Business;

namespace Vanrise.Notification.BP.Activities.BalanceAlertChecker
{
    public class ClearAlertsInput
    {
        public VRBalanceAlertRuleTypeSettings RuleTypeSettings { get; set; }
        public BaseQueue<VREntityBalanceInfoBatch> InputQueue { get; set; }

        public BaseQueue<VRBalanceUpdateRuleInfoPayloadBatch> OutputQueue { get; set; }


    }
    public sealed class ClearAlerts : DependentAsyncActivity<ClearAlertsInput>
    {
        [RequiredArgument]
        public InArgument<VRBalanceAlertRuleTypeSettings> RuleTypeSettings { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<VREntityBalanceInfoBatch>> InputQueue { get; set; }
        [RequiredArgument]
        public InOutArgument<BaseQueue<VRBalanceUpdateLastAlertInfoPayloadBatch>> OutputQueue { get; set; }
        protected override void DoWork(ClearAlertsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue(
                    (vrEntityBalanceInfoBatch) =>
                    {
                        VRBalanceUpdateLastAlertInfoPayloadBatch balanceUpdateLastAlertInfoPayloadBatch = new VRBalanceUpdateLastAlertInfoPayloadBatch { Items = new List<VRBalanceUpdateLastAlertInfoPayload>() };
                        foreach (var entityBalanceInfo in vrEntityBalanceInfoBatch.BalanceInfos)
                        {
                            VRBalanceAlertRuleCreateRuleTargetContext context = new VRBalanceAlertRuleCreateRuleTargetContext
                            {
                                EntityBalanceInfo = entityBalanceInfo,
                                RuleTypeSettings = inputArgument.RuleTypeSettings
                            };

                            VRAlertRuleManager alertRuleManager = new VRAlertRuleManager();

                            if (!entityBalanceInfo.AlertRuleId.HasValue)
                                throw new NullReferenceException("alertRule");
                            if (!entityBalanceInfo.ThresholdActionIndex.HasValue)
                                throw new NullReferenceException("balanceAlertThresholdAction");

                            var alertRule = alertRuleManager.GetVRAlertRule(entityBalanceInfo.AlertRuleId.Value);

                            VRBalanceAlertRuleSettings balanceAlertRuleSettings = alertRule.Settings.ExtendedSettings as VRBalanceAlertRuleSettings;
                            VRBalanceAlertThresholdAction balanceAlertThresholdAction = balanceAlertRuleSettings.ThresholdActions[entityBalanceInfo.ThresholdActionIndex.Value];

                        }

                    });
                } while (!ShouldStop(handle) && hasItems);
            });
        }

        protected override ClearAlertsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ClearAlertsInput
            {
                InputQueue = this.InputQueue.Get(context),
                RuleTypeSettings = this.RuleTypeSettings.Get(context)
            };
        }
    }
}
