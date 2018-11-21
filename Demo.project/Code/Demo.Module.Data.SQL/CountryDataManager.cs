using Demo.Module.Entities;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;

namespace Demo.Module.Data
{
	class CountryDataManager : BaseSQLDataManager, ICountryDataManager
	{
		#region Constructors

		public CountryDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
		}

		#endregion

		#region Public Methods

		public List<Entities.Country> GetCountries()
		{
			return GetItemsSP("sp_Country_GetAll", CountryMapper);
		}

		public bool Insert(Country country, out int insertedId)
		{
			string serializedCountrySettings = country.Settings != null ? Vanrise.Common.Serializer.Serialize(country.Settings): null;

			object id;
			int nbOfRecordsAffected = ExecuteNonQuerySP("sp_Country_Insert", out id, country.Name, serializedCountrySettings);

			bool result = (nbOfRecordsAffected > 0);
			if (result)
				insertedId = (int)id;
			else
				insertedId = 0;
			return result;
		}

		public bool Update(Country country)
		{
			string serializedCountrySettings = country.Settings != null ? Vanrise.Common.Serializer.Serialize(country.Settings) : null;
			int nbOfRecordsAffected = ExecuteNonQuerySP("sp_Country_Update", country.CountryId, country.Name, serializedCountrySettings);
			return nbOfRecordsAffected > 0;
		}

		public bool AreCountryUpdated(ref object updateHandle)
		{
			return base.IsDataUpdated("[dbo].[Country]", ref updateHandle);
		}

		#endregion

		#region Mappers

		Country CountryMapper(IDataReader reader)
		{
			Country country = new Country()
			{
				CountryId = GetReaderValue<int>(reader, "ID"),
				Name = GetReaderValue<string>(reader, "Name"),
				Settings = Vanrise.Common.Serializer.Deserialize<CountrySettings>(reader["Settings"] as string)

			};

			return country;
		}

		#endregion
	}
}