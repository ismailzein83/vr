using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
namespace Demo.Module.Data.SQL
{
    public class DemoCountryDataManager : BaseSQLDataManager, IDemoCountryDataManager
    {
        public DemoCountryDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }

        public List<DemoCountry> GetDemoCountries()
        {
            return GetItemsSP("[dbo].[sp_Countries_GetAll]", DemoCountryMapper);
        }

        public bool Insert(DemoCountry demoCountry, out int insertedId)
        {
            object id;
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Country_Insert]", out id, demoCountry.Name);
            insertedId = Convert.ToInt32(id);

            return (nbOfRecordsAffected > 0);
        }

        public bool Update(DemoCountry demoCountry)
        {
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Country_Update]", demoCountry.DemoCountryId, demoCountry.Name);
            return (nbOfRecordsAffected > 0);
        }

        public bool Delete(int demoCountryId)
        {
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Country_Delete]", demoCountryId);
            return (nbOfRecordsAffected > 0);
        }

        public bool AreDemoCountriesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[Country]", ref updateHandle);
        }

        DemoCountry DemoCountryMapper(IDataReader reader)
        {
            return new DemoCountry
            {
                DemoCountryId = GetReaderValue<int>(reader, "ID"),
                Name = GetReaderValue<string>(reader, "Name")
            };
        }
    }
}
