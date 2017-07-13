using Retail.Cost.Entities;
using System;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Retail.Cost.Business
{
    public class ConfigManager
    {
        #region Public Methods

        public TimeSpan GetDurationMargin()
        {
            CDRCostSettingData cdrCostSettingData = GetCDRCostSettingData();
            return cdrCostSettingData.DurationMargin;
        }

        public TimeSpan GetAttemptDateTimeMargin()
        {
            CDRCostSettingData cdrCostSettingData = GetCDRCostSettingData();
            return cdrCostSettingData.AttemptDateTimeMargin;
        }

        public TimeSpan GetAttemptDateTimeOffset()
        {
            CDRCostSettingData cdrCostSettingData = GetCDRCostSettingData();
            return cdrCostSettingData.AttemptDateTimeOffset;
        }

        public int GetMaxBatchDurationInMinutes()
        {
            CDRCostSettingData cdrCostSettingData = GetCDRCostSettingData();
            return cdrCostSettingData.MaxBatchDurationInMinutes;
        }

        #endregion

        #region private methods

        private CDRCostSettingData GetCDRCostSettingData()
        {
            SettingManager settingManager = new SettingManager();
            CDRCostSettingData cdrCostSettingData = settingManager.GetSetting<CDRCostSettingData>(Constants.CDRCostSettings);
            cdrCostSettingData.ThrowIfNull("costSettingData");

            return cdrCostSettingData;
        }

        #endregion
    }
}
