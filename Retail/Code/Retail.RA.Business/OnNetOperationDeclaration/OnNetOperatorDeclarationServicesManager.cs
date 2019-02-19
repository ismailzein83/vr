using Retail.RA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.RA.Business
{
    public class OnNetOperatorDeclarationServicesManager
    {
        public IEnumerable<OnNetOperatorDirectionServicesMappedCellExtensionConfiguration> GetMappedCellsExtensionConfigs()
        {
            var extensionConfigManager = new Vanrise.Common.Business.ExtensionConfigurationManager();
            return extensionConfigManager.GetExtensionConfigurations<OnNetOperatorDirectionServicesMappedCellExtensionConfiguration>(OnNetOperatorDirectionServicesMappedCellExtensionConfiguration.EXTENSION_TYPE);
        }
    }
}
