using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.Data
{
    public interface IVRNotificationDataManager : IDataManager
    {
        bool Insert(VRNotification notification, out long notificationId);
        VRNotification GetVRNotificationById(long notificationId);
        List<VRNotification> GetVRNotifications(Guid notificationTypeId, VRNotificationParentTypes parentTypes, string eventKey);
        void UpdateNotificationStatus(long notificationId, VRNotificationStatus vrNotificationStatus);
        List<VRNotification> GetUpdateVRNotifications(VRNotificationUpdateQuery input);
        List<VRNotification> GetBeforeIdVRNotifications(VRNotificationBeforeIdQuery input);
    }
}
