using System;
using TOne.WhS.Deal.Entities;
using Vanrise.Common.Business;
using Vanrise.Common;
using System.Collections.Generic;
using System.Linq;

namespace TOne.WhS.Deal.Business
{
    public class ConfigManager
    {
        #region Public Methods

        public Guid GetSwapDealBuyRouteRuleDefinitionId()
        {
            SwapDealTechnicalSettingData swapDealTechnicalSettingData = GetSwapDealTechnicalSettingData();
            return swapDealTechnicalSettingData.SwapDealBuyRouteRuleDefinitionId;
        }

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

        #region private methods

        private SwapDealTechnicalSettingData GetSwapDealTechnicalSettingData()
        {
            SettingManager settingManager = new SettingManager();
            SwapDealTechnicalSettingData swapDealTechnicalSettingData = settingManager.GetSetting<SwapDealTechnicalSettingData>(Constants.SwapDealTechnicalSettings);
            swapDealTechnicalSettingData.ThrowIfNull("swapDealTechnicalSettingData");

            return swapDealTechnicalSettingData;
        }

        #endregion
    }
}
