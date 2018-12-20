using Retail.BusinessEntity.Entities;
using System.Collections.Generic;
using Vanrise.Common.Business;

namespace Retail.BusinessEntity.Business
{
    public class BEConfigurationManager
    {
        public IEnumerable<PackageUsageVolumeRecurringPeriodConfig> GetPackageUsageVolumeRecurringPeriodConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<PackageUsageVolumeRecurringPeriodConfig>(PackageUsageVolumeRecurringPeriodConfig.EXTENSION_TYPE);
        }
    }
}
