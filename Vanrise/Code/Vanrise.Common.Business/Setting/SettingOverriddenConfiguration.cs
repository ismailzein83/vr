using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class SettingOverriddenConfiguration : OverriddenConfigurationExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("955CB0A1-A801-428F-8430-C046C0D3EAD3"); }
        }

        public Guid SettingId { get; set; }

        public string OverriddenName { get; set; }

        public string OverriddenCategory { get; set; }

        public object OverriddenData { get; set; }

        public override Type GetBehaviorType(IOverriddenConfigurationGetBehaviorContext context)
        {
            return typeof(SettingOverriddenConfigurationBehavior);
        }

        #region Private Methods

        private class SettingOverriddenConfigurationBehavior : OverriddenConfigurationBehavior
        {
            public override void GenerateScript(IOverriddenConfigurationBehaviorGenerateScriptContext context)
            {
                SettingManager settingManager = new SettingManager();
                List<Setting> settings = new List<Setting>();
                foreach (var config in context.Configs)
                {
                    SettingOverriddenConfiguration settingConfig = config.Settings.ExtendedSettings.CastWithValidate<SettingOverriddenConfiguration>("mailTypeConfig", config.OverriddenConfigurationId);

                    var setting = settingManager.GetSetting(settingConfig.SettingId);
                    setting.ThrowIfNull("setting", settingConfig.SettingId);
                    setting = setting.VRDeepCopy();
                    if (!String.IsNullOrEmpty(settingConfig.OverriddenName))
                        setting.Name = settingConfig.OverriddenName;
                    if (!String.IsNullOrEmpty(settingConfig.OverriddenCategory))
                        setting.Category = settingConfig.OverriddenCategory;
                    if (settingConfig.OverriddenData!= null)
                        setting.Data = settingConfig.OverriddenData;
                    settings.Add(setting);
                    
                }
                GenerateScript(settings, context.AddEntityScript);
            }

            public override void GenerateDevScript(IOverriddenConfigurationBehaviorGenerateDevScriptContext context)
            {
                IEnumerable<Guid> ids = context.Configs.Select(config => config.Settings.ExtendedSettings.CastWithValidate<SettingOverriddenConfiguration>("config.Settings.ExtendedSettings", config.OverriddenConfigurationId).SettingId).Distinct();
                SettingManager settingManager = new SettingManager();
                List<Setting> settings = new List<Setting>();
                foreach (var id in ids)
                {
                    var setting = settingManager.GetSetting(id);
                    setting.ThrowIfNull("setting", id);
                    settings.Add(setting);

                }
                GenerateScript(settings, context.AddEntityScript);
            }

            private void GenerateScript(List<Setting> settings, Action<string, string> addEntityScript)
            {
                ISettingDataManager dataManager = CommonDataManagerFactory.GetDataManager<ISettingDataManager>();
                dataManager.GenerateScript(settings, addEntityScript);
            }
        }

        #endregion
    }
}
