using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;

namespace TOne.BusinessEntity.Data.SQL
{
    public class CurrencyDataManager : BaseTOneDataManager, ICurrencyDataManager 
    {
        public List<Currency> GetCurrencies()
        {
            return GetItemsSP("[BEntity].[sp_Currency_GetAll]", CurrencyMapper);
        }
        private Currency CurrencyMapper(IDataReader reader)
        {
            Entities.Currency module = new Entities.Currency
            {
                CurrencyID = reader["CurrencyID"] as string,
                Name = reader["Name"] as string,
                IsMainCurrency = reader["IsMainCurrency"] as string,
                IsVisible = reader["IsVisible"] as string,
                //IsMainCurrency = GetReaderValue<char>(reader, "IsMainCurrency"),
                //IsVisible = GetReaderValue<char>(reader, "IsVisible"),
                LastRate = GetReaderValue<double>(reader, "LastRate"),
                LastUpdated = GetReaderValue<DateTime>(reader, "LastUpdated"),
                UserID = GetReaderValue<int>(reader, "UserID"),
                CurrencyFullName = (reader["CurrencyID"] + " - " + reader["Name"]) as string
            };
            return module;
        }
    }
}
