using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using System.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class ZoneServiceConfigManager: BaseSQLDataManager, IZoneServiceConfigDataManager
    {
       
        #region ctor/Local Variables
        public ZoneServiceConfigManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        { }

        #endregion

        #region Public Methods
        public List<Entities.ZoneServiceConfig> GetZoneServiceConfigs()
        {
            return GetItemsSP("[TOneWhS_BE].[sp_ZoneServiceConfig_GetAll]", ZoneServiceConfigMapper);
        }
        public bool Insert(Entities.ZoneServiceConfig zoneServiceFlag ,  out int insertedId)
        {
            object zoneServiceConfigId;

            int recordsEffected = ExecuteNonQuerySP("[TOneWhS_BE].[sp_ZoneServiceConfig_Insert]",
                out zoneServiceConfigId,
                zoneServiceFlag.Symbol, 
                Vanrise.Common.Serializer.Serialize(zoneServiceFlag.Settings)
             );
            insertedId = (int)zoneServiceConfigId;
            return (recordsEffected > 0);
        }
        public bool Update(Entities.ZoneServiceConfig zoneServiceFlag)
        {
            int recordsEffected = ExecuteNonQuerySP("[TOneWhS_BE].[sp_ZoneServiceConfig_Update]", zoneServiceFlag.ZoneServiceConfigId, zoneServiceFlag.Symbol,   Vanrise.Common.Serializer.Serialize(zoneServiceFlag.Settings));
            return (recordsEffected > 0);
        }
        public bool AreZoneServiceConfigsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[TOneWhS_BE].[ZoneServiceConfig]", ref updateHandle);
        }
        #endregion

        #region Private Methods
        #endregion
       
        #region  Mappers
        ZoneServiceConfig ZoneServiceConfigMapper(IDataReader reader)
        {
            ZoneServiceConfig zoneServiceConfig = new ZoneServiceConfig();

            zoneServiceConfig.ZoneServiceConfigId = (int)reader["ID"];
            zoneServiceConfig.Symbol = reader["Symbol"] as string;
            zoneServiceConfig.Settings = Vanrise.Common.Serializer.Deserialize<ServiceConfigSetting>((string)reader["Settings"]);

            return zoneServiceConfig;
        }

        #endregion

    }
}
