﻿using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Entities;

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
            int recordsEffected = ExecuteNonQuerySP("[common].[sp_City_Update]", city.CityId, city.Name, city.CountryId, Vanrise.Common.Serializer.Serialize(city.Settings));
            return (recordsEffected > 0);
        }

        public bool Insert(Entities.City city, out int insertedId)
        {
            object cityId;
            int recordsEffected = ExecuteNonQuerySP("[common].[sp_City_Insert]", out cityId, city.Name, city.CountryId, Vanrise.Common.Serializer.Serialize(city.Settings));
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
            city.Settings = !string.IsNullOrEmpty(settings) ? Vanrise.Common.Serializer.Deserialize<CitySettings>(settings) : null;
            return city;
        }

        # endregion

    }
}
