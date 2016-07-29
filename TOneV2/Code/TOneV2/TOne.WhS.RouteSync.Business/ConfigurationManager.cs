using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.Business
{
    public class ConfigurationManager
    {
        public SwitchInfoGetter GetSwitchInfoGetter()
        {
            Vanrise.Common.Business.SettingManager settingManager = new Vanrise.Common.Business.SettingManager();
            return settingManager.GetCachedOrCreate("WhS_RouteSync_ConfigurationManager_GetSwitchInfoGetter",
                () =>
                {
                    var routeSyncTechnicalSettings = GetTechnicalSettings(settingManager);
                    if (routeSyncTechnicalSettings.SwitchInfoGetterFQTN == null)
                        throw new NullReferenceException("routeSyncTechnicalSettings.SwitchInfoGetterFQTN");
                    Type switchInfoGetterType = Type.GetType(routeSyncTechnicalSettings.SwitchInfoGetterFQTN);
                    if (switchInfoGetterType == null)
                        throw new NullReferenceException(String.Format("switchInfoGetterType '{0}'", routeSyncTechnicalSettings.SwitchInfoGetterFQTN));
                    SwitchInfoGetter switchInfoGetter = Activator.CreateInstance(switchInfoGetterType) as SwitchInfoGetter;
                    if (switchInfoGetter == null)
                        throw new NullReferenceException(String.Format("switchInfoGetter '{0}'", routeSyncTechnicalSettings.SwitchInfoGetterFQTN));
                    return switchInfoGetter;
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
