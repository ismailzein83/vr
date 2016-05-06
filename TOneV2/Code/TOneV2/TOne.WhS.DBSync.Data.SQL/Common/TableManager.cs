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
            DropTempTable(); // Drop Temp Table

            CreateTable();// Create Temp Table
        }

        private void CreateTable()
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

            CreatePK();
        }

        public void DropTable()
        {
            if (_table.relatedForeignKeys != null)
                foreach (var fK in _table.relatedForeignKeys)
                    DropFK(fK);

            string dropTableIfExists =
                "IF object_id('" + _table.Name + "') is not null " +
                "Begin " +
                "drop table " + _table.Name + " " +
                "End ";
            ExecuteNonQueryText(dropTableIfExists, (cmd) => { });
        }

        private void DropTempTable()
        {
            string dropTempTableIfExists =
                "IF object_id('" + _table.TempName + "') is not null " +
                "Begin " +
                "drop table " + _table.TempName + " " +
                "End ";
            ExecuteNonQueryText(dropTempTableIfExists, (cmd) => { });
        }


        private void CreatePK()
        {
            if (_table.primaryKey != null)
            {
                string dropFKIfExists =
                 "IF object_id('" + _table.primaryKey.KeyName + "') is null " +
                 "Begin " +
                 "Alter table " + _table.Name + " ADD CONSTRAINT " + _table.primaryKey.KeyName + " PRIMARY KEY CLUSTERED (" + String.Join(",", _table.primaryKey.Fields) + ") " +
                 "End ";
                ExecuteNonQueryText(dropFKIfExists, (cmd) => { });
            }
        }

    }
}
