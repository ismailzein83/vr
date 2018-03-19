using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class CurrencyDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ICurrencyDataManager
    {
        public CurrencyDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        public List<Currency> GetCurrencies()
        {
            return GetItemsSP("common.sp_Currency_GetAll", CurrencyMapper);
        }
        public bool Update(Currency currency)
        {
            int recordsEffected = ExecuteNonQuerySP("common.sp_Currency_Update", currency.CurrencyId, currency.Name, currency.Symbol, currency.LastModifiedBy);
            return (recordsEffected > 0);
        }

        public bool Insert(Currency currency, out int insertedId)
        {
            object currencyId;

            int recordsEffected = ExecuteNonQuerySP("common.sp_Currency_Insert", out currencyId, currency.Name, currency.Symbol, currency.SourceId, currency.CreatedBy, currency.LastModifiedBy);
            insertedId = (int)currencyId;
            return (recordsEffected > 0);
        }
        public bool AreCurrenciesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("common.Currency", ref updateHandle);
        }

        #region Mappers
        public Currency CurrencyMapper(IDataReader reader)
        {
            Currency currency = new Currency
            {
                CurrencyId = (int)reader["ID"],
                Name = reader["Name"] as string,
                Symbol = reader["Symbol"] as string,
                SourceId = reader["SourceID"] as string,
                CreatedTime = GetReaderValue<DateTime?>(reader, "CreatedTime"),
                CreatedBy = GetReaderValue<int?>(reader, "CreatedBy"),
                LastModifiedBy = GetReaderValue<int?>(reader, "LastModifiedBy"),
                LastModifiedTime = GetReaderValue<DateTime?>(reader, "LastModifiedTime")
            };

            return currency;
        }
        #endregion
    }
}
