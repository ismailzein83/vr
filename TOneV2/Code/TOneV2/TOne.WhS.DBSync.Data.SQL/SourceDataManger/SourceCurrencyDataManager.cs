using System.Collections.Generic;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SourceCurrencyDataManager : BaseSQLDataManager
    {
        public SourceCurrencyDataManager(string connectionString)
            : base(connectionString, false)
        {
        }

        public List<SourceCurrency> GetSourceCurrencies()
        {
            return GetItemsText(query_getSourceCurrencies, SourceCurrencyMapper, null);
        }

        private SourceCurrency SourceCurrencyMapper(System.Data.IDataReader arg)
        {
            SourceCurrency sourceCurrency = new SourceCurrency()
            {
                SourceId = arg["CurrencyID"].ToString(),
                Name = arg["Name"].ToString(),
                Symbol = arg["CurrencyID"].ToString()
            };
            return sourceCurrency;
        }

        const string query_getSourceCurrencies = @"SELECT [CurrencyID]  ,[Name]  FROM [dbo].[Currency] WITH (NOLOCK)";
    }
}
