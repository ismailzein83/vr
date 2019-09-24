using Retail.Billing.Entities;
using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Retail.Billing.Business
{
    public class RetailBillingChargeTypeManager
    {
        public IEnumerable<RetailBillingChargeTypeExtendedSettingsConfig> GetChargeTypeExtendedSettingsConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<RetailBillingChargeTypeExtendedSettingsConfig>(RetailBillingChargeTypeExtendedSettingsConfig.EXTENSION_TYPE);
        }
    }
}
