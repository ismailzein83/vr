using System;
using System.Collections.Generic;
using Vanrise.Analytic.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;
using System.Linq;

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
                RecordFilterManager recordFilterManager = new RecordFilterManager();
                DAProfCalcOutputSettingsManager daProfCalcOutputSettingsManager = new DAProfCalcOutputSettingsManager();

                VRAlertRuleNotificationManager alertRuleNotificationManager = new VRAlertRuleNotificationManager();
                foreach (DAProfCalcOutputRecord outputRecord in context.OutputRecords)
                {
                    DAProfCalcAlertRuleExecPayload payload = outputRecord.DAProfCalcExecInput.DAProfCalcPayload as DAProfCalcAlertRuleExecPayload;
                    long alertRuleId = payload.AlertRuleId;
                    VRAlertRule alertRule = alertRuleManager.GetVRAlertRule(alertRuleId);

                    DAProfCalcAlertRuleSettings daProfCalcAlertRuleSettings = alertRule.Settings.ExtendedSettings as DAProfCalcAlertRuleSettings;
                    List<DataRecordField> dataRecordFields = daProfCalcOutputSettingsManager.GetOutputFields(outputRecord.DAProfCalcExecInput.OutputItemDefinitionId);

                    if (recordFilterManager.IsFilterGroupMatch(daProfCalcAlertRuleSettings.FilterGroup, new DAProfCalcRecordFilterGenericFieldMatchContext(outputRecord.Records, dataRecordFields)))
                    {
                        CreateAlertRuleNotificationInput notificationInput = new CreateAlertRuleNotificationInput()
                        {
                            Actions = daProfCalcAlertRuleSettings.Actions,
                            AlertRuleId = alertRuleId,
                            AlertRuleTypeId = alertRule.RuleTypeId,
                            Description = "This is the description",
                            EventKey = outputRecord.GroupingKey,
                            EventPayload = new DAProfCalcAlertRuleActionEventPayload() { AlertRuleId = alertRuleId, AlertRuleTypeId = alertRule.RuleTypeId, GroupingKey = outputRecord.GroupingKey, UserId = UserId },
                            UserId = UserId
                        };
                        alertRuleNotificationManager.CreateNotification(notificationInput);
                    }
                }
            }
        }

        public void Finalize(IDAProfCalcOutputRecordProcessorFinalizeContext context)
        {

        }
    }

    public class DAProfCalcRecordFilterGenericFieldMatchContext : IRecordFilterGenericFieldMatchContext
    {
        Dictionary<string, dynamic> _dataRecord;
        Dictionary<string, DataRecordField> _recordTypeFieldsByName;
        public DAProfCalcRecordFilterGenericFieldMatchContext(Dictionary<string, dynamic> dataRecord, List<DataRecordField> dataRecordFields)
        {
            _dataRecord = dataRecord;
            _recordTypeFieldsByName = dataRecordFields.ToDictionary(itm => itm.Name, itm => itm);
        }

        public object GetFieldValue(string fieldName, out DataRecordFieldType fieldType)
        {
            DataRecordField field;
            if (!_recordTypeFieldsByName.TryGetValue(fieldName, out field))
                throw new NullReferenceException(String.Format("field. fieldName '{0}'", fieldName));
            fieldType = field.Type;
            return _dataRecord[fieldName];
        }
    }
}