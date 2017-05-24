using System;
using TOne.WhS.Deal.Entities;
using Vanrise.Common.Business;
using Vanrise.Common;

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
