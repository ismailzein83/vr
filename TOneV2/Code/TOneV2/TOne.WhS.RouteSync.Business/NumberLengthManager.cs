using System.Collections.Generic;
using TOne.WhS.RouteSync.Entities.NumberLength;
using Vanrise.Common.Business;

namespace TOne.WhS.RouteSync.Business
{
    public class NumberLengthManager
    {
        public IEnumerable<NumberLengthEvaluatorConfig> GetNumberLengthEvaluatorExtensionConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<NumberLengthEvaluatorConfig>(NumberLengthEvaluatorConfig.EXTENSION_TYPE);
        }
    }
}
