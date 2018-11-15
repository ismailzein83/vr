using System;
using System.Collections.Generic;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Notification;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;

namespace Vanrise.Analytic.Business
{
    public class DAProfCalcGenerateVRAlertEventsOutputProcessor : IDAProfCalcOutputRecordProcessor
    {
        public DateTime EffectiveDate { get; set; }
        public Action<LogEntryType, string> LogMessage { get; set; }

        public DAProfCalcGenerateVRAlertEventsOutputProcessor(DateTime effectiveDate, Action<LogEntryType, string> logMessage)
        {
            this.EffectiveDate = effectiveDate;
            this.LogMessage = logMessage;
        }
        public void Initialize(IDAProfCalcOutputRecordProcessorIntializeContext context)
        {

        }

        public void ProcessOutputRecords(IDAProfCalcOutputRecordProcessorProcessContext context)
        {
            if (context.OutputRecords != null && context.OutputRecords.Count > 0)
            {
                DAProfCalcExecInput daProfCalcExecInput = context.DAProfCalcExecInput;
                daProfCalcExecInput.ThrowIfNull("context.DAProfCalcExecInput");

                Guid dataAnalysisItemDefinitionId = daProfCalcExecInput.OutputItemDefinitionId;

                DataAnalysisItemDefinitionManager dataAnalysisItemDefinitionManager = new DataAnalysisItemDefinitionManager();

                DAProfCalcAlertRuleExecPayload daProfCalcAlertRuleExecPayload = daProfCalcExecInput.DAProfCalcPayload.CastWithValidate<DAProfCalcAlertRuleExecPayload>("context.DAProfCalcExecInput.DAProfCalcPayload");

                Guid notificationTypeId = new DAProfCalcNotificationManager().GetDAProfCalcNotificationTypeId(daProfCalcAlertRuleExecPayload.AlertRuleTypeId, dataAnalysisItemDefinitionId);

                DAProfCalcOutputSettings daProfCalcOutputSettings = dataAnalysisItemDefinitionManager.GetDataAnalysisItemDefinitionSettings<DAProfCalcOutputSettings>(dataAnalysisItemDefinitionId);
                DataAnalysisItemDefinition dataAnalysisItemDefinition = dataAnalysisItemDefinitionManager.GetDataAnalysisItemDefinition(dataAnalysisItemDefinitionId);

                Dictionary<long, DAProfCalcNotificationData> daProfCalcNotificationDataDict = new Dictionary<long, DAProfCalcNotificationData>();

                foreach (DAProfCalcOutputRecord outputRecord in context.OutputRecords)
                {
                    DataRecordAlertRuleSettingsIsMatchedContext dataRecordAlertRuleSettingsIsMatchedContext = new DataRecordAlertRuleSettingsIsMatchedContext()
                    {
                        RecordFilterContext = new DataRecordDictFilterGenericFieldMatchContext(outputRecord.FieldValues, daProfCalcOutputSettings.RecordTypeId)
                    };

                    foreach (long alertRuleId in daProfCalcAlertRuleExecPayload.AlertRuleIds)
                    {
                        DAProfCalcNotificationData daProfCalcNotificationData;
                        if (!daProfCalcNotificationDataDict.TryGetValue(alertRuleId, out daProfCalcNotificationData))
                        {
                            VRAlertRule alertRule = new VRAlertRuleManager().GetVRAlertRule(alertRuleId);

                            alertRule.Settings.ThrowIfNull("alertRule.Settings", alertRuleId);
                            DAProfCalcAlertRuleSettings daProfCalcAlertRuleSettings = alertRule.Settings.ExtendedSettings.CastWithValidate<DAProfCalcAlertRuleSettings>(string.Format("alertRule.Settings.ExtendedSettings AlertRuleId:{0}", alertRuleId));

                            daProfCalcNotificationData = new DAProfCalcNotificationData()
                            {
                                DAProfCalcAlertRuleSettings = daProfCalcAlertRuleSettings,
                                AlertRule = alertRule
                            };
                            daProfCalcNotificationDataDict.Add(alertRuleId, daProfCalcNotificationData);
                        }

                        if (daProfCalcNotificationData.DAProfCalcAlertRuleSettings.Settings.IsRuleMatched(dataRecordAlertRuleSettingsIsMatchedContext))
                        {
                            DataRecordAlertRuleNotification dataRecordAlertRuleNotification = new DataRecordAlertRuleNotification()
                            {
                                Actions = dataRecordAlertRuleSettingsIsMatchedContext.Actions,
                                AlertLevelId = dataRecordAlertRuleSettingsIsMatchedContext.AlertLevelId,
                                FieldValues = outputRecord.FieldValues,
                                GroupingKey = outputRecord.GroupingKey
                            };
                            daProfCalcNotificationData.DataRecordAlertRuleNotifications.Add(dataRecordAlertRuleNotification);
                            daProfCalcNotificationData.EventKeys.Add(outputRecord.GroupingKey);
                        }
                    }
                }

                foreach (var daProfCalcNotificationDataItem in daProfCalcNotificationDataDict)
                {
                    DAProfCalcNotificationData daProfCalcNotificationData = daProfCalcNotificationDataItem.Value;

                    int numberOfMinutes = daProfCalcNotificationData.DAProfCalcAlertRuleSettings.DAProfCalcAnalysisPeriod.GetPeriodInMinutes();
                    DateTime periodStart = EffectiveDate.AddMinutes(-1 * numberOfMinutes);

                    string description = string.Format("Analysis Period: {0}, Period Start: {1:yyyy-MM-dd HH:mm:ss}, Period End: {2:yyyy-MM-dd HH:mm:ss}, Rule: {3}", daProfCalcNotificationData.DAProfCalcAlertRuleSettings.DAProfCalcAnalysisPeriod.GetDescription(), periodStart, EffectiveDate, daProfCalcNotificationData.AlertRule.Name);

                    DataRecordAlertRuleNotificationManager dataRecordAlertRuleNotificationManager = new DataRecordAlertRuleNotificationManager();
                    int numberOfNotificationsCreated;
                    bool createNotification = dataRecordAlertRuleNotificationManager.TryCreateAlertRuleNotifications(daProfCalcNotificationData.DataRecordAlertRuleNotifications, daProfCalcNotificationData.EventKeys, daProfCalcNotificationData.AlertRule.RuleTypeId, notificationTypeId, daProfCalcNotificationDataItem.Key,
                         daProfCalcNotificationData.DAProfCalcAlertRuleSettings.MinNotificationInterval, daProfCalcOutputSettings.RecordTypeId, description, daProfCalcNotificationData.AlertRule.UserId, out numberOfNotificationsCreated);

                    if (createNotification)
                        LogMessage(LogEntryType.Information, string.Format("Action Rule {0} created {1} notification(s)", daProfCalcNotificationData.AlertRule.Name, numberOfNotificationsCreated));
                }
            }
        }

        public void Finalize(IDAProfCalcOutputRecordProcessorFinalizeContext context)
        {

        }

        private class DAProfCalcNotificationData
        {
            public DAProfCalcNotificationData()
            {
                DataRecordAlertRuleNotifications = new List<DataRecordAlertRuleNotification>();
                EventKeys = new List<string>();
            }
            public List<DataRecordAlertRuleNotification> DataRecordAlertRuleNotifications { get; set; }
            public List<string> EventKeys { get; set; }
            public VRAlertRule AlertRule { get; set; }

            public DAProfCalcAlertRuleSettings DAProfCalcAlertRuleSettings { get; set; }
        }
    }
}