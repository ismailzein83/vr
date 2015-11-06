using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class CurrencyExchangeRateDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ICurrencyExchangeRateDataManager
    {
        public CurrencyExchangeRateDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        public List<CurrencyExchangeRate> GetCurrenciesExchangeRate()
        {
            return GetItemsSP("common.sp_CurrencyExchangeRate_GetAll", CurrencyExchangeRateMapper);
        }



        public bool Insert(CurrencyExchangeRate currencyExchangeRate, out int insertedId)
        {
            object currencyExchangeRateId;

            int recordsEffected = ExecuteNonQuerySP("common.sp_CurrencyExchangeRate_Insert", out currencyExchangeRateId, currencyExchangeRate.Rate, currencyExchangeRate.CurrencyId, currencyExchangeRate.ExchangeDate);
            insertedId = (int)currencyExchangeRateId;
            return (recordsEffected > 0);
        }
        public bool Update(CurrencyExchangeRate currencyExchangeRate)
        {
            int recordsEffected = ExecuteNonQuerySP("common.sp_CurrencyExchangeRate_Update", currencyExchangeRate.CurrencyExchangeRateId, currencyExchangeRate.Rate, currencyExchangeRate.CurrencyId ,currencyExchangeRate.ExchangeDate);
            return (recordsEffected > 0);
        }
        public bool AreCurrenciesExchangeRateUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("common.CurrencyExchangeRate", ref updateHandle);
        }

        #region privat methode
        private CurrencyExchangeRate CurrencyExchangeRateMapper(IDataReader reader)
        {
            CurrencyExchangeRate currencyExchangeRate = new CurrencyExchangeRate
            {
                CurrencyExchangeRateId = (long)reader["ID"],
                CurrencyId = (int)reader["CurrencyId"],
                Rate = GetReaderValue<decimal>(reader,"Rate"),
                ExchangeDate = GetReaderValue<DateTime>(reader, "ExchangeDate"),

            };

            return currencyExchangeRate;
        }
        #endregion
    }
}
