using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Notification.Entities
{
    public class VRNotificationTypeDefinitionConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_Notification_VRNotificationTypeSettings";

        public string Editor { get; set; }
    }
}
