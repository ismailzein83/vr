using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using TOne.WhS.Jazz.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.Jazz.Business
{
    public class ConfigManager
    {
        public Guid GetJazzTechnicalSettings()
        {
            SettingManager settingManager = new SettingManager();
            JazzTechnicalSettingData jazzTechnicalSettings = settingManager.GetSetting<JazzTechnicalSettingData>(JazzTechnicalSettingData.SETTING_TYPE);

            jazzTechnicalSettings.ThrowIfNull("Jazz Technical Settings");

            return jazzTechnicalSettings.AnalyticTableId;
        }
    }
}


