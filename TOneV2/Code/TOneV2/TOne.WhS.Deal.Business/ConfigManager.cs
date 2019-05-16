using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Deal.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.Deal.Business
{
    public class ConfigManager
    {
        #region Public Methods

        public DealTechnicalSettingData GetDealTechnicalSettingData()
        {
            SettingManager settingManager = new SettingManager();
            DealTechnicalSettingData dealTechnicalSettingData = settingManager.GetSetting<DealTechnicalSettingData>(Constants.DealTechnicalSettings);
            dealTechnicalSettingData.ThrowIfNull("dealTechnicalSettingData");

            return dealTechnicalSettingData;
        }

        public int GetDealTechnicalSettingIntervalOffsetInMinutes()
        {
            DealTechnicalSettingData dealTechnicalSettingData = GetDealTechnicalSettingData();
            return dealTechnicalSettingData.IntervalOffsetInMinutes;
        }

        public IEnumerable<DealSaleRateEvaluatorConfig> GetSaleRateEvaluatorConfigurationTemplateConfigs()
        {
            var extensionConfigManager = new ExtensionConfigurationManager();
            return extensionConfigManager.GetExtensionConfigurations<DealSaleRateEvaluatorConfig>(DealSaleRateEvaluatorConfig.EXTENSION_TYPE).OrderBy(x => x.Title);
        }

        public IEnumerable<DealSupplierRateEvaluatorConfig> GetSupplierRateEvaluatorConfigurationTemplateConfigs()
        {
            var extensionConfigManager = new ExtensionConfigurationManager();
            return extensionConfigManager.GetExtensionConfigurations<DealSupplierRateEvaluatorConfig>(DealSupplierRateEvaluatorConfig.EXTENSION_TYPE).OrderBy(x => x.Title);
        }

        #endregion
    }
}