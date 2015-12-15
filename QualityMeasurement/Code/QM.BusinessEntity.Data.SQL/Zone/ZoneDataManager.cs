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
                ZoneId = (int)reader["ID"],
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

        public bool AreZonesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("QM_BE.Zone", ref updateHandle);
        }
    }
}
