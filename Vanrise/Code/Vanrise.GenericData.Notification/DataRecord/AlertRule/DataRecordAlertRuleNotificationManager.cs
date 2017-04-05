using System;
using System.Collections.Generic;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;

namespace Vanrise.GenericData.Notification
{
    public class DataRecordAlertRuleNotificationManager
    {
        public void CreateAlertRuleNotifications(List<DataRecordAlertRuleNotification> dataRecordAlertRuleNotifications, List<string> eventKeys, Guid alertRuleTypeId, 
            Guid notificationTypeId, long alertRuleId, TimeSpan minNotificationInterval, Guid dataRecordTypeId, string alertNotificationDescription, int userId)
        {
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

                if (dataRecordAlertRuleNotificationsToBeExecuted != null && dataRecordAlertRuleNotificationsToBeExecuted.Count > 0)
                {
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
                        alertRuleNotificationManager.CreateNotification(notificationInput);
                    }
                }
            }
        }
    }
}