using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Notification.BP.Activities.BalanceAlertThresholdUpdate;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;
using Vanrise.Queueing;
using Vanrise.Common;

namespace Vanrise.Notification.BP.Activities.BalanceAlertChecker
{
    public class RecreateAlertsInput
    {
        public VRBalanceAlertRuleTypeSettings RuleTypeSettings { get; set; }
        public Guid AlertTypeId { get; set; }
        public BaseQueue<VREntityBalanceInfoBatch> InputQueue { get; set; }
        public BaseQueue<VRBalanceUpdateRecreateAlertIntervalPayloadBatch> OutputQueue { get; set; }
    }

    public sealed class RecreateAlerts : DependentAsyncActivity<RecreateAlertsInput>
    {
        [RequiredArgument]
        public InArgument<VRBalanceAlertRuleTypeSettings> RuleTypeSettings { get; set; }
        [RequiredArgument]
        public InArgument<Guid> AlertTypeId { get; set; }
        [RequiredArgument]
        public InArgument<BaseQueue<VREntityBalanceInfoBatch>> InputQueue { get; set; }
        [RequiredArgument]
        public InOutArgument<BaseQueue<VRBalanceUpdateRecreateAlertIntervalPayloadBatch>> OutputQueue { get; set; }

        protected override void DoWork(RecreateAlertsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
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
                        VRBalanceUpdateRecreateAlertIntervalPayloadBatch balanceUpdateRecreateAlertIntervalPayloadBatch = new VRBalanceUpdateRecreateAlertIntervalPayloadBatch { Items = new List<VRBalanceUpdateRecreateAlertIntervalPayload>() };
                        foreach (var entityBalanceInfo in vrEntityBalanceInfoBatch.BalanceInfos)
                        {
                            VRAlertRuleManager alertRuleManager = new VRAlertRuleManager();

                            if (!entityBalanceInfo.AlertRuleId.HasValue)
                                continue;

                            if (!entityBalanceInfo.LastExecutedAlertThreshold.HasValue)
                                continue;

                            var alertRule = alertRuleManager.GetVRAlertRule(entityBalanceInfo.AlertRuleId.Value);

                            VRBalanceAlertRuleSettings balanceAlertRuleSettings = alertRule.Settings.ExtendedSettings.CastWithValidate<VRBalanceAlertRuleSettings>("alertRule.Settings.ExtendedSettings", alertRule.VRAlertRuleId);

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

                            var matchThresholdInfo = thresholdsInfos.FirstOrDefault(itm => itm.Threshold == entityBalanceInfo.LastExecutedAlertThreshold.Value);

                            if(matchThresholdInfo != null && matchThresholdInfo.Threshold >= entityBalanceInfo.CurrentBalance)
                            {
                                var currentThreshold = matchThresholdInfo.Threshold;
                                
                                List<VRAction> actionsToRecreate = matchThresholdInfo.ThresholdAction.Actions != null ? matchThresholdInfo.ThresholdAction.Actions.Where(a => a.CanReexecute).ToList() : null;

                                CreateAlertRuleNotificationInput createAlertRuleNotification = new CreateAlertRuleNotificationInput
                                {
                                    AlertLevelId = matchThresholdInfo.ThresholdAction.AlertLevelId,
                                    Actions = actionsToRecreate,
                                    AlertRuleId = alertRule.VRAlertRuleId,
                                    EntityId = entityBalanceInfo.EntityId,
                                    EventKey = string.Format("{0}_{1:0.00}_Repeated", entityBalanceInfo.EntityId, currentThreshold),
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
                                    Description = string.Format("Reexecuting balance actions for '{0}' (threshold '{1}')", entityBalanceInfo.EntityName, currentThreshold),
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
                            }
                            
                            balanceUpdateRecreateAlertIntervalPayloadBatch.Items.Add(new VRBalanceUpdateRecreateAlertIntervalPayload
                            {
                                EntityBalanceInfo = entityBalanceInfo,
                                RecreateAlertAfter = balanceAlertRuleSettings.RepeatEvery
                            });
                        }
                        inputArgument.OutputQueue.Enqueue(balanceUpdateRecreateAlertIntervalPayloadBatch);
                    });
                } while (!ShouldStop(handle) && hasItems);
            });
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} repeated notifications created.", totalNotificationsCreated);
        }

        protected override RecreateAlertsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new RecreateAlertsInput
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
                this.OutputQueue.Set(context, new MemoryQueue<VRBalanceUpdateRecreateAlertIntervalPayloadBatch>());
            base.OnBeforeExecute(context, handle);
        }

        private class AlertThresholdInfo
        {
            public Decimal Threshold { get; set; }

            public VRBalanceAlertThresholdAction ThresholdAction { get; set; }
        }
    }
}
