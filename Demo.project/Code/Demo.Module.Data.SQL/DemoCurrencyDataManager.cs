using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
namespace Demo.Module.Data.SQLDemoCurrency
{
    public class DemoCurrencyDataManager : BaseSQLDataManager, IDemoCurrencyDataManager
    {
        public DemoCurrencyDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }

        public List<DemoCurrency> GetDemoCurrencies()
        {
            return GetItemsSP("[dbo].[sp_DemoCurrency_GetAll]", DemoCurrencyMapper);
        }

        public bool Insert(DemoCurrency demoCurrency, out int insertedId)
        {
            object id;
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_DemoCurrency_Insert]", out id, demoCurrency.Name);
            insertedId = Convert.ToInt32(id);

            return (nbOfRecordsAffected > 0);
        }

        public bool Update(DemoCurrency demoCurrency)
        {
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_DemoCurrency_Update]", demoCurrency.DemoCurrencyId, demoCurrency.Name);
            return (nbOfRecordsAffected > 0);
        }

        public bool Delete(int demoCurrencyId)
        {
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_DemoCurrency_Delete]", demoCurrencyId);
            return (nbOfRecordsAffected > 0);
        }

        public bool AreDemoCurrenciesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[DemoCurrency]", ref updateHandle);
        }

        DemoCurrency DemoCurrencyMapper(IDataReader reader)
        {
            return new DemoCurrency
            {
                DemoCurrencyId = GetReaderValue<int>(reader, "ID"),
                Name = GetReaderValue<string>(reader, "Name")
            };
        }
    }
}
