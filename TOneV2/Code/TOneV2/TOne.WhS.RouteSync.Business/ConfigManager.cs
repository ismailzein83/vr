using System;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Common;

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
		
		public RouteSynchronizerSwitchSettings GetRouteSynchronizerSwitchSettings(Guid configId)
		{
			var switchSettingsByConfigId = GetRouteSynchronizerSwitchSettings();
			var switchSettings = switchSettingsByConfigId.GetRecord(configId);
			switchSettings.ThrowIfNull(string.Format("No RouteSynchronizerSwitchSettings found with config id {0}", configId));
			return switchSettings;
		}

		private Dictionary<Guid, RouteSynchronizerSwitchSettings> GetRouteSynchronizerSwitchSettings()
		{
			Vanrise.Common.Business.SettingManager settingManager = new Vanrise.Common.Business.SettingManager();
			return settingManager.GetCachedOrCreate("WhS_RouteSync_ConfigurationManager_GetRouteSynchronizerSwitchSettings",
				() =>
				{
					var routeSyncSettings = GetRouteSyncSettings(settingManager);
					if (routeSyncSettings.SwitchSettingsByConfigId == null)
						throw new NullReferenceException("routeSyncSettings.SwitchSettingsByConfigId");
					return routeSyncSettings.SwitchSettingsByConfigId;
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
