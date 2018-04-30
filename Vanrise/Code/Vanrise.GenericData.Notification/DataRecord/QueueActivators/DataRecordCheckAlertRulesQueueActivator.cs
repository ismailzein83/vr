using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.QueueActivators;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Notification
{
    public class DataRecordCheckAlertRulesQueueActivator : Vanrise.Queueing.Entities.QueueActivator
    {
        public Guid AlertRuleTypeId { get; set; }

        public override void OnDisposed()
        {
        }

        public override void ProcessItem(Queueing.Entities.PersistentQueueItem item, Queueing.Entities.ItemsToEnqueue outputItems)
        {
        }

        //public override void ProcessItem(Queueing.Entities.IQueueActivatorExecutionContext context)
        //{
        //    DataRecordBatch dataRecordBatch = context.ItemToProcess as DataRecordBatch;
        //    var queueItemType = context.CurrentStage.QueueItemType as DataRecordBatchQueueItemType;
        //    if (queueItemType == null)
        //        throw new Exception("current stage QueueItemType is not of type DataRecordBatchQueueItemType");
        //    var recordTypeId = queueItemType.DataRecordTypeId;
        //    var batchRecords = dataRecordBatch.GetBatchRecords(recordTypeId);

        //    VRAlertRuleType vrAlertRuleType = new VRAlertRuleTypeManager().GetVRAlertRuleType(this.AlertRuleTypeId);
        //    vrAlertRuleType.ThrowIfNull("vrAlertRuleType", this.AlertRuleTypeId);
        //    DataRecordAlertRuleTypeSettings dataRecordAlertRuleTypeSettings = vrAlertRuleType.Settings.CastWithValidate<DataRecordAlertRuleTypeSettings>("vrAlertRuleType.Settings", this.AlertRuleTypeId);

        //    List<VRAlertRule> vrAlertRules = new VRAlertRuleManager().GetActiveRules(this.AlertRuleTypeId);

        //    foreach (var vrAlertRule in vrAlertRules)
        //    {
        //        List<DataRecordAlertRuleNotification> dataRecordAlertRuleNotifications = new List<DataRecordAlertRuleNotification>();
        //        HashSet<string> eventKeys = new HashSet<string>();

        //        vrAlertRule.Settings.ThrowIfNull("vrAlertRule.Settings", vrAlertRule.VRAlertRuleId);
        //        DataRecordAlertRuleExtendedSettings dataRecordAlertRuleExtendedSettings =
        //            vrAlertRule.Settings.ExtendedSettings.CastWithValidate<DataRecordAlertRuleExtendedSettings>(string.Format("vrAlertRule.Settings.ExtendedSettings AlertRuleId:{0}", vrAlertRule.VRAlertRuleId));

        //        dataRecordAlertRuleExtendedSettings.AvailableIdentificationFields.ThrowIfNull("vrAlertRuleSettings.DataRecordAlertRuleExtendedSettings.AvailableIdentificationFields");
        //        IEnumerable<string> availableIdentificationFieldNames = dataRecordAlertRuleExtendedSettings.AvailableIdentificationFields.Select(itm => itm.Name);

        //        foreach (var record in batchRecords)
        //        {
        //            Dictionary<string, dynamic> fieldValues = record.GetDictionaryFromDataRecordType();

        //            string groupingKey;
        //            if (AreFieldValuesValid(availableIdentificationFieldNames, fieldValues, out groupingKey))
        //            {
        //                if (eventKeys.Contains(groupingKey))
        //                    continue;

        //                DataRecordAlertRuleSettingsIsMatchedContext dataRecordAlertRuleSettingsIsMatchedContext = new DataRecordAlertRuleSettingsIsMatchedContext()
        //                {
        //                    RecordFilterContext = new DataRecordDictFilterGenericFieldMatchContext(fieldValues, recordTypeId)
        //                };

        //                if (dataRecordAlertRuleExtendedSettings.Settings.IsRuleMatched(dataRecordAlertRuleSettingsIsMatchedContext))
        //                {
        //                    DataRecordAlertRuleNotification dataRecordAlertRuleNotification = new DataRecordAlertRuleNotification()
        //                    {
        //                        Actions = dataRecordAlertRuleSettingsIsMatchedContext.Actions,
        //                        AlertLevelId = dataRecordAlertRuleSettingsIsMatchedContext.AlertLevelId,
        //                        FieldValues = fieldValues,
        //                        GroupingKey = groupingKey
        //                    };
        //                    dataRecordAlertRuleNotifications.Add(dataRecordAlertRuleNotification);
        //                    eventKeys.Add(groupingKey);
        //                }
        //            }

        //            DataRecordAlertRuleNotificationManager dataRecordAlertRuleNotificationManager = new DataRecordAlertRuleNotificationManager();
        //            dataRecordAlertRuleNotificationManager.CreateAlertRuleNotifications(dataRecordAlertRuleNotifications, eventKeys.ToList(), vrAlertRule.RuleTypeId, dataRecordAlertRuleTypeSettings.NotificationTypeId,
        //                vrAlertRule.VRAlertRuleId, dataRecordAlertRuleExtendedSettings.MinNotificationInterval, recordTypeId, string.Format("Alert Rule {0}", vrAlertRule.Name), vrAlertRule.UserId);
        //        }
        //    }
        //}
        public override void ProcessItem(Queueing.Entities.IQueueActivatorExecutionContext context)
        {
            DataRecordBatch dataRecordBatch = context.ItemToProcess as DataRecordBatch;
            var queueItemType = context.CurrentStage.QueueItemType as DataRecordBatchQueueItemType;
            if (queueItemType == null)
                throw new Exception("current stage QueueItemType is not of type DataRecordBatchQueueItemType");
            var recordTypeId = queueItemType.DataRecordTypeId;
            var batchRecords = dataRecordBatch.GetBatchRecords(recordTypeId);
           
            DataRecordRuleEvaluationManager dataRecordRuleEvaluationManager = new DataRecordRuleEvaluationManager();
            dataRecordRuleEvaluationManager.EvaluateDataRecordRule(batchRecords, this.AlertRuleTypeId);
        }


        #region Private Methods

        private bool AreFieldValuesValid(IEnumerable<string> availableIdentificationFieldNames, Dictionary<string, dynamic> fieldValues, out string groupingKey)
        {
            StringBuilder strBuilder = new StringBuilder();
            dynamic fieldValue;
            groupingKey = string.Empty;

            foreach (var fieldName in availableIdentificationFieldNames)
            {
                if (fieldValues.TryGetValue(fieldName, out fieldValue))
                {
                    if (fieldValue == null)
                        return false;

                    strBuilder.AppendFormat("{0}@", fieldValue);
                }
            }

            groupingKey = strBuilder.ToString();
            return true;
        }

        #endregion

        #region Private Classes

        private class AlertNotificationData
        {
            public AlertNotificationData()
            {
                DataRecordAlertRuleNotifications = new List<DataRecordAlertRuleNotification>();
                EventKeys = new HashSet<string>();
            }
            public List<DataRecordAlertRuleNotification> DataRecordAlertRuleNotifications { get; set; }
            public HashSet<string> EventKeys { get; set; }
            public IEnumerable<string> AvailableIdentificationFieldNames { get; set; }
            public VRAlertRule AlertRule { get; set; }
            public DataRecordAlertRuleExtendedSettings DataRecordAlertRuleExtendedSettings { get; set; }
        }

        #endregion
    }
}
