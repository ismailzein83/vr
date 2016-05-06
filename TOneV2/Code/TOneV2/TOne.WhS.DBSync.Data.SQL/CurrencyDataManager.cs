using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class CurrencyDataManager : BaseSQLDataManager, ICurrencyDataManager
    {
        readonly string[] columns = { "Symbol", "Name", "SourceID" };
        Table _table;
        TableManager tableManager;

        public CurrencyDataManager() :
            base(GetConnectionStringName("TOneWhS_BE_MigrationDBConnStringKey", "TOneV2MigrationDBConnString"))
        {
            _table = DefineTable();
            tableManager = new TableManager(_table);
        }

        public void MigrateCurrenciesToDB(List<Currency> currencies)
        {
            tableManager.DropandCreateTempTable(); // Drop and Create Temp
            ApplyCurrenciesToDB(currencies); // Apply to Temp
            tableManager.DropTable();
            tableManager.RenameTablefromTempandRestorePK(); // Rename Temp table to be Table name
        }


        private void ApplyCurrenciesToDB(List<Currency> currencies)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var c in currencies)
                {
                    wr.WriteLine(String.Format("{0}^{1}^{2}", c.Symbol, c.Name, c.CurrencyId));
                }
                wr.Close();
            }

            Object preparedCurrencies = new BulkInsertInfo
            {
                TableName = _table.TempName,
                DataFilePath = filePath,
                ColumnNames = columns,
                TabLock = true,
                KeepIdentity = true,
                FieldSeparator = '^',
            };

            InsertBulkToTable(preparedCurrencies as BaseBulkInsertInfo);
        }

        private static Table DefineTable()
        {
            Table table = new Table();
            table.Schema = "Common";
            table.NamewithoutSchema = "Currency";
            table.CreateTableQuery =
                "CREATE TABLE " + table.TempName + "( " +
                "[ID] [int] IDENTITY(1,1) NOT NULL, " +
                "[Symbol] [nvarchar](10) NOT NULL, " +
                "[Name] [nvarchar](255) NOT NULL, " +
                "[timestamp] [timestamp] NULL, " +
                "[SourceID] [varchar](50) NULL) ";

            var relatedForeignKeys = new List<FKey>(); // no keys are foreign keys of other tables
            relatedForeignKeys.Add(new FKey { KeyName = "FK_CurrencyExchangeRate_Currency", TableName = "common.CurrencyExchangeRate" });
            table.relatedForeignKeys = relatedForeignKeys;


            var primaryKey = new PKey { Fields = new List<string> { "ID" }, KeyName = "PK_Currency" };
            table.primaryKey = primaryKey;


            return table;
        }

    }
}
