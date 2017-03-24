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
        void GetFirstPageVRNotifications(IVRNotificationFirstPageContext context);
        List<VRNotification> GetUpdateVRNotifications(IVRNotificationUpdateContext context);
        void GetBeforeIdVRNotifications(IVRNotificationBeforeIdContext context);
    }
}
