using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                StringBuilder scriptBuilder = new StringBuilder();
                SettingManager settingManager = new SettingManager();
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
                    if (scriptBuilder.Length > 0)
                    {
                        scriptBuilder.Append(",");
                        scriptBuilder.AppendLine();
                    }
                    scriptBuilder.AppendFormat(@"('{0}','{1}','{2}','{3}','{4}','{5}',{6})", setting.SettingId, setting.Name,setting.Type, setting.Category, Serializer.Serialize(setting.Settings, true), Serializer.Serialize(setting.Data), setting.IsTechnical ? 1 : 0);
                }
                string script = String.Format(@"set nocount on;
;with cte_data([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical]))
merge	[common].[Setting] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Type] = s.[Type],[Category] = s.[Category],[Settings] = s.[Settings],[Data] = s.[Data],[IsTechnical] = s.[IsTechnical]
when not matched by target then
	insert([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
	values(s.[ID],s.[Name],s.[Type],s.[Category],s.[Settings],s.[Data],s.[IsTechnical]);", scriptBuilder);
                context.AddEntityScript("[common].[Setting]", script);
            }
        }

        #endregion
    }
}
