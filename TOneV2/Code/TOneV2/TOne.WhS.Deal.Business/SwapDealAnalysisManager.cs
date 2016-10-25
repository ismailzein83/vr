using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.Business
{
    public class SwapDealAnalysisManager
    {
        public SwapDealAnalysisResult AnalyzeDeal(SwapDealAnalysisSettings analysisSettings)
        {
            throw new NotImplementedException();
        }

		public IEnumerable<SwapDealAnalysisOutboundRateCalcMethodConfig> GetOutboundRateCalcMethodExtensionConfigs()
		{
			var extensionConfigManager = new Vanrise.Common.Business.ExtensionConfigurationManager();
			return extensionConfigManager.GetExtensionConfigurations<SwapDealAnalysisOutboundRateCalcMethodConfig>(SwapDealAnalysisOutboundRateCalcMethodConfig.EXTENSION_TYPE).OrderBy(x => x.Title);
		}
    }
}
