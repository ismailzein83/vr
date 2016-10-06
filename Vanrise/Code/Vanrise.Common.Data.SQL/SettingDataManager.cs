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

        public bool UpdateSetting(Setting setting)
        {
            return ExecuteNonQuerySP("common.sp_Setting_Update", setting.SettingId, setting.Name, Vanrise.Common.Serializer.Serialize(setting.Data)) > 0;
        }

        public bool AreSettingsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("common.Setting", ref updateHandle);
        }


        #region Mappers
        public Setting SettingMapper(IDataReader reader)
        {
            var settings = reader["Settings"] as string;
            var data = reader["Data"] as string;
            return new Setting
            {
                SettingId = GetReaderValue<Guid>(reader,"ID"),
                Name = reader["Name"] as string,
                Type = reader["Type"] as string,
                Category = reader["Category"] as string,
                Settings = !string.IsNullOrEmpty(settings) ? Vanrise.Common.Serializer.Deserialize<SettingConfiguration>(settings) : null,
                Data = !string.IsNullOrEmpty(data) ? Vanrise.Common.Serializer.Deserialize(data) : null,
                IsTechnical = GetReaderValue<Boolean>(reader,"IsTechnical")
            };
        }
        #endregion
    }
}
