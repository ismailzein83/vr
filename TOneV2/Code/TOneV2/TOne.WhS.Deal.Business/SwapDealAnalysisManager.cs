using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;
using TOne.WhS.Deal.Entities.Settings;
using Vanrise.Common.Business;
using Vanrise.Entities;

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

		public SwapDealAnalysisSettingData GetSwapDealAnalysisSettingData()
		{
			var settingManager = new SettingManager();
			Setting setting = settingManager.GetSettingByType(Constants.SwapDealAnalysisSettings);
			if (setting == null)
				throw new NullReferenceException("setting");
			if (setting.Data == null)
				throw new NullReferenceException("setting.Data");
			var swapDealAnalysisSettingData = setting.Data as SwapDealAnalysisSettingData;
			if (swapDealAnalysisSettingData == null)
				throw new NullReferenceException("swapDealAnalysisSettingData");
			return swapDealAnalysisSettingData;
		}
    }
}
