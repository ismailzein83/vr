using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using System;

namespace Vanrise.Common.Data.SQL
{
    public class RegionDataManager : BaseSQLDataManager, IRegionDataManager
    {

        public RegionDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {

        }

        public List<Entities.Region> GetRegions()
        {
            return GetItemsSP("[common].[sp_Region_GetAll]", RegionMapper);
        }

        public bool Update(Region region)
        {
            int recordsEffected = ExecuteNonQuerySP("[common].[sp_Region_Update]", region.RegionId, region.Name, region.CountryId, Vanrise.Common.Serializer.Serialize(region.Settings), region.LastModifiedBy);
            return (recordsEffected > 0);
        }

        public bool Insert(Region region, out int insertedId)
        {
            object RegionId;
            int recordsEffected = ExecuteNonQuerySP("[common].[sp_Region_Insert]", out RegionId, region.Name, region.CountryId, Vanrise.Common.Serializer.Serialize(region.Settings), region.CreatedBy, region.LastModifiedBy);
            insertedId = (int)RegionId;
            return (recordsEffected > 0);
        }

        public bool AreRegionsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[common].[Region]", ref updateHandle);
        }


        #region Mappers

        Region RegionMapper(IDataReader reader)
        {
           var settings = reader["Settings"] as string;

            Region Region = new Region();
            Region.RegionId = (int)reader["ID"];
            Region.Name = reader["Name"] as string;
            Region.CountryId = (int)reader["CountryID"];
            Region.Settings = !string.IsNullOrEmpty(settings) ? Vanrise.Common.Serializer.Deserialize<RegionSettings>(settings) : null;
            Region.CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime");
            Region.CreatedBy = GetReaderValue<int?>(reader, "CreatedBy");
            Region.LastModifiedBy = GetReaderValue<int?>(reader, "LastModifiedBy");
            Region.LastModifiedTime = GetReaderValue<DateTime?>(reader, "LastModifiedTime");

            return Region;
        }

        # endregion

    }
}
