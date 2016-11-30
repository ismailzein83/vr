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
using Vanrise.Notification.BP.Activities.BalanceAlertThresholdUpdate;

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
                            VRBalanceAlertRuleManager vrBalanceAlertRuleManager = new VRBalanceAlertRuleManager();
                            foreach (var entityBalanceInfo in vrEntityBalanceInfoBatch.BalanceInfos)
                            {
                                bool hasAlert = false;
                                VRBalanceAlertRuleCreateRuleTargetContext context = new VRBalanceAlertRuleCreateRuleTargetContext
                                {
                                    EntityBalanceInfo = entityBalanceInfo,
                                    RuleTypeSettings = inputArgument.RuleTypeSettings
                                };
                                decimal? threshold = default(decimal?);
                                GenericRuleTarget targetRule = inputArgument.RuleTypeSettings.Behavior.CreateRuleTarget(context);
                                VRAlertRule matchedRule = vrBalanceAlertRuleManager.GetMatchRule(inputArgument.AlertTypeId, targetRule);
                                if (matchedRule == null)
                                    continue;

                                VRBalanceAlertRuleSettings balanceAlertRuleSettings = matchedRule.Settings.ExtendedSettings as VRBalanceAlertRuleSettings;

                                List<VRAction> vrActions = new List<VRAction>();
                                List<VRAction> vrRollbackActions = new List<VRAction>();
                                foreach (VRBalanceAlertThresholdAction balanceAlertThresholdAction in balanceAlertRuleSettings.ThresholdActions)
                                {
                                    VRBalanceAlertThresholdContext vrBalanceAlertThresholdContext = new VRBalanceAlertThresholdContext { EntityBalanceInfo = entityBalanceInfo };
                                    threshold = balanceAlertThresholdAction.Threshold.GetThreshold(vrBalanceAlertThresholdContext);
                                    if (entityBalanceInfo.NextAlertThreshold == threshold)
                                    {
                                        vrActions = balanceAlertThresholdAction.Actions;
                                        vrRollbackActions = balanceAlertThresholdAction.RollbackActions;
                                        hasAlert = true;
                                        break;
                                    }
                                }
                                if (hasAlert)
                                {
                                    CreateAlertRuleNotificationInput notificationInput = new CreateAlertRuleNotificationInput
                                    {
                                        Actions = vrActions,
                                        ClearanceActions = vrRollbackActions,
                                        AlertRuleId = matchedRule.VRAlertRuleId,
                                        EventKey = entityBalanceInfo.EntityId,
                                        EventPayload = new VRBalanceAlertEventPayload
                                        {
                                            AlertRuleTypeId = inputArgument.AlertTypeId,
                                            CurrentBalance = entityBalanceInfo.CurrentBalance,
                                            EntityId = entityBalanceInfo.EntityId,
                                            Threshold = threshold.Value
                                        },
                                        AlertRuleTypeId = inputArgument.AlertTypeId,
                                        UserId = handle.SharedInstanceData.InstanceInfo.InitiatorUserId,
                                        Description = ""
                                    };
                                    VRAlertRuleNotificationManager notificationManager = new VRAlertRuleNotificationManager();
                                    notificationManager.CreateNotification(notificationInput);
                                }
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
