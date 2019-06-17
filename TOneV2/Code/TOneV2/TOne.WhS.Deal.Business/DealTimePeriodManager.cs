using System.Collections.Generic;
using TOne.WhS.Deal.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.Deal.Business
{
    public class DealTimePeriodManager
    {
        public IEnumerable<DealTimePeriodConfig> GetDealTimePeriodTemplateConfigs()
        {
            var extensionConfigManager = new ExtensionConfigurationManager();
            return extensionConfigManager.GetExtensionConfigurations<DealTimePeriodConfig>(DealTimePeriodConfig.EXTENSION_TYPE);
        }
    }
}