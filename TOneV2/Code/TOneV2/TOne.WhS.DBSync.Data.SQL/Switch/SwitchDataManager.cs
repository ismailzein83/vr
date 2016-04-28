using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SwitchDataManager : BaseSQLDataManager, ISwitchDataManager
    {
        readonly string[] columns = { "Name", "SourceID" };

        public SwitchDataManager() :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneV2DBConnString"))
        {

        }

        public void MigrateSwitchesToDB(List<Switch> switches)
        {
            Table table = DefineTable();

            TableManager tableManager = new TableManager();

            tableManager.DropandCreateTempTable(table); // Drop and Create Temp

            ApplySwitchesToDB(switches, table); // Apply to Temp

            tableManager.DropTable(table); // Drop Table

            tableManager.RenameTablefromTemp(table); // Rename Temp table to be Table name
        }


        private void ApplySwitchesToDB(List<Switch> switches, Table table)
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
                TableName = table.TempName,
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
            table.NamewithoutSchema = "WalidSwitch";
            table.CreateTableQuery =
                "CREATE TABLE " + table.TempName + "( " +
                "[ID] [int] IDENTITY(1,1) NOT NULL, " +
                "[Name] [varchar](50) NULL, " +
                "[timestamp] [timestamp] NULL, " +
                "[SourceID] [varchar](50) NULL) ";


            var foreignKeys = new List<TableKey>();
            foreignKeys.Add(new TableKey { KeyName = "FK_Trunk_Switch", TableName = "dbo.Trunk" });

            table.foreignKeys = foreignKeys;
            return table;
        }

    }
}
