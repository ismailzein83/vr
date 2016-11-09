using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.Business
{
    public class VRNotificationTypeManager
    {
        Vanrise.Common.Business.VRComponentTypeManager _vrComponentTypeManager = new Common.Business.VRComponentTypeManager();

        public VRNotificationType GetNotificationType(Guid notificationTypeId)
        {
            return _vrComponentTypeManager.GetComponentType<VRNotificationTypeSettings, VRNotificationType>(notificationTypeId);
        }

        public VRNotificationTypeSettings GetNotificationTypeSettings(Guid notificationTypeId)
        {
            return _vrComponentTypeManager.GetComponentTypeSettings<VRNotificationTypeSettings>(notificationTypeId);
        }
    }
}
