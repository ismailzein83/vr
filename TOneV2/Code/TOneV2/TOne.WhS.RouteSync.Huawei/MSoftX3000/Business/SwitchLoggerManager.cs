using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using TOne.WhS.RouteSync.Huawei.Entities;


namespace TOne.WhS.RouteSync.Huawei.Business
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
