using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace QM.BusinessEntity.Data.SQL
{
    public class ZoneDataManager : BaseSQLDataManager, IZoneDataManager
    {
         public ZoneDataManager() :
            base(GetConnectionStringName("QM_BE_DBConnStringKey", "QM_BE_DBConnString"))
        {

        }


         public Zone GetZoneBySourceId(string sourceZoneId)
         {
             return GetItemSP("[QM_BE].sp_Zone_GetBySourceID", ZoneMapper, sourceZoneId);
         }
       


        public Zone ZoneMapper(IDataReader reader)
        {
            Zone Zone = new Zone()
            {
                ZoneId = (long)reader["ID"],
                Name = reader["Name"] as string,
                SourceId = reader["SourceZoneID"] as string,
                CountryId = (int)reader["CountryID"],
                BeginEffectiveDate = (reader["BED"] as string) == null ? default(DateTime) : (DateTime)reader["BED"],
                EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EED"),
                Settings = Vanrise.Common.Serializer.Deserialize<ZoneSettings>(reader["Settings"] as string),
                IsFromTestingConnectorZone = GetReaderValue<bool>(reader, "IsFromTestingConnectorZone")
            };
            return Zone;
        }

        public List<Zone> GetZones()
        {
                return GetItemsSP("[QM_BE].[sp_Zone_GetAll]", ZoneMapper);
        }

        public void InsertZoneFromSource(Zone zone)
        {
            object settings = null;
            if (zone.Settings != null)
                settings = Vanrise.Common.Serializer.Serialize(zone.Settings);
            
            ExecuteNonQuerySP("[QM_BE].[sp_Zone_InsertFromSource]", zone.ZoneId, zone.Name, zone.SourceId, zone.CountryId, ToDBNullIfDefault(zone.BeginEffectiveDate), settings, zone.IsFromTestingConnectorZone);
        }

        public void UpdateZoneFromSource(Zone zone)
        {
            object settings = null;
            if (zone.Settings != null)
                settings = Vanrise.Common.Serializer.Serialize(zone.Settings);

            ExecuteNonQuerySP("[QM_BE].[sp_Zone_UpdateFromSource]", zone.ZoneId, zone.Name, zone.SourceId, zone.CountryId, ToDBNullIfDefault(zone.BeginEffectiveDate), settings, zone.IsFromTestingConnectorZone);
        }

        public bool AreZonesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("QM_BE.Zone", ref updateHandle);
        }
       
    }
}
