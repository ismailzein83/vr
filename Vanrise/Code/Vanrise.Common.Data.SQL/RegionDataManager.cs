using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Entities;

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

        public bool Update(Entities.Region Region)
        {
            int recordsEffected = ExecuteNonQuerySP("[common].[sp_Region_Update]", Region.RegionId, Region.Name, Region.CountryId, Vanrise.Common.Serializer.Serialize(Region.Settings));
            return (recordsEffected > 0);
        }

        public bool Insert(Entities.Region Region, out int insertedId)
        {
            object RegionId;
            int recordsEffected = ExecuteNonQuerySP("[common].[sp_Region_Insert]", out RegionId, Region.Name, Region.CountryId, Vanrise.Common.Serializer.Serialize(Region.Settings));
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

            return Region;
        }

        # endregion

    }
}
