using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Ericsson.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.RouteSync.Ericsson.Business
{
    public class SwitchLoggerManager
    {
        public IEnumerable<SwitchLoggerConfig> GetSwitchLoggerTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<SwitchLoggerConfig>(SwitchLoggerConfig.EXTENSION_TYPE);
        }
    }
}
