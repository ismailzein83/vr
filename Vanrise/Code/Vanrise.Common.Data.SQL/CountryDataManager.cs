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
    public class CountryDataManager : BaseSQLDataManager, ICountrytDataManager
    {

        public CountryDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {

        }

        private Country CountryMapper(IDataReader reader)
        {
            Country country = new Country
            {
                CountryId = (int)reader["ID"],
                Name = reader["Name"] as string
                 
            };

            return country;
        }

        
        public List<Country> GetCountries()
        {
            return GetItemsSP("common.sp_Country_GetAll", CountryMapper);
        }
        public bool Update(Country country)
        {
            int recordsEffected = ExecuteNonQuerySP("common.sp_Country_Update", country.CountryId, country.Name);
            return (recordsEffected > 0);
        }

        public bool Insert(Country country, out int insertedId)
        {
            object countryId;

            int recordsEffected = ExecuteNonQuerySP("common.sp_Country_Insert", out countryId, country.Name);
            insertedId = (int)countryId;
            return (recordsEffected > 0);
        }

        public bool AreCountriesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("common.Country", ref updateHandle);
        }


    }
}
