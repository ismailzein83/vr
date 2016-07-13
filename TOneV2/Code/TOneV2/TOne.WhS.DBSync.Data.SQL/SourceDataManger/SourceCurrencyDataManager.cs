using System.Collections.Generic;
using System.Data;
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

        private SourceCurrency SourceCurrencyMapper(IDataReader arg)
        {
            SourceCurrency sourceCurrency = new SourceCurrency()
            {
                SourceId = arg["CurrencyID"].ToString(),
                Name = arg["Name"] as string,
                IsMainCurrency = arg["IsMainCurrency"].ToString().Equals("Y", System.StringComparison.InvariantCultureIgnoreCase) ? true : false,
                Symbol = arg["CurrencyID"].ToString(),
            };
            return sourceCurrency;
        }

        const string query_getSourceCurrencies = @"SELECT [CurrencyID] ,[IsMainCurrency] ,[Name]  FROM [dbo].[Currency] WITH (NOLOCK)";
    }
}
