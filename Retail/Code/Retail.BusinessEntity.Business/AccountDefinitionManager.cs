using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace Retail.BusinessEntity.Business
{
    public class AccountDefinitionManager
    {
        public IEnumerable<AccountViewDefinitionSettingsConfig> GetAccountViewDefinitionSettingsConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<AccountViewDefinitionSettingsConfig>(AccountViewDefinitionSettingsConfig.EXTENSION_TYPE);
         }
    }
}
