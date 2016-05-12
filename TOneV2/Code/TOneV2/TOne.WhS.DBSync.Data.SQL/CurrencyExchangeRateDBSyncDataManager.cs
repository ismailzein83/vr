using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class CurrencyExchangeRateDBSyncDataManager : BaseSQLDataManager
    {
        readonly string[] columns = { "CurrencyID", "Rate", "ExchangeDate", "SourceID" };

        public CurrencyExchangeRateDBSyncDataManager(string tableName) :
            base(GetConnectionStringName("ConfigurationMigrationDBConnStringKey", "ConfigurationMigrationDBConnString"))
        {
        }

        public void ApplyCurrencyExchangeRatesToTemp(List<CurrencyExchangeRate> currencyExchangeRates)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var c in currencyExchangeRates)
                {
                    wr.WriteLine(String.Format("{0}^{1}^{2}^{3}", c.CurrencyId, c.Rate, c.ExchangeDate, c.SourceId));
                }
                wr.Close();
            }

            Object preparedCurrencyExchangeRates = new BulkInsertInfo
            {
                TableName = "[common].[CurrencyExchangeRate_Temp]",
                DataFilePath = filePath,
                ColumnNames = columns,
                TabLock = true,
                KeepIdentity = true,
                FieldSeparator = '^',
            };

            InsertBulkToTable(preparedCurrencyExchangeRates as BaseBulkInsertInfo);
        }



    }
}
