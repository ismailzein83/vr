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

        public DealTechnicalSettingData GetDealTechnicalSettingData()
        {
            SettingManager settingManager = new SettingManager();
            DealTechnicalSettingData dealTechnicalSettingData = settingManager.GetSetting<DealTechnicalSettingData>(Constants.DealTechnicalSettings);
            dealTechnicalSettingData.ThrowIfNull("dealTechnicalSettingData");

            return dealTechnicalSettingData;
        }

        public int GetDealTechnicalSettingIntervalOffset()
        {
            DealTechnicalSettingData dealTechnicalSettingData = GetDealTechnicalSettingData();
            return dealTechnicalSettingData.IntervalOffset;
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
