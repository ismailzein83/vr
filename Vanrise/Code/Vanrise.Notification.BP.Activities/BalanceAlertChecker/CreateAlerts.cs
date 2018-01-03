using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;
using Vanrise.Queueing;
using System.Linq;
using Vanrise.Notification.BP.Activities.BalanceAlertThresholdUpdate;

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
            VRAlertRuleTypeManager alertRuleTypeManager = new VRAlertRuleTypeManager();
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

                                var alertRule = alertRuleManager.GetVRAlertRule(entityBalanceInfo.AlertRuleId.Value);

                                VRBalanceAlertRuleSettings balanceAlertRuleSettings = alertRule.Settings.ExtendedSettings as VRBalanceAlertRuleSettings;

                                VRBalanceActiveAlertInfo activeAlertsInfo = new VRBalanceActiveAlertInfo();
                                if (entityBalanceInfo.ActiveAlertsInfo != null && entityBalanceInfo.ActiveAlertsInfo.ActiveAlertsThersholds != null)
                                    activeAlertsInfo.ActiveAlertsThersholds.AddRange(entityBalanceInfo.ActiveAlertsInfo.ActiveAlertsThersholds);

                                List<AlertThresholdInfo> thresholdsInfos = new List<AlertThresholdInfo>();
                                for (int i = 0; i < balanceAlertRuleSettings.ThresholdActions.Count; i++)
                                {
                                    VRBalanceAlertThresholdAction balanceAlertThresholdAction = balanceAlertRuleSettings.ThresholdActions[i];
                                    VRBalanceAlertThresholdContext vrBalanceAlertThresholdContext = new VRBalanceAlertThresholdContext { EntityBalanceInfo = entityBalanceInfo, AlertRuleTypeId = inputArgument.AlertTypeId };
                                    decimal roundedRuleThresholdValue = CalculateNextAlertThresholds.ThresholdRounding(balanceAlertThresholdAction.Threshold.GetThreshold(vrBalanceAlertThresholdContext));
                                    thresholdsInfos.Add(new AlertThresholdInfo { Threshold = roundedRuleThresholdValue, ThresholdAction = balanceAlertThresholdAction });
                                }

                                foreach (var thresholdInfo in thresholdsInfos.OrderByDescending(itm => itm.Threshold))
                                {
                                    var currentThreshold = thresholdInfo.Threshold;
                                    if (entityBalanceInfo.LastExecutedAlertThreshold.HasValue && entityBalanceInfo.LastExecutedAlertThreshold.Value <= currentThreshold)
                                        continue;

                                    if (entityBalanceInfo.CurrentBalance > currentThreshold)
                                        break;

                                    CreateAlertRuleNotificationInput createAlertRuleNotification = new CreateAlertRuleNotificationInput
                                    {
                                        AlertLevelId = thresholdInfo.ThresholdAction.AlertLevelId,
                                        Actions = thresholdInfo.ThresholdAction.Actions,
                                        ClearanceActions = thresholdInfo.ThresholdAction.RollbackActions,
                                        AlertRuleId = alertRule.VRAlertRuleId,
                                        EntityId = entityBalanceInfo.EntityId,
                                        EventKey = string.Format("{0}_{1}", entityBalanceInfo.EntityId, currentThreshold),
                                        EventPayload = new VRBalanceAlertEventPayload
                                        {
                                            AlertRuleTypeId = inputArgument.AlertTypeId,
                                            CurrentBalance = entityBalanceInfo.CurrentBalance,
                                            CurrencyId = entityBalanceInfo.CurrencyId,
                                            EntityId = entityBalanceInfo.EntityId,
                                            Threshold = currentThreshold
                                        },
                                        AlertRuleTypeId = inputArgument.AlertTypeId,
                                        UserId = handle.SharedInstanceData.InstanceInfo.InitiatorUserId,
                                        Description = string.Format("Executing balance actions for '{0}' (threshold '{1}')", entityBalanceInfo.EntityName, currentThreshold),
                                        NotificationTypeId = alertRuleTypeManager.GetVRAlertRuleTypeSettings<VRBalanceAlertRuleTypeSettings>(inputArgument.AlertTypeId).NotificationTypeId
                                    };

                                    activeAlertsInfo.ActiveAlertsThersholds.Add(new VRBalanceActiveAlertThreshold
                                    {
                                        AlertRuleId = alertRule.VRAlertRuleId,
                                        Threshold = currentThreshold
                                    });

                                    VRAlertRuleNotificationManager alertRuleNotificationManager = new VRAlertRuleNotificationManager();
                                    totalNotificationsCreated++;
                                    alertRuleNotificationManager.CreateNotification(createAlertRuleNotification);
                                    //handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} Notifications created.", totalNotificationsCreated);
                                }

                                balanceUpdateLastAlertInfoPayloadBatch.Items.Add(new VRBalanceUpdateLastAlertInfoPayload
                                {
                                    EntityBalanceInfo = entityBalanceInfo,
                                    LastExecutedAlertThreshold = activeAlertsInfo.ActiveAlertsThersholds.Count > 0 ? activeAlertsInfo.ActiveAlertsThersholds.Min(a => a.Threshold) : null,
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

        private class AlertThresholdInfo
        {
            public Decimal Threshold { get; set; }

            public VRBalanceAlertThresholdAction ThresholdAction { get; set; }
        }
    }
}
