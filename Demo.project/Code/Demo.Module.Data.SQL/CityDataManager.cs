using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Demo.Module.Entities;
using Vanrise.Data.SQL;

namespace Demo.Module.Data
{
	public class CityDataManager : BaseSQLDataManager, ICityDataManager
	{
		#region Constructors

		public CityDataManager() :
			base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
		{
		}

		#endregion

		#region Public Methods

		public List<Entities.City> GetCities()
		{
			return GetItemsSP("sp_City_GetAll", CityMapper);
		}

		public bool Insert(City city, out int insertedId)
		{
			object id;

			string serializedCitySettings = city.Settings != null ? Vanrise.Common.Serializer.Serialize(city.Settings) : null;

			int nbOfRecordsAffecterd = ExecuteNonQuerySP("sp_City_Insert", out id, city.Name, city.CountryId, serializedCitySettings);
			bool result = nbOfRecordsAffecterd > 0;
			if (result)
				insertedId = (int)id;
			else
				insertedId = 0;
			return result;
		}

		public bool Update(City city)
		{
			string serializedCitySettings = city.Settings != null ? Vanrise.Common.Serializer.Serialize(city.Settings) : null;
			int nbOfRecordsAffected = ExecuteNonQuerySP("sp_City_Update", city.CityId, city.Name, city.CountryId, serializedCitySettings);
			return nbOfRecordsAffected > 0;
		}

		public bool AreCityUpdated(ref object updateHandle)
		{
			return base.IsDataUpdated("[dbo].[City]", ref updateHandle);
		}

		#endregion

		#region Mappers

		City CityMapper(IDataReader reader)
		{
			var city = new City()
			{
				CityId = GetReaderValue<int>(reader, "ID"),
				Name = GetReaderValue<string>(reader, "Name"),
				CountryId = GetReaderValue<int>(reader, "CountryID"),
				Settings = Vanrise.Common.Serializer.Deserialize<CitySettings>(reader["Settings"] as string)
			};
			return city;
		}

		#endregion
	}
}