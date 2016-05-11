using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class CurrencyExchangeRateDataManager : BaseSQLDataManager
    {
        readonly string[] columns = { "CurrencyID", "Rate", "ExchangeDate", "SourceID" };
        string _tempTableName;

        public CurrencyExchangeRateDataManager(string tableName) :
            base(GetConnectionStringName("ConfigurationMigrationDBConnStringKey", "ConfigurationMigrationDBConnString"))
        {
            _tempTableName = tableName;
        }

        public void ApplyCurrencyExchangeRatesToDB(List<CurrencyExchangeRate> currencyExchangeRates)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var c in currencyExchangeRates)
                {
                    wr.WriteLine(String.Format("{0}^{1}^{2}^{3}", c.CurrencyId, c.Rate, c.ExchangeDate, c.SourceID));
                }
                wr.Close();
            }

            Object preparedCurrencyExchangeRates = new BulkInsertInfo
            {
                TableName = _tempTableName,
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
