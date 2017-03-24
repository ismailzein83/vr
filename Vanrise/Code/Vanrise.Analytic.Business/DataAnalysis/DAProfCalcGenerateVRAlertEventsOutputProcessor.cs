using System;
using Vanrise.Analytic.Entities;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;
using Vanrise.GenericData.Notification;
using Vanrise.Common;

namespace Vanrise.Analytic.Business
{
    public class DAProfCalcGenerateVRAlertEventsOutputProcessor : IDAProfCalcOutputRecordProcessor
    {
        public int UserId { get; set; }

        public DAProfCalcGenerateVRAlertEventsOutputProcessor(int userId)
        {
            UserId = userId;
        }
        public void Initialize(IDAProfCalcOutputRecordProcessorIntializeContext context)
        {

        }

        public void ProcessOutputRecords(IDAProfCalcOutputRecordProcessorProcessContext context)
        {
            if (context.OutputRecords != null)
            {
                VRAlertRuleManager alertRuleManager = new VRAlertRuleManager();

                VRAlertRuleNotificationManager alertRuleNotificationManager = new VRAlertRuleNotificationManager();

                DataAnalysisItemDefinitionManager dataAnalysisItemDefinitionManager = new DataAnalysisItemDefinitionManager();
                VRAlertRuleTypeManager vrAlertRuleTypeManager = new VRAlertRuleTypeManager();

                foreach (DAProfCalcOutputRecord outputRecord in context.OutputRecords)
                {
                    Guid dataAnalysisItemDefinitionId = outputRecord.DAProfCalcExecInput.OutputItemDefinitionId;

                    DAProfCalcOutputSettings daProfCalcOutputSettings = dataAnalysisItemDefinitionManager.GetDataAnalysisItemDefinitionSettings<DAProfCalcOutputSettings>(dataAnalysisItemDefinitionId);

                    DAProfCalcAlertRuleExecPayload payload = outputRecord.DAProfCalcExecInput.DAProfCalcPayload as DAProfCalcAlertRuleExecPayload;
                    long alertRuleId = payload.AlertRuleId;
                    VRAlertRule alertRule = alertRuleManager.GetVRAlertRule(alertRuleId);

                    DAProfCalcAlertRuleSettings daProfCalcAlertRuleSettings = alertRule.Settings.ExtendedSettings as DAProfCalcAlertRuleSettings;

                    DAProfCalcAlertRuleIsMatchedContext daProfCalcAlertRuleIsMatchedContext = new DAProfCalcAlertRuleIsMatchedContext() { DataRecordTypeId = daProfCalcOutputSettings.RecordTypeId, OutputRecords = outputRecord.Records };
                    if (daProfCalcAlertRuleSettings.Settings.IsRuleMatched(daProfCalcAlertRuleIsMatchedContext))
                    {
                        DataAnalysisItemDefinition dataAnalysisItemDefinition = dataAnalysisItemDefinitionManager.GetDataAnalysisItemDefinition(dataAnalysisItemDefinitionId);
                        CreateAlertRuleNotificationInput notificationInput = new CreateAlertRuleNotificationInput()
                        {
                            Actions = daProfCalcAlertRuleIsMatchedContext.Actions,
                            AlertLevelId = daProfCalcAlertRuleIsMatchedContext.AlertLevelId,
                            AlertRuleId = alertRuleId,
                            AlertRuleTypeId = alertRule.RuleTypeId,
                            Description = string.Format("Alert Rule {0}, Data Analysis Item {1}", alertRule.Name, dataAnalysisItemDefinition.Name),
                            EventKey = outputRecord.GroupingKey,
                            EventPayload = new DataRecordAlertRuleActionEventPayload()
                            {
                                DataRecordTypeId = daProfCalcOutputSettings.RecordTypeId,
                                OutputRecords = outputRecord.Records
                            },
                            UserId = UserId,
                            NotificationTypeId = GetNotificationTypeId(vrAlertRuleTypeManager, alertRule.RuleTypeId, dataAnalysisItemDefinitionId)
                        };
                        alertRuleNotificationManager.CreateNotification(notificationInput);
                    }
                }
            }
        }

        private Guid GetNotificationTypeId(VRAlertRuleTypeManager vrAlertRuleTypeManager, Guid alertRuleTypeId, Guid dataAnalysisItemDefinitionId)
        {
            VRAlertRuleType alertRuleType = vrAlertRuleTypeManager.GetVRAlertRuleType(alertRuleTypeId);
            DAProfCalcAlertRuleTypeSettings alertRuleSettings = alertRuleType.Settings as DAProfCalcAlertRuleTypeSettings;
            if (alertRuleSettings == null)
                throw new Exception(String.Format("alertRuleType.Settings is not of type DAProfCalcAlertRuleTypeSettings. it is of type '0'", alertRuleSettings.GetType()));

            if (alertRuleSettings.DAProfCalcItemNotifications == null)
                throw new NullReferenceException(string.Format("alertRuleSettings.DAProfCalcItemNotifications is null for alertRuleTypeId:{0}", alertRuleTypeId));

            DAProfCalcItemNotification daProfCalcItemNotification = alertRuleSettings.DAProfCalcItemNotifications.FindRecord(itm => itm.DataAnalysisItemDefinitionId == dataAnalysisItemDefinitionId);
            if (daProfCalcItemNotification == null)
                throw new NullReferenceException(string.Format("daProfCalcItemNotification is null for alertRuleTypeId:{0} and dataAnalysisItemDefinitionId:{1}", alertRuleTypeId, dataAnalysisItemDefinitionId));

            return daProfCalcItemNotification.NotificationTypeId;
        }

        public void Finalize(IDAProfCalcOutputRecordProcessorFinalizeContext context)
        {

        }
    }
}