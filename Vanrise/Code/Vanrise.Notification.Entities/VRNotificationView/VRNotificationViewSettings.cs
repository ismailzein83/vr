using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Notification.Entities
{
    public class VRNotificationViewSettings : ViewSettings
    {
        public List<VRNotificationViewSettingItem> Settings { get; set; }
        public override string GetURL(View view)
        {
            return String.Format("#/viewwithparams/VR_Notification/Views/VRNotification/VRNotificationLog/{{\"viewId\":\"{0}\"}}", view.ViewId);
        }
    }

    public class VRNotificationViewSettingItem
    {
        public Guid VRNotificationTypeId { get; set; }
    }
}
