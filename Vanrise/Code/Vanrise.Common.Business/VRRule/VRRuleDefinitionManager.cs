using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRRuleDefinitionManager
    {
        public IEnumerable<VRRuleDefinitionExtendedSettingsConfig> GetVRRuleDefinitionExtendedSettingsConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<VRRuleDefinitionExtendedSettingsConfig>(VRRuleDefinitionExtendedSettingsConfig.EXTENSION_TYPE);
        }
    }
}
