using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class CurrencyExchangeRateDataManager : BaseSQLDataManager, ICurrencyExchangeRateDataManager
    {
        readonly string[] columns = { "CurrencyID", "Rate", "ExchangeDate", "SourceID" };
        Table _table;
        TableManager tableManager;

        public CurrencyExchangeRateDataManager() :
            base(GetConnectionStringName("TOneWhS_BE_MigrationDBConnStringKey", "TOneV2MigrationDBConnString"))
        {
            _table = DefineTable();
            tableManager = new TableManager(_table);
        }

        public void MigrateCurrencyExchangeRatesToDB(List<CurrencyExchangeRate> currencyExchangeRates)
        {
            tableManager.DropandCreateTempTable(); // Drop and Create Temp
            ApplyCurrencyExchangeRatesToDB(currencyExchangeRates); // Apply to Temp
            tableManager.DropTable();
            tableManager.RenameTablefromTempandRestorePK(); // Rename Temp table to be Table name
        }


        private void ApplyCurrencyExchangeRatesToDB(List<CurrencyExchangeRate> currencyExchangeRates)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var c in currencyExchangeRates)
                {
                    wr.WriteLine(String.Format("{0}^{1}^{2}^{3}", c.CurrencyId, c.Rate, c.ExchangeDate, c.CurrencyExchangeRateId));
                }
                wr.Close();
            }

            Object preparedCurrencyExchangeRates = new BulkInsertInfo
            {
                TableName = _table.TempName,
                DataFilePath = filePath,
                ColumnNames = columns,
                TabLock = true,
                KeepIdentity = true,
                FieldSeparator = '^',
            };

            InsertBulkToTable(preparedCurrencyExchangeRates as BaseBulkInsertInfo);
        }

        private static Table DefineTable()
        {
            Table table = new Table();
            table.Schema = "Common";
            table.NamewithoutSchema = "CurrencyExchangeRate";
            table.CreateTableQuery =
                "CREATE TABLE " + table.TempName + "( " +
                "[ID] [bigint] IDENTITY(1,1) NOT NULL, " +
                "[CurrencyID] [int] NOT NULL, " +
                "[Rate] [decimal](18, 5) NOT NULL, " +
                "[ExchangeDate] [datetime] NOT NULL, " +
                "[timestamp] [timestamp] NULL, " +
                "[SourceID] [varchar](50) NULL) ";

            var primaryKey = new PKey { Fields = new List<string> { "ID" }, KeyName = "PK_CurrencyExchangeRate" };
            table.primaryKey = primaryKey;

            return table;
        }

    }
}
