using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class TableManager : BaseSQLDataManager
    {
        Table _table;
        public TableManager(Table table) :
            base(GetConnectionStringName("TOneWhS_BE_MigrationDBConnStringKey", "TOneV2MigrationDBConnString"))
        {
            _table = table;
        }

        public void DropandCreateTempTable()
        {
            DropTable(); // Drop Temp Table

            CreateTable();// Create Temp Table
        }

        public void CreateTable()
        {
            ExecuteNonQueryText(_table.CreateTableQuery, (cmd) => { });
            

        }

        private void DropFK(FKey fK)
        {
            string dropFKIfExists =
               "IF object_id('" + fK.KeyName + "') is not null " +
               "Begin " +
               "Alter table " + fK.TableName + " DROP CONSTRAINT " + fK.KeyName + " " +
               "End ";
            ExecuteNonQueryText(dropFKIfExists, (cmd) => { });
        }

        public void RenameTablefromTempandRestorePK()
        {
            string renameTable =
                "sp_rename '" + _table.TempName + "' , '" + _table.NamewithoutSchema + "'";
            ExecuteNonQueryText(renameTable, (cmd) => { });

            CreatePK(_table.primaryKey);
        }

        public void DropTable()
        {
            foreach (var fK in _table.foreignKeys)
                DropFK(fK);

            string dropTempTableIfExists =
                "IF object_id('" + _table.Name + "') is not null " +
                "Begin " +
                "drop table " + _table.Name + " " +
                "End ";
            ExecuteNonQueryText(dropTempTableIfExists, (cmd) => { });
        }

        private void CreatePK(PKey pK)
        {
            string dropFKIfExists =
               "IF object_id('" + pK.KeyName + "') is null " +
               "Begin " +
               "Alter table " + _table.Name + " ADD CONSTRAINT " + pK.KeyName + " PRIMARY KEY CLUSTERED (" + String.Join(",", pK.Fields) + ") " +
               "End ";
            ExecuteNonQueryText(dropFKIfExists, (cmd) => { });
        }

    }
}
