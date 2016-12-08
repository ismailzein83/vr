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
    }
    public sealed class CreateAlerts : DependentAsyncActivity<CreateAlertsInput>
    {
        [RequiredArgument]
        public InArgument<VRBalanceAlertRuleTypeSettings> RuleTypeSettings { get; set; }
        [RequiredArgument]
        public InArgument<Guid> AlertTypeId { get; set; }
        [RequiredArgument]
        public InArgument<BaseQueue<VREntityBalanceInfoBatch>> InputQueue { get; set; }

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

                            foreach (var entityBalanceInfo in vrEntityBalanceInfoBatch.BalanceInfos)
                            {
                                VRBalanceAlertRuleCreateRuleTargetContext context = new VRBalanceAlertRuleCreateRuleTargetContext
                                {
                                    EntityBalanceInfo = entityBalanceInfo,
                                    RuleTypeSettings = inputArgument.RuleTypeSettings
                                };

                                VRAlertRuleManager alertRuleManager = new VRAlertRuleManager();
                                var alertRule = entityBalanceInfo.AlertRuleId.HasValue ? alertRuleManager.GetVRAlertRule(entityBalanceInfo.AlertRuleId.Value) : null;

                                if (alertRule == null)
                                    throw new NullReferenceException("alertRule");

                                VRBalanceAlertRuleSettings balanceAlertRuleSettings = alertRule.Settings.ExtendedSettings as VRBalanceAlertRuleSettings;
                                VRBalanceAlertThresholdAction balanceAlertThresholdAction = entityBalanceInfo.ThresholdActionIndex.HasValue ? balanceAlertRuleSettings.ThresholdActions[entityBalanceInfo.ThresholdActionIndex.Value] : null;

                                if (balanceAlertThresholdAction == null)
                                    throw new NullReferenceException("balanceAlertThresholdAction");

                                VRBalanceAlertThresholdContext vrBalanceAlertThresholdContext = new VRBalanceAlertThresholdContext { EntityBalanceInfo = entityBalanceInfo };
                                CreateAlertRuleNotificationInput notificationInput = new CreateAlertRuleNotificationInput
                                {
                                    Actions = balanceAlertThresholdAction.Actions,
                                    ClearanceActions = balanceAlertThresholdAction.RollbackActions,
                                    AlertRuleId = alertRule.VRAlertRuleId,
                                    EventKey = entityBalanceInfo.EntityId,
                                    EventPayload = new VRBalanceAlertEventPayload
                                    {
                                        AlertRuleTypeId = inputArgument.AlertTypeId,
                                        CurrentBalance = entityBalanceInfo.CurrentBalance,
                                        EntityId = entityBalanceInfo.EntityId,
                                        Threshold = balanceAlertThresholdAction.Threshold.GetThreshold(vrBalanceAlertThresholdContext)
                                    },
                                    AlertRuleTypeId = inputArgument.AlertTypeId,
                                    UserId = handle.SharedInstanceData.InstanceInfo.InitiatorUserId,
                                    Description = ""
                                };
                                VRAlertRuleNotificationManager notificationManager = new VRAlertRuleNotificationManager();
                                notificationManager.CreateNotification(notificationInput);
                            }
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
                RuleTypeSettings = this.RuleTypeSettings.Get(context)
            };
        }

    }
}
