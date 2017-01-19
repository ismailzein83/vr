using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.Sales.Business
{
    public class RatePlanExtensionConfigManager
    {
        public IEnumerable<CostCalculationMethodSetting> GetCostCalculationMethodTemplates()
        {
            var extensionConfigManager = new ExtensionConfigurationManager();
            return extensionConfigManager.GetExtensionConfigurations<CostCalculationMethodSetting>(CostCalculationMethodSetting.EXTENSION_TYPE).OrderBy(x => x.Title);
        }

        public IEnumerable<RateCalculationMethodSetting> GetRateCalculationMethodTemplates()
        {
            var extensionConfigManager = new ExtensionConfigurationManager();
            return extensionConfigManager.GetExtensionConfigurations<RateCalculationMethodSetting>(RateCalculationMethodSetting.EXTENSION_TYPE).OrderBy(x => x.Title);
        }

		public IEnumerable<BulkActionTypeSettings> GetBulkActionTypeExtensionConfigs(SalePriceListOwnerType ownerType)
		{
			var extensionConfigManager = new ExtensionConfigurationManager();

			Func<BulkActionTypeSettings, bool> filterFunc = (bulkActionTypeSettings) =>
			{
				return (ownerType == SalePriceListOwnerType.SellingProduct) ? bulkActionTypeSettings.IsApplicableToSellingProduct : bulkActionTypeSettings.IsApplicableToCustomer;
			};
			
			return extensionConfigManager.GetExtensionConfigurations<BulkActionTypeSettings>(BulkActionTypeSettings.EXTENSION_TYPE).FindAllRecords(filterFunc).OrderBy(x => x.Title);
		}

		public IEnumerable<BulkActionZoneFilterTypeSettings> GetBulkActionZoneFilterTypeExtensionConfigs()
		{
			var extensionConfigManager = new ExtensionConfigurationManager();
			return extensionConfigManager.GetExtensionConfigurations<BulkActionZoneFilterTypeSettings>(BulkActionZoneFilterTypeSettings.EXTENSION_TYPE).OrderBy(x => x.Title);
		}
    }
}
