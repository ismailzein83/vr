using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRPushNotificationHandler : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_Common_PushNotificationHandler";

        public bool IsEnabled { get; set; }

        
        public VRPushNotificationHandlerSettings HandlerSettings { get; set; }
    }
}
