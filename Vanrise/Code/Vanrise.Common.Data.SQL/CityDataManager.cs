using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using System;

namespace Vanrise.Common.Data.SQL
{
    public class CityDataManager : BaseSQLDataManager, ICityDataManager
    {

        public CityDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {

        }

        public List<Entities.City> GetCities()
        {
            return GetItemsSP("[common].[sp_City_GetAll]", CityMapper);
        }

        public bool Update(Entities.City city)
        {
            int recordsEffected = ExecuteNonQuerySP("[common].[sp_City_Update]", city.CityId, city.Name, city.CountryId, Vanrise.Common.Serializer.Serialize(city.Settings), city.LastModifiedBy);
            return (recordsEffected > 0);
        }

        public bool Insert(Entities.City city, out int insertedId)
        {
            object cityId;
            int recordsEffected = ExecuteNonQuerySP("[common].[sp_City_Insert]", out cityId, city.Name, city.CountryId, Vanrise.Common.Serializer.Serialize(city.Settings), city.CreatedBy, city.LastModifiedBy);
            insertedId = (int)cityId;
            return (recordsEffected > 0);
        }

        public bool AreCitiesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[common].[City]", ref updateHandle);
        }


        #region Mappers

        City CityMapper(IDataReader reader)
        {
            var settings = reader["Settings"] as string;

            City city = new City();
            city.CityId = (int)reader["ID"];
            city.Name = reader["Name"] as string;
            city.CountryId = (int)reader["CountryID"];
            city.SourceId = reader["SourceID"] as string;
            city.Settings = !string.IsNullOrEmpty(settings) ? Vanrise.Common.Serializer.Deserialize<CitySettings>(settings) : null;
            city.CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime");
            city.CreatedBy = GetReaderValue<int?>(reader, "CreatedBy");
            city.LastModifiedBy = GetReaderValue<int?>(reader, "LastModifiedBy");
            city.LastModifiedTime = GetReaderValue<DateTime?>(reader, "LastModifiedTime");
            return city;
        }

        # endregion

    }
}
