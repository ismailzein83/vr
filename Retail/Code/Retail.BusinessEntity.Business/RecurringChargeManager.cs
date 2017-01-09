using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace Retail.BusinessEntity.Business
{
    public class RecurringChargeManager
    {
        public IEnumerable<AccountRecurringChargeEvaluatorConfig> GetAccountRecurringChargeEvaluatorExtensionConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<AccountRecurringChargeEvaluatorConfig>(AccountRecurringChargeEvaluatorConfig.EXTENSION_TYPE);
        }

        public IEnumerable<AccountRecurringChargeRuleSetSettingsConfig> GetAccountRecurringChargeRuleSetSettingsExtensionConfigs()  
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<AccountRecurringChargeRuleSetSettingsConfig>(AccountRecurringChargeRuleSetSettingsConfig.EXTENSION_TYPE); 
        }
    }
}
 