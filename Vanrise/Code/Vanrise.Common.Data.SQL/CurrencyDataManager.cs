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
            : base(GetConnectionStringName("SecurityDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        public List<Currency> GetCurrencies()
        {
            return GetItemsSP("common.sp_Currency_GetAll", CurrencyMapper);
        }

        public bool AreCurrenciesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("common.Currency", ref updateHandle);
        }

        #region privat methode
        private Currency CurrencyMapper(IDataReader reader)
        {
            Currency country = new Currency
            {
                CurrencyId = (int)reader["ID"],
                Name = reader["Name"] as string,
                Symbol = reader["Symbol"] as string

            };

            return country;
        }
        #endregion
    }
}
