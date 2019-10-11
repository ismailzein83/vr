using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using TOne.WhS.RouteSync.Huawei.SoftX3000.Entities;


namespace TOne.WhS.RouteSync.Huawei.SoftX3000.Business
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
