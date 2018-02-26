using System;
using System.Collections.Generic;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.Data
{
    public interface IVRNotificationDataManager : IDataManager
    {
        bool Insert(VRNotification notification, out long notificationId);
        VRNotification GetVRNotificationById(long notificationId);
        List<VRNotification> GetVRNotifications(Guid notificationTypeId, VRNotificationParentTypes parentTypes, string eventKey, List<VRNotificationStatus> statusesToLoad);
        void UpdateNotificationStatus(long notificationId, VRNotificationStatus vrNotificationStatus, IVRActionRollbackEventPayload rollbackEventPayload, long? executeBPInstanceId, long? clearBPInstanceId);
        void GetFirstPageVRNotifications(IVRNotificationFirstPageContext context);
        List<VRNotification> GetUpdateVRNotifications(IVRNotificationUpdateContext context);
        void GetBeforeIdVRNotifications(IVRNotificationBeforeIdContext context);
        List<VRNotification> GetNotClearedNotifications(Guid notificationTypeId, VRNotificationParentTypes parentTypes, List<string> eventKeys, DateTime? notificationCreatedAfter);
    }
}