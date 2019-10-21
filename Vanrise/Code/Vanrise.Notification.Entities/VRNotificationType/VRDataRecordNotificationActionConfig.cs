using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Notification.Entities
{
    public class VRDataRecordNotificationActionConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_Notification_VRDataRecordNotificationActionSettings";

        public string Editor { get; set; }
    }
}
