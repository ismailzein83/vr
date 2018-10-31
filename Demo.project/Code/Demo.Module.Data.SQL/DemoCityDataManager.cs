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
    public class DemoCityDataManager : BaseSQLDataManager, IDemoCityDataManager
    {
        public DemoCityDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }

        public List<DemoCity> GetDemoCities()
        {
            return GetItemsSP("[dbo].[sp_City_GetAll]", DemoCityMapper);
        }

        public bool Insert(DemoCity demoCity, out int insertedId)
        {
            object id;
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_City_Insert]", out id, demoCity.Name);
            insertedId = Convert.ToInt32(id);

            return (nbOfRecordsAffected > 0);
        }

        public bool Update(DemoCity demoCity)
        {
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_City_Update]", demoCity.DemoCityId, demoCity.Name);
            return (nbOfRecordsAffected > 0);
        }

        public bool Delete(int demoCityId)
        {
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_City_Delete]", demoCityId);
            return (nbOfRecordsAffected > 0);
        }

        public bool AreDemoCitiesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[City]", ref updateHandle);
        }

        DemoCity DemoCityMapper(IDataReader reader)
        {
            return new DemoCity
            {
                DemoCityId = GetReaderValue<int>(reader, "ID"),
                Name = GetReaderValue<string>(reader, "Name"),
                DemoCountryId=GetReaderValue<int>(reader, "CountryID")
            };
        }
    }
}
