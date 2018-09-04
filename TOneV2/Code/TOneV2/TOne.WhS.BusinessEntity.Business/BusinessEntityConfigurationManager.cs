using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class BusinessEntityConfigurationManager
    {
        public IEnumerable<CodeListResolverConfig> GetCodeListResolverSettingsTemplates()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<CodeListResolverConfig>(CodeListResolverConfig.EXTENSION_TYPE);
        }

        public IEnumerable<ExcludedDestinationsConfig> GetExcludedDestinationsTemplates()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<ExcludedDestinationsConfig>(ExcludedDestinationsConfig.EXTENSION_TYPE);
        }
    }
}
