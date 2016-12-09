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

namespace Vanrise.Notification.BP.Activities.BalanceAlertChecker
{
    public class CreateAlertsInput
    {
        public VRBalanceAlertRuleTypeSettings RuleTypeSettings { get; set; }
        public Guid AlertTypeId { get; set; }
        public BaseQueue<VREntityBalanceInfoBatch> InputQueue { get; set; }
        public BaseQueue<VRBalanceUpdateLastAlertInfoPayloadBatch> OutputQueue { get; set; }
    }
    public sealed class CreateAlerts : DependentAsyncActivity<CreateAlertsInput>
    {
        [RequiredArgument]
        public InArgument<VRBalanceAlertRuleTypeSettings> RuleTypeSettings { get; set; }
        [RequiredArgument]
        public InArgument<Guid> AlertTypeId { get; set; }
        [RequiredArgument]
        public InArgument<BaseQueue<VREntityBalanceInfoBatch>> InputQueue { get; set; }
        [RequiredArgument]
        public InOutArgument<BaseQueue<VRBalanceUpdateLastAlertInfoPayloadBatch>> OutputQueue { get; set; }

        protected override void DoWork(CreateAlertsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
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

                                VRBalanceAlertThresholdContext vrBalanceAlertThresholdContext = new VRBalanceAlertThresholdContext { EntityBalanceInfo = entityBalanceInfo };
                                var lastExecutedThreshold = balanceAlertThresholdAction.Threshold.GetThreshold(vrBalanceAlertThresholdContext);
                                CreateAlertRuleNotificationInput createAlertRuleNotification = new CreateAlertRuleNotificationInput
                                {
                                    Actions = balanceAlertThresholdAction.Actions,
                                    ClearanceActions = balanceAlertThresholdAction.RollbackActions,
                                    AlertRuleId = alertRule.VRAlertRuleId,
                                    EventKey = string.Format("{0}_{1}", entityBalanceInfo.EntityId, lastExecutedThreshold),
                                    EventPayload = new VRBalanceAlertEventPayload
                                    {
                                        AlertRuleTypeId = inputArgument.AlertTypeId,
                                        CurrentBalance = entityBalanceInfo.CurrentBalance,
                                        EntityId = entityBalanceInfo.EntityId,
                                        Threshold = lastExecutedThreshold
                                    },
                                    AlertRuleTypeId = inputArgument.AlertTypeId,
                                    UserId = handle.SharedInstanceData.InstanceInfo.InitiatorUserId,
                                    Description = ""
                                };

                                VRAlertRuleNotificationManager alertRuleNotificationManager = new VRAlertRuleNotificationManager();
                                alertRuleNotificationManager.CreateNotification(createAlertRuleNotification);
                                balanceUpdateLastAlertInfoPayloadBatch.Items.Add(new VRBalanceUpdateLastAlertInfoPayload
                                {
                                    EntityBalanceInfo = entityBalanceInfo,
                                    LastExecutedAlertThreshold = lastExecutedThreshold
                                });
                            }
                            inputArgument.OutputQueue.Enqueue(balanceUpdateLastAlertInfoPayloadBatch);
                        });
                    } while (!ShouldStop(handle) && hasItems);
                });
        }

        protected override CreateAlertsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new CreateAlertsInput
            {
                InputQueue = this.InputQueue.Get(context),
                AlertTypeId = this.AlertTypeId.Get(context),
                RuleTypeSettings = this.RuleTypeSettings.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<VRBalanceUpdateLastAlertInfoPayloadBatch>());
            base.OnBeforeExecute(context, handle);
        }
    }
}
