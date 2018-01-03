using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Vanrise.BusinessProcess;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;
using Vanrise.Queueing;

namespace Vanrise.Notification.BP.Activities.BalanceAlertChecker
{
    public class ClearAlertsInput
    {
        public VRBalanceAlertRuleTypeSettings RuleTypeSettings { get; set; }
        public BaseQueue<VREntityBalanceInfoBatch> InputQueue { get; set; }

        public BaseQueue<VRBalanceUpdateLastAlertInfoPayloadBatch> OutputQueue { get; set; }


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
            VRAlertRuleTypeManager alertRuleTypeManager = new VRAlertRuleTypeManager();
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

                            var activeAlertThresholds = new VRBalanceActiveAlertInfo();
                            activeAlertThresholds.ActiveAlertsThersholds.AddRange(entityBalanceInfo.ActiveAlertsInfo.ActiveAlertsThersholds);

                            foreach (var activeThreshold in entityBalanceInfo.ActiveAlertsInfo.ActiveAlertsThersholds.OrderBy(t => t.Threshold))
                            {
                                if (entityBalanceInfo.CurrentBalance > activeThreshold.Threshold)
                                {
                                    VRAlertRuleManager alertRuleManager = new VRAlertRuleManager();
                                    var alertRule = alertRuleManager.GetVRAlertRule(activeThreshold.AlertRuleId.Value);

                                    VRAlertRuleNotificationManager alertRuleNotificationManager = new VRAlertRuleNotificationManager();
                                    ClearAlertRuleNotificationInput notificationInput = new ClearAlertRuleNotificationInput
                                    {
                                        UserId = handle.SharedInstanceData.InstanceInfo.InitiatorUserId,
                                        EntityId = entityBalanceInfo.EntityId,
                                        EventKey = string.Format("{0}_{1}", entityBalanceInfo.EntityId, activeThreshold.Threshold),
                                        AlertRuleId = activeThreshold.AlertRuleId,
                                        RuleTypeId = alertRule.RuleTypeId,
                                        Description = string.Format("Rolling back balance actions for '{0}' (threshold '{1}')", entityBalanceInfo.EntityName, activeThreshold.Threshold),
                                        NotificationTypeId = alertRuleTypeManager.GetVRAlertRuleTypeSettings<VRBalanceAlertRuleTypeSettings>(alertRule.RuleTypeId).NotificationTypeId
                                    };
                                    alertRuleNotificationManager.ClearNotifications(notificationInput);
                                    activeAlertThresholds.ActiveAlertsThersholds.Remove(activeThreshold);
                                }
                                else
                                {
                                    break;
                                }
                            }

                            balanceUpdateLastAlertInfoPayloadBatch.Items.Add(new VRBalanceUpdateLastAlertInfoPayload
                            {
                                EntityBalanceInfo = entityBalanceInfo,
                                LastExecutedAlertThreshold = activeAlertThresholds.ActiveAlertsThersholds.Count > 0 ? activeAlertThresholds.ActiveAlertsThersholds.Min(a => a.Threshold) : null,
                                ActiveAlertsInfo = activeAlertThresholds
                            });
                        }
                        inputArgument.OutputQueue.Enqueue(balanceUpdateLastAlertInfoPayloadBatch);
                    });
                } while (!ShouldStop(handle) && hasItems);
            });
        }

        protected override ClearAlertsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ClearAlertsInput
            {
                InputQueue = this.InputQueue.Get(context),
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
