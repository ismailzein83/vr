using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.RouteSync.Business
{
    public class SwitchRouteSynchronizerManager
    {
        public IEnumerable<SwitchRouteSynchronizerConfig> GetSwitchRouteSynchronizerExtensionConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<SwitchRouteSynchronizerConfig>(SwitchRouteSynchronizerConfig.EXTENSION_TYPE);
        }
    }
}
