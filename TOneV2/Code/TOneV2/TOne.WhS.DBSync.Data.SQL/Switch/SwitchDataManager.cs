using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SwitchDataManager : BaseSQLDataManager, ISwitchDataManager
    {
        readonly string[] columns = { "Name", "SourceID" };
        Table _table;
        TableManager tableManager;

        public SwitchDataManager() :
            base(GetConnectionStringName("TOneWhS_BE_MigrationDBConnStringKey", "TOneV2MigrationDBConnString"))
        {
            _table = DefineTable();
            tableManager = new TableManager(_table);
        }

        public void MigrateSwitchesToDB(List<Switch> switches)
        {
            tableManager.DropandCreateTempTable(); // Drop and Create Temp
            ApplySwitchesToDB(switches); // Apply to Temp
            tableManager.DropTable();
            tableManager.RenameTablefromTempandRestorePK(); // Rename Temp table to be Table name
        }


        private void ApplySwitchesToDB(List<Switch> switches)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var s in switches)
                {
                    wr.WriteLine(String.Format("{0}^{1}", s.Name, s.SwitchId));
                }
                wr.Close();
            }

            Object preparedSwitches = new BulkInsertInfo
            {
                TableName = _table.TempName,
                DataFilePath = filePath,
                ColumnNames = columns,
                TabLock = true,
                KeepIdentity = true,
                FieldSeparator = '^',
            };

            InsertBulkToTable(preparedSwitches as BaseBulkInsertInfo);
        }

        private static Table DefineTable()
        {
            Table table = new Table();
            table.Schema = "TOneWhS_BE";
            table.NamewithoutSchema = "Switch";
            table.CreateTableQuery =
                "CREATE TABLE " + table.TempName + "( " +
                "[ID] [int] IDENTITY(1,1) NOT NULL, " +
                "[Name] [varchar](50) NULL, " +
                "[timestamp] [timestamp] NULL, " +
                "[SourceID] [varchar](50) NULL) ";


            var foreignKeys = new List<FKey>();
            foreignKeys.Add(new FKey { KeyName = "FK_Trunk_Switch", TableName = "dbo.Trunk" });

            table.foreignKeys = foreignKeys;


            var primaryKey = new PKey { Fields = new List<string> { "ID" }, KeyName = "PK_Switch" };
            table.primaryKey = primaryKey;

            return table;
        }

    }
}
