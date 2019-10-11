using System.Collections.Generic;
using TOne.WhS.RouteSync.Entities.CodeCharge;
using Vanrise.Common.Business;

namespace TOne.WhS.RouteSync.Business
{
    public class CodeChargeManager
    {
        public IEnumerable<CodeChargeEvaluatorConfig> GetCodeChargeEvaluatorExtensionConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<CodeChargeEvaluatorConfig>(CodeChargeEvaluatorConfig.EXTENSION_TYPE);
        }
    }
}
