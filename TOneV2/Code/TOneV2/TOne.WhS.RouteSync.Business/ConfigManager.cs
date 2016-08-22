using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.Business
{
    public class ConfigManager
    {
        public SwitchInfoGetter GetSwitchInfoGetter()
        {
            Vanrise.Common.Business.SettingManager settingManager = new Vanrise.Common.Business.SettingManager();
            return settingManager.GetCachedOrCreate("WhS_RouteSync_ConfigurationManager_GetSwitchInfoGetter",
                () =>
                {
                    var routeSyncTechnicalSettings = GetTechnicalSettings(settingManager);
                    if (routeSyncTechnicalSettings.SwitchInfoGetter == null)
                        throw new NullReferenceException("routeSyncTechnicalSettings.SwitchInfoGetter");
                    return routeSyncTechnicalSettings.SwitchInfoGetter;
                });
        }

        private RouteSyncTechnicalSettings GetTechnicalSettings(Vanrise.Common.Business.SettingManager settingManager)
        {
            var routeSyncTechnicalSettings = settingManager.GetSetting<RouteSyncTechnicalSettings>(RouteSyncTechnicalSettings.SETTING_TYPE);
            if (routeSyncTechnicalSettings == null)
                throw new NullReferenceException("routeSyncTechnicalSettings");
            return routeSyncTechnicalSettings;
        }
    }
}
