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

                VRAlertRuleNotificationManager alertRuleNotificationManager = new VRAlertRuleNotificationManager();

                DataAnalysisItemDefinitionManager dataAnalysisItemDefinitionManager = new DataAnalysisItemDefinitionManager();

                foreach (DAProfCalcOutputRecord outputRecord in context.OutputRecords)
                {
                    Guid dataAnalysisItemDefinitionId = outputRecord.DAProfCalcExecInput.OutputItemDefinitionId;

                    DataAnalysisItemDefinition dataAnalysisItemDefinition = dataAnalysisItemDefinitionManager.GetDataAnalysisItemDefinition(dataAnalysisItemDefinitionId);
                    if (dataAnalysisItemDefinition == null)
                        throw new NullReferenceException(string.Format("dataAnalysisItemDefinition. OutputItemDefinitionId: {0}", dataAnalysisItemDefinitionId));

                    if (dataAnalysisItemDefinition.Settings == null)
                        throw new NullReferenceException(string.Format("dataAnalysisItemDefinition.Settings. OutputItemDefinitionId: {0}", dataAnalysisItemDefinitionId));

                    DAProfCalcOutputSettings daProfCalcOutputSettings = dataAnalysisItemDefinition.Settings as DAProfCalcOutputSettings;
                    if (daProfCalcOutputSettings == null)
                        throw new Exception(String.Format("dataAnalysisItemDefinition.Settings is not of type DAProfCalcOutputSettings. it is of type {0}", dataAnalysisItemDefinition.Settings.GetType()));

                    DAProfCalcAlertRuleExecPayload payload = outputRecord.DAProfCalcExecInput.DAProfCalcPayload as DAProfCalcAlertRuleExecPayload;
                    long alertRuleId = payload.AlertRuleId;
                    VRAlertRule alertRule = alertRuleManager.GetVRAlertRule(alertRuleId);

                    DAProfCalcAlertRuleSettings daProfCalcAlertRuleSettings = alertRule.Settings.ExtendedSettings as DAProfCalcAlertRuleSettings;

                    List<DataRecordField> dataRecordFields = daProfCalcOutputSettings.GetOutputFields(null);

                    if (recordFilterManager.IsFilterGroupMatch(daProfCalcAlertRuleSettings.FilterGroup, new DAProfCalcRecordFilterGenericFieldMatchContext(outputRecord.Records, dataRecordFields)))
                    {
                        CreateAlertRuleNotificationInput notificationInput = new CreateAlertRuleNotificationInput()
                        {
                            Actions = daProfCalcAlertRuleSettings.Actions,
                            AlertRuleId = alertRuleId,
                            AlertRuleTypeId = alertRule.RuleTypeId,
                            Description = "This is the description",
                            EventKey = outputRecord.GroupingKey,
                            EventPayload = new DAProfCalcAlertRuleActionEventPayload()
                            {
                                AlertRuleId = alertRuleId,
                                AlertRuleTypeId = alertRule.RuleTypeId,
                                GroupingKey = outputRecord.GroupingKey,
                                UserId = UserId,
                                DataRecordTypeId = daProfCalcOutputSettings.RecordTypeId,
                                OutputRecords = outputRecord.Records
                            },
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