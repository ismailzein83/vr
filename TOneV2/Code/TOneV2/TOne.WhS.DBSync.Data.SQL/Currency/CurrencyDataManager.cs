using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class CurrencyDataManager : BaseSQLDataManager, ICurrencyDataManager
    {
        readonly string[] columns = { "Symbol", "Name","SourceID" };

        public CurrencyDataManager() :
            base(GetConnectionStringName("TOneWhS_BE_MigrationDBConnStringKey", "TOneV2MigrationDBConnString"))
        {

        }

        public void MigrateCurrenciesToDB(List<Currency> currencies)
        {
            Table table = DefineTable();

            TableManager tableManager = new TableManager();

            tableManager.DropandCreateTempTable(table); // Drop and Create Temp

            ApplyCurrenciesToDB(currencies, table); // Apply to Temp

            tableManager.DropTable(table);

            tableManager.RenameTablefromTemp(table); // Rename Temp table to be Table name
        }


        private void ApplyCurrenciesToDB(List<Currency> currencies, Table table)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var c in currencies)
                {
                    wr.WriteLine(String.Format("{0}^{1}", c.Name, c.CurrencyId));
                }
                wr.Close();
            }

            Object preparedCurrencies = new BulkInsertInfo
            {
                TableName = table.TempName,
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
                "CREATE TABLE [common].[Currency]( " +
                "[ID] [int] IDENTITY(1,1) NOT NULL, " +
                "[Symbol] [nvarchar](10) NOT NULL, " +
                "[Name] [nvarchar](255) NOT NULL, " +
                "[timestamp] [timestamp] NULL " +
                "[SourceID] [varchar](50) NULL ";

            var foreignKeys = new List<TableKey>(); // no keys are foreign keys of other tables
            table.foreignKeys = foreignKeys;
            return table;
        }

    }
}
