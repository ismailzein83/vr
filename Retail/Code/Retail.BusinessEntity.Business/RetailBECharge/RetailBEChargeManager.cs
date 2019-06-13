using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace Retail.BusinessEntity.Business
{
    public class RetailBEChargeManager
    {
        public IEnumerable<RetailBEChargeSettingsConfig> GetRetailBEChargeSettingsConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<RetailBEChargeSettingsConfig>(RetailBEChargeSettingsConfig.EXTENSION_TYPE);
        }
    }
}
