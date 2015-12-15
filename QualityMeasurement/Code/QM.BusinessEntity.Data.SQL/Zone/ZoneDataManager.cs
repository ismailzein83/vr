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
        public ZoneDataManager()
            : base("MainDBConnString")
        {
        }

        public Zone ZoneMapper(IDataReader reader)
        {
            Zone Zone = new Zone()
            {
                ZoneId = (long)reader["ID"],
                Name = reader["Name"] as string,
                SourceId = reader["SourceZoneID"] as string,
                CountryId = (int)reader["CountryID"],
                BeginEffectiveDate = (DateTime)reader["BED"],
                EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EED")
            };
            return Zone;
        }

        public List<Zone> GetZones()
        {
            return GetItemsSP("[QM_BE].[sp_Zone_GetAll]", ZoneMapper);
        }

        public void InsertZoneFromSource(Zone zone)
        {
            ExecuteNonQuerySP("[QM_BE].[sp_Zone_InsertFromSource]", zone.ZoneId, zone.Name, zone.SourceId , zone.CountryId , zone.BeginEffectiveDate);
        }

        public void UpdateZoneFromSource(Zone zone)
        {

            ExecuteNonQuerySP("[QM_BE].[sp_Zone_UpdateFromSource]", zone.ZoneId, zone.Name);
        }
        public bool AreZonesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("QM_BE.Zone", ref updateHandle);
        }

        public List<Zone> GetZonesByCountry(int CountryId)
        {
            throw new NotImplementedException();
        }
    }
}
