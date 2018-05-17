using System;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.Business
{
    public class ConfigManager
    {
        public int GetRouteSyncProcessRouteBatchSize()
        {
            RouteSyncProcess routeSyncProcess = GetRouteSyncProcessSettings();
            return routeSyncProcess.RouteBatchSize;
        }

        public int GetRouteSyncProcessDifferentialRoutesPerTransaction()
        {
            RouteSyncProcess routeSyncProcess = GetRouteSyncProcessSettings();
            return routeSyncProcess.DifferentialRoutesPerTransaction;
        }

        public int GetRouteSyncProcessIndexesCommandTimeoutInSeconds()
        {
            RouteSyncProcess routeSyncProcess = GetRouteSyncProcessSettings();
            return routeSyncProcess.IndexCommandTimeoutInMinutes * 60;
        }

        public bool GetRouteSyncProcessExecuteFullRouteSyncWhenPartialNotSupported()
        {
            RouteSyncProcess routeSyncProcess = GetRouteSyncProcessSettings();
            return routeSyncProcess.ExecuteFullRouteSyncWhenPartialNotSupported;
        }

        public SwitchInfoGetter GetSwitchInfoGetter()
        {
            Vanrise.Common.Business.SettingManager settingManager = new Vanrise.Common.Business.SettingManager();
            return settingManager.GetCachedOrCreate("WhS_RouteSync_ConfigurationManager_GetSwitchInfoGetter",
                () =>
                {
                    var routeSyncTechnicalSettings = GetRouteSyncTechnicalSettings(settingManager);
                    if (routeSyncTechnicalSettings.SwitchInfoGetter == null)
                        throw new NullReferenceException("routeSyncTechnicalSettings.SwitchInfoGetter");
                    return routeSyncTechnicalSettings.SwitchInfoGetter;
                });
        }

        private RouteSyncProcess GetRouteSyncProcessSettings()
        {
            Vanrise.Common.Business.SettingManager settingManager = new Vanrise.Common.Business.SettingManager();
            return settingManager.GetCachedOrCreate("WhS_RouteSync_ConfigurationManager_GetRouteSyncProcessSettings",
                () =>
                {
                    var routeSyncSettings = GetRouteSyncSettings(settingManager);
                    if (routeSyncSettings.RouteSyncProcess == null)
                        throw new NullReferenceException("routeSyncSettings.RouteSyncProcess");
                    return routeSyncSettings.RouteSyncProcess;
                });
        }

        private RouteSyncTechnicalSettings GetRouteSyncTechnicalSettings(Vanrise.Common.Business.SettingManager settingManager)
        {
            var routeSyncTechnicalSettings = settingManager.GetSetting<RouteSyncTechnicalSettings>(RouteSyncTechnicalSettings.SETTING_TYPE);
            if (routeSyncTechnicalSettings == null)
                throw new NullReferenceException("routeSyncTechnicalSettings");
            return routeSyncTechnicalSettings;
        }

        private RouteSyncSettings GetRouteSyncSettings(Vanrise.Common.Business.SettingManager settingManager)
        {
            var routeSyncSettings = settingManager.GetSetting<RouteSyncSettings>(RouteSyncSettings.SETTING_TYPE);
            if (routeSyncSettings == null)
                throw new NullReferenceException("routeSyncSettings");
            return routeSyncSettings;
        }
    }
}
