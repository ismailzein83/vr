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
        public ZoneServiceConfigManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        { }

        public List<Entities.ZoneServiceConfig> GetZoneServiceConfigs()
        {
            return GetItemsSP("[TOneWhS_BE].[sp_ZoneServiceConfig_GetAll] ", ZoneServiceConfigMapper);
        }

        public bool Insert(Entities.ZoneServiceConfig zoneServiceFlag)
        {
            int recordsEffected = ExecuteNonQuerySP("[TOneWhS_BE].[sp_ZoneServiceConfig_Insert] ", zoneServiceFlag.ServiceFlag, zoneServiceFlag.Name, zoneServiceFlag.Settings);
            return (recordsEffected > 0);
        }

        public bool Update(Entities.ZoneServiceConfig zoneServiceFlag)
        {
            int recordsEffected = ExecuteNonQuerySP("[TOneWhS_BE].[sp_ZoneServiceConfig_Update] ", zoneServiceFlag.ServiceFlag, zoneServiceFlag.Name, zoneServiceFlag.Settings);
            return (recordsEffected > 0);
        }

        public bool AreZoneServiceConfigsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[TOneWhS_BE].[ZoneServiceConfig]", ref updateHandle);
        }


        #region  Mappers

        ZoneServiceConfig ZoneServiceConfigMapper(IDataReader reader)
        {
            ZoneServiceConfig zoneServiceConfig = new ZoneServiceConfig();

            zoneServiceConfig.ServiceFlag = (Int16)reader["ServiceFlag"];
            zoneServiceConfig.Name = reader["Name"] as string;
            zoneServiceConfig.Settings = reader["Settings"] as string;

            return zoneServiceConfig;
        }

        #endregion

    }
}
