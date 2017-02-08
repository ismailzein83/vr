using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;
using Vanrise.Queueing;

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
            int totalNotificationsCreated = 0;
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

                                VRBalanceActiveAlertInfo activeAlertsInfo = new VRBalanceActiveAlertInfo();
                                if (entityBalanceInfo.ActiveAlertsInfo != null && entityBalanceInfo.ActiveAlertsInfo.ActiveAlertsThersholds != null)
                                    activeAlertsInfo.ActiveAlertsThersholds.AddRange(entityBalanceInfo.ActiveAlertsInfo.ActiveAlertsThersholds);
                                var lastExecutedThreshold = default(decimal?);
                                for (int i = entityBalanceInfo.ThresholdActionIndex.Value; i < balanceAlertRuleSettings.ThresholdActions.Count; i++)
                                {
                                    VRBalanceAlertThresholdAction balanceAlertThresholdAction = balanceAlertRuleSettings.ThresholdActions[i];

                                    VRBalanceAlertThresholdContext vrBalanceAlertThresholdContext = new VRBalanceAlertThresholdContext { EntityBalanceInfo = entityBalanceInfo,AlertRuleTypeId = inputArgument.AlertTypeId };
                                    var currentThreshold = balanceAlertThresholdAction.Threshold.GetThreshold(vrBalanceAlertThresholdContext);
                                    if (entityBalanceInfo.CurrentBalance < currentThreshold)
                                    {
                                        lastExecutedThreshold = currentThreshold;
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
                                                Threshold = lastExecutedThreshold.Value
                                            },                                             
                                            AlertRuleTypeId = inputArgument.AlertTypeId,
                                            UserId = handle.SharedInstanceData.InstanceInfo.InitiatorUserId,
                                            Description = ""
                                        };

                                        activeAlertsInfo.ActiveAlertsThersholds.Add(new VRBalanceActiveAlertThreshold
                                        {
                                            AlertRuleId = alertRule.VRAlertRuleId,
                                            Threshold = lastExecutedThreshold
                                        });

                                        VRAlertRuleNotificationManager alertRuleNotificationManager = new VRAlertRuleNotificationManager();
                                        totalNotificationsCreated++;
                                        alertRuleNotificationManager.CreateNotification(createAlertRuleNotification);
                                        //handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} Notifications created.", totalNotificationsCreated);
                                    }
                                }
                                balanceUpdateLastAlertInfoPayloadBatch.Items.Add(new VRBalanceUpdateLastAlertInfoPayload
                                {
                                    EntityBalanceInfo = entityBalanceInfo,
                                    LastExecutedAlertThreshold = lastExecutedThreshold,
                                    ActiveAlertsInfo = activeAlertsInfo
                                });
                            }
                            inputArgument.OutputQueue.Enqueue(balanceUpdateLastAlertInfoPayloadBatch);
                        });
                    } while (!ShouldStop(handle) && hasItems);
                });
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} Notifications created.", totalNotificationsCreated);
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
