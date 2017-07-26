using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class SettingDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ISettingDataManager
    {
        public SettingDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        public IEnumerable<Setting> GetSettings()
        {
            return GetItemsSP("common.sp_Setting_GetAll", SettingMapper);
        }

        public bool UpdateSetting(SettingToEdit setting)
        {
            return ExecuteNonQuerySP("common.sp_Setting_Update", setting.SettingId, setting.Name, Vanrise.Common.Serializer.Serialize(setting.Data)) > 0;
        }

        public bool AreSettingsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("common.Setting", ref updateHandle);
        }

        public void GenerateScript(List<Setting> settings, Action<string, string> addEntityScript)
        {
            StringBuilder scriptBuilder = new StringBuilder();
            var technicalSettings = settings.Where(s => s.IsTechnical).ToList();
            if (technicalSettings.Count > 0)
                scriptBuilder.Append(GenerateSettingsScript(technicalSettings, true));
            var nonTechnicalSettings = settings.Where(s => !s.IsTechnical).ToList();
            if (nonTechnicalSettings.Count > 0)
            {
                if (scriptBuilder.Length > 0)
                {
                    scriptBuilder.AppendLine();
                    scriptBuilder.AppendLine();
                }
                scriptBuilder.Append(GenerateSettingsScript(nonTechnicalSettings, false));
            }

            addEntityScript("[common].[Setting]", scriptBuilder.ToString());
        }

        private static string GenerateSettingsScript(List<Setting> settings, bool withUpdate)
        {
            StringBuilder scriptBuilder = new StringBuilder();
            foreach (var setting in settings)
            {
                if (scriptBuilder.Length > 0)
                {
                    scriptBuilder.Append(",");
                    scriptBuilder.AppendLine();
                }
                scriptBuilder.AppendFormat(@"('{0}','{1}','{2}','{3}','{4}','{5}',{6})", setting.SettingId, setting.Name, setting.Type, setting.Category, Serializer.Serialize(setting.Settings, true), Serializer.Serialize(setting.Data), setting.IsTechnical ? 1 : 0);
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
on		1=1 and t.[ID] = s.[ID]"
                + (withUpdate ?
@"
when matched then
	update set
	[Name] = s.[Name],[Type] = s.[Type],[Category] = s.[Category],[Settings] = s.[Settings],[Data] = s.[Data],[IsTechnical] = s.[IsTechnical]" : "") +
@"
when not matched by target then
	insert([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
	values(s.[ID],s.[Name],s.[Type],s.[Category],s.[Settings],s.[Data],s.[IsTechnical]);", scriptBuilder);
            return script;
        }

        #region Mappers
        public Setting SettingMapper(IDataReader reader)
        {
            var settings = reader["Settings"] as string;
            var data = reader["Data"] as string;
            return new Setting
            {
                SettingId = GetReaderValue<Guid>(reader, "ID"),
                Name = reader["Name"] as string,
                Type = reader["Type"] as string,
                Category = reader["Category"] as string,
                Settings = !string.IsNullOrEmpty(settings) ? Vanrise.Common.Serializer.Deserialize<SettingConfiguration>(settings) : null,
                Data = !string.IsNullOrEmpty(data) ? Vanrise.Common.Serializer.Deserialize<SettingData>(data) : null,
                IsTechnical = GetReaderValue<Boolean>(reader, "IsTechnical")
            };
        }
        #endregion
    }
}
