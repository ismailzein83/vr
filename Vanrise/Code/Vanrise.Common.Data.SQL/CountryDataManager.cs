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
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        private Country CountryMapper(IDataReader reader)
        {
            Country country = new Country
            {
                CountryId = (int)reader["ID"],
                Name = reader["Name"] as string,
                SourceId = reader["SourceId"] as string,
                CreatedTime =  GetReaderValue<DateTime?>(reader, "CreatedTime"),
                CreatedBy = GetReaderValue<int?>(reader, "CreatedBy"),
                LastModifiedBy = GetReaderValue<int?>(reader, "LastModifiedBy"),
                LastModifiedTime = GetReaderValue<DateTime?>(reader, "LastModifiedTime")
                 
            };

            return country;
        }

        
        public List<Country> GetCountries()
        {
            return GetItemsSP("common.sp_Country_GetAll", CountryMapper);
        }
        public bool Update(Country country)
        {
            int recordsEffected = ExecuteNonQuerySP("common.sp_Country_Update", country.CountryId, country.Name, country.LastModifiedBy);
            return (recordsEffected > 0);
        }

        public bool Insert(Country country)
        {
            

            int recordsEffected = ExecuteNonQuerySP("common.sp_Country_Insert", country.CountryId, country.Name, country.CreatedBy, country.LastModifiedBy);
            
            return (recordsEffected > 0);
        }
        public void InsertFromSource(Country country)
        {
            ExecuteNonQuerySP("common.sp_Country_InsertFromSource", country.CountryId, country.Name, country.SourceId, country.CreatedBy, country.LastModifiedBy);
        }

        public void UpdateFromSource(Country country)
        {

            ExecuteNonQuerySP("common.sp_Country_UpdateFromSource", country.CountryId, country.Name, country.LastModifiedBy);
        }
        public bool AreCountriesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("common.Country", ref updateHandle);
        }


    }
}
