using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace Retail.Teles.Business
{
    public class TelesRoutingGroupManager
    {
        public IEnumerable<RoutingGroupConditionConfig> GetRoutingGroupConditionConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<RoutingGroupConditionConfig>(RoutingGroupConditionConfig.EXTENSION_TYPE);
        }
    }
}
