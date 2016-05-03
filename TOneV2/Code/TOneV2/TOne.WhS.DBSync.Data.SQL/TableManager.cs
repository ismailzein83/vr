using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class TableManager : BaseSQLDataManager
    {
        public void DropandCreateTempTable(Table table)
        {
            DropTable(table); // Drop Temp Table

            CreateTable(table);// Create Temp Table
        }

        public void CreateTable(Table table)
        {
            ExecuteNonQueryText(table.CreateTableQuery, (cmd) => { });
        }

        public void DropFK(TableKey fK)
        {
            string dropFKIfExists =
               "IF object_id('" + fK.KeyName + "') is not null " +
               "Begin " +
               "Alter table " + fK.TableName + " DROP CONSTRAINT " + fK.KeyName + " " +
               "End ";
            ExecuteNonQueryText(dropFKIfExists, (cmd) => { });
        }

        public void RenameTablefromTemp(Table table)
        {
            string renameTable =
                "sp_rename '" + table.TempName + "' , '" + table.NamewithoutSchema + "'";
            ExecuteNonQueryText(renameTable, (cmd) => { });
        }

        public void DropTable(Table table)
        {
            foreach (var fK in table.foreignKeys)
                DropFK(fK);

            string dropTempTableIfExists =
                "IF object_id('" + table.Name + "') is not null " +
                "Begin " +
                "drop table " + table.Name + " " +
                "End ";
            ExecuteNonQueryText(dropTempTableIfExists, (cmd) => { });
        }

    }
}
