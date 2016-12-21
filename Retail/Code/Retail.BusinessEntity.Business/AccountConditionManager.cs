using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace Retail.BusinessEntity.Business
{
    public class AccountConditionManager
    {
        public IEnumerable<AccountConditionConfig> GetAccountConditionConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<AccountConditionConfig>(AccountConditionConfig.EXTENSION_TYPE);
        }
    }
}
