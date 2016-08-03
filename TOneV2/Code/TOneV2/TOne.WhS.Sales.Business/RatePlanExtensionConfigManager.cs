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
            return extensionConfigManager.GetExtensionConfigurations<CostCalculationMethodSetting>(Constants.CostCalculationMethod).OrderBy(x => x.Title);
        }

        public IEnumerable<RateCalculationMethodSetting> GetRateCalculationMethodTemplates()
        {
            var extensionConfigManager = new ExtensionConfigurationManager();
            return extensionConfigManager.GetExtensionConfigurations<RateCalculationMethodSetting>(Constants.RateCalculationMethod).OrderBy(x => x.Title);
        }
    }
}
