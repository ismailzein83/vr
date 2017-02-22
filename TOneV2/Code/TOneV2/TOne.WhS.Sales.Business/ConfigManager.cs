using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.Sales.Business
{
    public class ConfigManager
    {
        #region Public Methods

        public int GetNewRateDayOffset()
        {
            RatePlanSettingsData ratePlanSettings = GetRatePlanSettings();
            return ratePlanSettings.NewRateDayOffset;
        }

        public int GetIncreasedRateDayOffset()
        {
            RatePlanSettingsData ratePlanSettings = GetRatePlanSettings();
            return ratePlanSettings.IncreasedRateDayOffset;
        }

        public int GetDecreasedRateDayOffset()
        {
            RatePlanSettingsData ratePlanSettings = GetRatePlanSettings();
            return ratePlanSettings.DecreasedRateDayOffset;
        }

        #endregion

        #region Private Methods

        private RatePlanSettingsData GetRatePlanSettings()
        {
            var settingManager = new SettingManager();
            RatePlanSettingsData ratePlanSettings = settingManager.GetSetting<RatePlanSettingsData>(Constants.RatePlanSettingsType);
            if (ratePlanSettings == null)
                throw new Vanrise.Entities.DataIntegrityValidationException("RatePlanSettings were not found");
            return ratePlanSettings;
        }

        #endregion
    }
}
