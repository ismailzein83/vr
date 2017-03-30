using System;
using Vanrise.Analytic.Entities;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;
using Vanrise.GenericData.Notification;
using Vanrise.Common;
using System.Collections.Generic;
using System.Linq;
using Vanrise.GenericData.Business;

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
            if (context.OutputRecords != null && context.OutputRecords.Count > 0)
            {
                DAProfCalcExecInput daProfCalcExecInput = context.DAProfCalcExecInput;
                daProfCalcExecInput.ThrowIfNull("context.DAProfCalcExecInput");

                Guid dataAnalysisItemDefinitionId = daProfCalcExecInput.OutputItemDefinitionId;

                DataAnalysisItemDefinitionManager dataAnalysisItemDefinitionManager = new DataAnalysisItemDefinitionManager();

                DAProfCalcAlertRuleExecPayload daProfCalcAlertRuleExecPayload = daProfCalcExecInput.DAProfCalcPayload.CastWithValidate<DAProfCalcAlertRuleExecPayload>("context.DAProfCalcExecInput.DAProfCalcPayload");

                DAProfCalcOutputSettings daProfCalcOutputSettings = dataAnalysisItemDefinitionManager.GetDataAnalysisItemDefinitionSettings<DAProfCalcOutputSettings>(dataAnalysisItemDefinitionId);
                DataAnalysisItemDefinition dataAnalysisItemDefinition = dataAnalysisItemDefinitionManager.GetDataAnalysisItemDefinition(dataAnalysisItemDefinitionId);

                VRAlertRule alertRule = new VRAlertRuleManager().GetVRAlertRule(daProfCalcAlertRuleExecPayload.AlertRuleId);
                Guid notificationTypeId = new DAProfCalcNotificationManager().GetDAProfCalcNotificationTypeId(alertRule.RuleTypeId, dataAnalysisItemDefinitionId);

                alertRule.Settings.ThrowIfNull("alertRule.Settings", daProfCalcAlertRuleExecPayload.AlertRuleId);
                DAProfCalcAlertRuleSettings daProfCalcAlertRuleSettings = alertRule.Settings.ExtendedSettings.CastWithValidate<DAProfCalcAlertRuleSettings>(string.Format("alertRule.Settings.ExtendedSettings AlertRuleId:{0}", daProfCalcAlertRuleExecPayload.AlertRuleId));

                List<DataRecordAlertRuleNotification> dataRecordAlertRuleNotifications = new List<DataRecordAlertRuleNotification>();
                List<string> eventKeys = new List<string>();

                foreach (DAProfCalcOutputRecord outputRecord in context.OutputRecords)
                {
                    DataRecordAlertRuleSettingsIsMatchedContext dataRecordAlertRuleSettingsIsMatchedContext = new DataRecordAlertRuleSettingsIsMatchedContext() 
                    {
                        RecordFilterContext = new DataRecordDictFilterGenericFieldMatchContext(outputRecord.FieldValues, daProfCalcOutputSettings.RecordTypeId)
                    };

                    if (daProfCalcAlertRuleSettings.Settings.IsRuleMatched(dataRecordAlertRuleSettingsIsMatchedContext))
                    {
                        DataRecordAlertRuleNotification dataRecordAlertRuleNotification = new DataRecordAlertRuleNotification()
                        {
                            Actions = dataRecordAlertRuleSettingsIsMatchedContext.Actions,
                            AlertLevelId = dataRecordAlertRuleSettingsIsMatchedContext.AlertLevelId,
                            FieldValues = outputRecord.FieldValues,
                            GroupingKey = outputRecord.GroupingKey
                        };
                        dataRecordAlertRuleNotifications.Add(dataRecordAlertRuleNotification);
                        eventKeys.Add(outputRecord.GroupingKey);
                    }
                }
                DataRecordAlertRuleNotificationManager dataRecordAlertRuleNotificationManager = new DataRecordAlertRuleNotificationManager();
                dataRecordAlertRuleNotificationManager.CreateAlertRuleNotifications(dataRecordAlertRuleNotifications, eventKeys, alertRule.RuleTypeId, notificationTypeId, alertRule.VRAlertRuleId,
                     daProfCalcAlertRuleSettings.MinNotificationInterval, daProfCalcOutputSettings.RecordTypeId, string.Format("Alert Rule {0}, Data Analysis Item {1}", alertRule.Name, dataAnalysisItemDefinition.Name), UserId);
            }
        }

        public void Finalize(IDAProfCalcOutputRecordProcessorFinalizeContext context)
        {

        }
    }
}