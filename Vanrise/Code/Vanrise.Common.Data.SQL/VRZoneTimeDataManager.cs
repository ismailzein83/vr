using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Data.SQL;

namespace Vanrise.Common.Data.SQL
{
    public class VRTimeZoneDataManager : BaseSQLDataManager, IVRTimeZoneDataManager
    {

        public VRTimeZoneDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        private VRTimeZone VRTimeZoneMapper(IDataReader reader)
        {
            VRTimeZone timeZone = new VRTimeZone
            {
                TimeZoneId = (int)reader["ID"],
                Name = reader["Name"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<VRTimeZoneSettings>(reader["Settings"] as string) ,
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
                CreatedBy = GetReaderValue<int?>(reader, "CreatedBy"),
                LastModifiedBy = GetReaderValue<int?>(reader, "LastModifiedBy"),
                LastModifiedTime = GetReaderValue<DateTime?>(reader, "LastModifiedTime")
            };        
            return timeZone;
        }

        
        public List<VRTimeZone> GetVRTimeZones()
        {
            return GetItemsSP("common.sp_VRTimeZone_GetAll", VRTimeZoneMapper);
        }
      
        
        public bool Update(VRTimeZone timeZone)
        {
            int recordsEffected = ExecuteNonQuerySP("common.sp_VRTimeZone_Update",
                timeZone.TimeZoneId,
                timeZone.Name,
                Vanrise.Common.Serializer.Serialize(timeZone.Settings),
                timeZone.LastModifiedBy);
            return (recordsEffected > 0);
        }

        public bool Insert(VRTimeZone timeZone, out int insertedId)
        {

            object timeZoneId;
            int recordsEffected = ExecuteNonQuerySP("common.sp_VRTimeZone_Insert",
                out timeZoneId,
                timeZone.Name,
                Vanrise.Common.Serializer.Serialize(timeZone.Settings),
                timeZone.CreatedBy,
                timeZone.LastModifiedBy
                );
            insertedId = (int)timeZoneId;
            return (recordsEffected > 0);
        }
    
        public bool AreVRTimeZonesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("common.VRTimeZone", ref updateHandle);
        }


    }
}
