using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
namespace Vanrise.GenericData.Notification
{
    public class DataRecordRuleEvaluationManager
    {

        public void EvaluateDataRecordRule(List<dynamic> batchRecords, Guid alertRuleTypeId)
        {
            VRAlertRuleType vrAlertRuleType = new VRAlertRuleTypeManager().GetVRAlertRuleType(alertRuleTypeId);
            vrAlertRuleType.ThrowIfNull("vrAlertRuleType", alertRuleTypeId);
            DataRecordAlertRuleTypeSettings dataRecordAlertRuleTypeSettings = vrAlertRuleType.Settings.CastWithValidate<DataRecordAlertRuleTypeSettings>("vrAlertRuleType.Settings", alertRuleTypeId);

            List<VRAlertRule> vrAlertRules = new VRAlertRuleManager().GetActiveRules(alertRuleTypeId);

            Dictionary<long, AlertNotificationData> alertNotificationDataDict = new Dictionary<long, AlertNotificationData>();

            Dictionary<string, DataRecordField> dataRecordFields = new DataRecordTypeManager().GetDataRecordTypeFields(dataRecordAlertRuleTypeSettings.DataRecordTypeId);

            foreach (var record in batchRecords)
            {
                Dictionary<string, dynamic> fieldValues = record.GetDictionaryFromDataRecordType();

                DataRecordAlertRuleSettingsIsMatchedContext dataRecordAlertRuleSettingsIsMatchedContext = new DataRecordAlertRuleSettingsIsMatchedContext()
                {
                    RecordFilterContext = new DataRecordDictFilterGenericFieldMatchContext(fieldValues, dataRecordFields)
                };

                foreach (var vrAlertRule in vrAlertRules)
                {
                    AlertNotificationData alertNotificationData;
                    if (!alertNotificationDataDict.TryGetValue(vrAlertRule.VRAlertRuleId, out alertNotificationData))
                    {
                        vrAlertRule.Settings.ThrowIfNull("vrAlertRule.Settings", vrAlertRule.VRAlertRuleId);
                        DataRecordAlertRuleExtendedSettings dataRecordAlertRuleExtendedSettings =
                            vrAlertRule.Settings.ExtendedSettings.CastWithValidate<DataRecordAlertRuleExtendedSettings>(string.Format("vrAlertRule.Settings.ExtendedSettings AlertRuleId:{0}", vrAlertRule.VRAlertRuleId));

                        dataRecordAlertRuleExtendedSettings.AvailableIdentificationFields.ThrowIfNull("vrAlertRuleSettings.DataRecordAlertRuleExtendedSettings.AvailableIdentificationFields");
                        alertNotificationData = new AlertNotificationData()
                        {
                            AvailableIdentificationFieldNames = dataRecordAlertRuleExtendedSettings.AvailableIdentificationFields.Select(itm => itm.Name),
                            DataRecordAlertRuleExtendedSettings = dataRecordAlertRuleExtendedSettings,
                            AlertRule = vrAlertRule
                        };
                        alertNotificationDataDict.Add(vrAlertRule.VRAlertRuleId, alertNotificationData);
                    }

                    string groupingKey;
                    if (AreFieldValuesValid(alertNotificationData.AvailableIdentificationFieldNames, fieldValues, out groupingKey))
                    {
                        if (alertNotificationData.EventKeys.Contains(groupingKey))
                            continue;

                        if (alertNotificationData.DataRecordAlertRuleExtendedSettings.Settings.IsRuleMatched(dataRecordAlertRuleSettingsIsMatchedContext))
                        {
                            DataRecordAlertRuleNotification dataRecordAlertRuleNotification = new DataRecordAlertRuleNotification()
                            {
                                Actions = dataRecordAlertRuleSettingsIsMatchedContext.Actions,
                                AlertLevelId = dataRecordAlertRuleSettingsIsMatchedContext.AlertLevelId,
                                FieldValues = fieldValues,
                                GroupingKey = groupingKey
                            };
                            alertNotificationData.DataRecordAlertRuleNotifications.Add(dataRecordAlertRuleNotification);
                            alertNotificationData.EventKeys.Add(groupingKey);
                        }
                    }
                }
            }

            DataRecordAlertRuleNotificationManager dataRecordAlertRuleNotificationManager = new DataRecordAlertRuleNotificationManager();

            foreach (var alertNotificationDataItem in alertNotificationDataDict)
            {
                AlertNotificationData alertNotificationData = alertNotificationDataItem.Value;
                dataRecordAlertRuleNotificationManager.CreateAlertRuleNotifications(alertNotificationData.DataRecordAlertRuleNotifications, alertNotificationData.EventKeys.ToList(), alertNotificationData.AlertRule.RuleTypeId, dataRecordAlertRuleTypeSettings.NotificationTypeId,
                    alertNotificationData.AlertRule.VRAlertRuleId, alertNotificationData.DataRecordAlertRuleExtendedSettings.MinNotificationInterval, dataRecordAlertRuleTypeSettings.DataRecordTypeId, string.Format("Alert Rule {0}", alertNotificationData.AlertRule.Name), alertNotificationData.AlertRule.UserId);
            }
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
