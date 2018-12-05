using System;
using Retail.RA.Entities;
using System.Collections.Generic;


namespace Retail.RA.Business
{
    public class OperatorDeclarationServicesManager
    {
        public IEnumerable<OperatorDirectionServicesMappedCellExtensionConfiguration> GetMappedCellsExtensionConfigs()
        {
            var extensionConfigManager = new Vanrise.Common.Business.ExtensionConfigurationManager();
            return extensionConfigManager.GetExtensionConfigurations<OperatorDirectionServicesMappedCellExtensionConfiguration>(OperatorDirectionServicesMappedCellExtensionConfiguration.EXTENSION_TYPE);
        }
    }
}
