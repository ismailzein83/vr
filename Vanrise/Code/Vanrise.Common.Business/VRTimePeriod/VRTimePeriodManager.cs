using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRTimePeriodManager
    {
        public IEnumerable<VRTimePeriodConfig> GetVRTimePeriodConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<VRTimePeriodConfig>(VRTimePeriodConfig.EXTENSION_TYPE);
        }
    }
}
