using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;

namespace Vanrise.GenericData.Notification
{
    public class DataRecordAlertRuleNotificationManager
    {
        public bool TryCreateAlertRuleNotifications(List<DataRecordAlertRuleNotification> dataRecordAlertRuleNotifications, List<string> eventKeys, Guid alertRuleTypeId,
                       Guid notificationTypeId, long alertRuleId, TimeSpan minNotificationInterval, Guid dataRecordTypeId, string alertNotificationDescription, int userId, out int numberOfNotificationsCreated)
        {
            bool createNotification = false;
            numberOfNotificationsCreated = 0;

            if (dataRecordAlertRuleNotifications != null && dataRecordAlertRuleNotifications.Count > 0)
            {
                VRAlertRuleNotificationManager vrAlertRuleNotificationManager = new VRAlertRuleNotificationManager();
                Dictionary<string, VRNotification> notClearedNotifications = vrAlertRuleNotificationManager.GetNotClearedAlertRuleNotifications(alertRuleTypeId, notificationTypeId, alertRuleId, eventKeys, DateTime.Now.Subtract(minNotificationInterval));

                VRAlertLevelManager vrAlertLevelManager = new VRAlertLevelManager();
                VRNotification tempNotification;

                List<DataRecordAlertRuleNotification> dataRecordAlertRuleNotificationsToBeExecuted;
                if (notClearedNotifications != null && notClearedNotifications.Count > 0)
                {
                    dataRecordAlertRuleNotificationsToBeExecuted = new List<DataRecordAlertRuleNotification>();

                    foreach (var dataRecordAlertRuleNotification in dataRecordAlertRuleNotifications)
                    {
                        if (!notClearedNotifications.TryGetValue(dataRecordAlertRuleNotification.GroupingKey, out tempNotification)
                            || vrAlertLevelManager.GetAlertLevelWeight(dataRecordAlertRuleNotification.AlertLevelId) > vrAlertLevelManager.GetAlertLevelWeight(tempNotification.AlertLevelId))
                        {
                            dataRecordAlertRuleNotificationsToBeExecuted.Add(dataRecordAlertRuleNotification);
                        }
                    }
                }
                else
                    dataRecordAlertRuleNotificationsToBeExecuted = dataRecordAlertRuleNotifications;

                VRNotificationTypeManager notificationTypeManager = new VRNotificationTypeManager();

                OnDataRecordNotificationCreatedAction onDataRecordNotificationCreatedAction = null;
                var notificationTypeExtendedSettings = notificationTypeManager.GetVRNotificationTypeExtendedSettings(notificationTypeId);
                var dataRecordNotificationTypeSettings = notificationTypeExtendedSettings as DataRecordNotificationTypeSettings;
                if (dataRecordNotificationTypeSettings != null && dataRecordNotificationTypeSettings.OnNotificationCreatedHandler != null)
                    onDataRecordNotificationCreatedAction = dataRecordNotificationTypeSettings.OnNotificationCreatedHandler.Action;

                if (dataRecordAlertRuleNotificationsToBeExecuted != null && dataRecordAlertRuleNotificationsToBeExecuted.Count > 0)
                {
                    createNotification = true;
                    numberOfNotificationsCreated = dataRecordAlertRuleNotificationsToBeExecuted.Count;

                    VRAlertRuleNotificationManager alertRuleNotificationManager = new VRAlertRuleNotificationManager();

                    foreach (DataRecordAlertRuleNotification dataRecordAlertRuleNotification in dataRecordAlertRuleNotificationsToBeExecuted)
                    {
                        CreateAlertRuleNotificationInput notificationInput = new CreateAlertRuleNotificationInput()
                        {
                            Actions = dataRecordAlertRuleNotification.Actions,
                            AlertLevelId = dataRecordAlertRuleNotification.AlertLevelId,
                            AlertRuleId = alertRuleId,
                            AlertRuleTypeId = alertRuleTypeId,
                            Description = alertNotificationDescription,
                            EventKey = dataRecordAlertRuleNotification.GroupingKey,
                            EventPayload = new DataRecordAlertRuleActionEventPayload()
                            {
                                DataRecordTypeId = dataRecordTypeId,
                                OutputRecords = dataRecordAlertRuleNotification.FieldValues
                            },
                            UserId = userId,
                            NotificationTypeId = notificationTypeId
                        };

                        var alertRuleNotificationOutput = alertRuleNotificationManager.CreateNotification(notificationInput);

                        if (onDataRecordNotificationCreatedAction != null)
                        {
                            alertRuleNotificationOutput.ThrowIfNull("alertRuleNotificationOutput");
                            alertRuleNotificationOutput.CreateVRNotificationOutput.ThrowIfNull("alertRuleNotificationOutput.CreateVRNotificationOutput");

                            Dictionary<NotificationActionMappingField, dynamic> enumFieldValuesByFieldName = new Dictionary<NotificationActionMappingField, dynamic>()
                            {
                                {NotificationActionMappingField.AlertNotificationDescription, alertNotificationDescription },
                                {NotificationActionMappingField.AlertRuleTypeId, alertRuleTypeId },
                                {NotificationActionMappingField.AlertRuleId, alertRuleId },
                                {NotificationActionMappingField.MinNotificationInterval, minNotificationInterval },
                                {NotificationActionMappingField.UserId, userId },
                                {NotificationActionMappingField.NotificationTypeId, notificationTypeId },
                                {NotificationActionMappingField.Notification, alertRuleNotificationOutput.CreateVRNotificationOutput.NotificationId },
                                {NotificationActionMappingField.AlertRuleLevelId,dataRecordAlertRuleNotification.AlertLevelId }
                            };

                            OnDataRecordNotificationCreatedExecutionContext executionActionContext = new OnDataRecordNotificationCreatedExecutionContext(dataRecordAlertRuleNotification.FieldValues, dataRecordTypeId, enumFieldValuesByFieldName);
                            onDataRecordNotificationCreatedAction.Execute(executionActionContext);
                        }
                    }
                }
            }
            return createNotification;
        }
    }
}