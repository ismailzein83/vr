using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;
using Vanrise.Common.Business;

namespace Retail.BusinessEntity.Business
{
    public class AccountSynchronizerHandlerManager
    {
        public IEnumerable<AccountSynchronizerInsertHandlerConfig> GetAccountSynchronizerInsertHandlerConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<AccountSynchronizerInsertHandlerConfig>(AccountSynchronizerInsertHandlerConfig.EXTENSION_TYPE);
        }
    }
}
