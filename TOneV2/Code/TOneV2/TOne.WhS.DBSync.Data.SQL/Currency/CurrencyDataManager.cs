using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class CurrencyDataManager : BaseSQLDataManager, ICurrencyDataManager
    {
        readonly string[] columns = { "Name", "SourceID" };

        public CurrencyDataManager() :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneV2DBConnString"))
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
            table.Schema = "TOneWhS_BE";
            table.NamewithoutSchema = "WalidCurrency";
            table.CreateTableQuery =
                "CREATE TABLE " + table.TempName + "( " +
                "[ID] [int] IDENTITY(1,1) NOT NULL, " +
                "[Name] [varchar](50) NULL, " +
                "[timestamp] [timestamp] NULL, " +
                "[SourceID] [varchar](50) NULL) ";


            var foreignKeys = new List<TableKey>();
            foreignKeys.Add(new TableKey { KeyName = "FK_Trunk_Currency", TableName = "dbo.Trunk" });

            table.foreignKeys = foreignKeys;
            return table;
        }

    }
}
