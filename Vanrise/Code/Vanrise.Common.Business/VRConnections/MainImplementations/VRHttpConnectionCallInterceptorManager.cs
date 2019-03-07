using System.Collections.Generic;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRHttpConnectionCallInterceptorManager
    {
        public IEnumerable<VRHttpConnectionCallInterceptorConfig> GetHttpConnectionCallInterceptorTemplateConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<VRHttpConnectionCallInterceptorConfig>(VRHttpConnectionCallInterceptorConfig.EXTENSION_TYPE);
        }
    }
}
