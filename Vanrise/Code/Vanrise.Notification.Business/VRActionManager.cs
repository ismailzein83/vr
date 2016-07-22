using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.Business
{
    public class VRActionManager
    {
        public void CreateAction(CreateVRActionInput createActionInput)
        {

        }

        public IEnumerable<VRActionConfig> GetVRActionConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<VRActionConfig>(VRActionConfig.EXTENSION_TYPE);
        }
    }
}
