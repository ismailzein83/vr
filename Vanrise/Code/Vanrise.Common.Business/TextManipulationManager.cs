using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class TextManipulationManager
    {
        public IEnumerable<TextManipulationActionSettingsConfig> GetTextManipulationActionSettingsConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<TextManipulationActionSettingsConfig>(TextManipulationActionSettingsConfig.EXTENSION_TYPE);
        }
    }
}
