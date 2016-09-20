using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
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
    }
}
