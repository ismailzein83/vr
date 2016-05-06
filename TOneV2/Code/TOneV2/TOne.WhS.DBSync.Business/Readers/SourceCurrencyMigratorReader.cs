using System.Collections.Generic;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities.EntityMigrator;

namespace TOne.WhS.DBSync.Business.SourceMigratorsReaders
{
    public class SourceCurrencyMigratorReader : IMigrationSourceItemReader<SourceCurrency> 
    {
        public string ConnectionString { get; set; }

        public IEnumerable<SourceCurrency> GetSourceItems()
        {
            DataManager dataManager = new DataManager(this.ConnectionString);
            return dataManager.GetSourceCurrencies();
        }

        private class DataManager : Vanrise.Data.SQL.BaseSQLDataManager
        {
            public DataManager(string connectionString)
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

            const string query_getSourceCurrencies = @"SELECT [CurrencyID]  ,[Name]   ,[IsMainCurrency]  ,[IsVisible]  ,[LastRate]  ,[LastUpdated]  ,[UserID]  ,[timestamp]   ,[DS_ID_auto]  FROM [dbo].[Currency]";
        }
    }
}
