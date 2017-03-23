using System;
using System.Collections.Generic;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.Data
{
    public interface IVRNotificationDataManager : IDataManager
    {
        bool Insert(VRNotification notification, out long notificationId);
        VRNotification GetVRNotificationById(long notificationId);
        List<VRNotification> GetVRNotifications(Guid notificationTypeId, VRNotificationParentTypes parentTypes, string eventKey);
        void UpdateNotificationStatus(long notificationId, VRNotificationStatus vrNotificationStatus);
        void GetFirstPageVRNotifications(VRNotificationFirstPageInput input, long? nbOfRows, ref byte[] maxTimeStamp, Func<VRNotification, bool> onItemReady);
        void GetUpdateVRNotifications(VRNotificationUpdateInput input, ref byte[] maxTimeStamp, Action<VRNotification> onItemReady);
        void GetBeforeIdVRNotifications(VRNotificationBeforeIdInput input, long? nbOfRows, Func<VRNotification, bool> onItemReady);
    }
}
