using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Notification.Entities;
using Vanrise.Common;
using Vanrise.Security.Entities;
using Vanrise.Security.Business;
using Vanrise.Entities;

namespace Vanrise.Notification.Business
{
    public class VRDataRecordNotificationTypeManager
    {
        public IEnumerable<VRDataRecordNotificationActionConfig> GetDataRecordNotificationActionConfigSettings()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<VRDataRecordNotificationActionConfig>(VRDataRecordNotificationActionConfig.EXTENSION_TYPE);
        }
    }
}
