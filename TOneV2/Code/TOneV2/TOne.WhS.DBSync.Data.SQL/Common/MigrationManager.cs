using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using TOne.WhS.DBSync.Entities;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class MigrationManager
    {
        const string _Temp = "_Temp";
        List<DBTable> _dbTables = new List<DBTable>();

        public MigrationManager()
        {
            _dbTables.Add(new DBTable { Name = "CurrencyExchangeRate", Schema = "Common", Database = "TOneV2_Migration" });
            _dbTables.Add(new DBTable { Name = "Currency", Schema = "Common", Database = "TOneV2_Migration" });
            _dbTables.Add(new DBTable { Name = "CurrencyExchangeRateZone", Schema = "Common", Database = "TOneV2_Migration" });

        }

        public void ExecuteMigration()
        {
            string connString = "server=192.168.110.185;database=TOneV2_Migration;uid=development;password=dev!123;";

            ServerConnection serverConnection = ExecuteMigrationPhase1(connString);

            serverConnection = ExecuteMigrationPhase2(connString, serverConnection);
        }

        private ServerConnection ExecuteMigrationPhase2(string connString, ServerConnection serverConnection)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                try
                {
                    serverConnection = new ServerConnection(sqlConnection);
                    Server server = new Server(serverConnection);
                    serverConnection.BeginTransaction();

                    RenameTempTables(server);
                    CreateIndexes(sqlConnection);
                    CreateForeignKeys(sqlConnection);

                    serverConnection.CommitTransaction();
                }
                catch (Exception ex)
                {
                    serverConnection.RollBackTransaction();
                    throw ex;
                }
            }
            return serverConnection;
        }

        private ServerConnection ExecuteMigrationPhase1(string connString)
        {
            ServerConnection serverConnection = null;
            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                try
                {
                    serverConnection = new ServerConnection(sqlConnection);
                    Server server = new Server(serverConnection);
                    serverConnection.BeginTransaction();

                    DefineTables(server);
                    CreateTempTables(sqlConnection);
                    FillTables();
                    DropOriginalTables();
                    serverConnection.CommitTransaction();
                }
                catch (Exception ex)
                {
                    serverConnection.RollBackTransaction();
                    throw ex;
                }
            }
            return serverConnection;
        }

        private string ScriptIndexes(Table sourceTable)
        {
            string script = string.Empty;

            foreach (Index index in sourceTable.Indexes)
            {
                StringCollection sc = index.Script();
                string[] strings = new string[sc.Count];
                sc.CopyTo(strings, 0);
                script = script + string.Join(" ", strings);
            }
            return script;
        }

        private string ScriptFKs(Table sourceTable)
        {
            string script = string.Empty;

            foreach (ForeignKey fk in sourceTable.ForeignKeys)
            {
                StringCollection sc = fk.Script();
                string[] strings = new string[sc.Count];
                sc.CopyTo(strings, 0);
                script = script + string.Join(" ", strings);
            }
            return script;
        }

        private string ScriptTempTable(Table sourceTable)
        {
            ScriptingOptions so = new ScriptingOptions();
            so.DriAllKeys = false;
            string script = string.Empty;
            StringCollection sc = sourceTable.Script(so);
            string[] strings = new string[sc.Count];
            sc.CopyTo(strings, 0);
            script = script + string.Join(" ", strings);
            script = script.Replace("[" + sourceTable.Schema + "].[" + sourceTable.Name + "]", "[" + sourceTable.Schema + "].[" + sourceTable.Name + _Temp + "]");
            return script;
        }

        private void DropOriginalTables()
        {
            //Set Priority by References
            foreach (DBTable table in _dbTables)
            {
                table.DBFKs = new List<DBForeignKey>();
                foreach (ForeignKey fk in table.Info.ForeignKeys)
                {
                    table.DBFKs.Add(new DBForeignKey { ReferencedKey = fk.ReferencedKey, ReferencedTable = fk.ReferencedTable, ReferencedTableSchema = fk.ReferencedTableSchema });
                }
            }
            DropTables();
        }

        private void CreateForeignKeys(SqlConnection sqlConnection)
        {
            //Create Foreign Keys for Tables
            foreach (DBTable table in _dbTables)
                ExecuteSql.ExecuteImmediate(table.ScriptedFKs, sqlConnection);
        }

        private void CreateIndexes(SqlConnection sqlConnection)
        {
            //Create Indexes Keys for Tables
            foreach (DBTable table in _dbTables)
                ExecuteSql.ExecuteImmediate(table.ScriptedIndexes, sqlConnection);
        }

        private void RenameTempTables(Server server)
        {
            foreach (DBTable dbTable in _dbTables)
            {
                DBTable dbTempTable = new DBTable();
                dbTempTable.Name = dbTable.Name + _Temp;
                dbTempTable.Schema = dbTable.Schema;
                dbTempTable.Database = dbTable.Database;

                Table tempTable = server.Databases[dbTempTable.Database].Tables[dbTempTable.Name, dbTempTable.Schema];
                tempTable.Rename(tempTable.Name.Replace(_Temp, ""));
            }
        }

        private void FillTables()
        {
            //Fill Temp Tables
            foreach (DBTable table in _dbTables)
            {

            }
        }

        public void CreateTempTables(SqlConnection sqlConnection)
        {
            // Create Temp Tables
            foreach (DBTable table in _dbTables)
            {
                ExecuteSql.ExecuteImmediate(table.ScriptedTempTable, sqlConnection);
            }
        }

        private void DefineTables(Server server)
        {
            foreach (DBTable table in _dbTables)
            {
                Table sourceTable = GetTableReference(server, table);
                table.Info = sourceTable;
                table.ScriptedIndexes = ScriptIndexes(sourceTable);
                table.ScriptedFKs = ScriptFKs(sourceTable);
                table.ScriptedTempTable = ScriptTempTable(sourceTable);
            }
        }

        private void DropTables()
        {
            bool hasUnDropped = _dbTables.Exists(x => x.DroppedOriginal == false);
            if (hasUnDropped)
            {
                // Drop Original Tables
                foreach (DBTable table in _dbTables.Where(x => x.DroppedOriginal == false))
                {
                    bool isReferenced = false;

                    foreach (DBTable otherTable in _dbTables.Where(x => x.Name != table.Name))
                    {
                        if (otherTable.DBFKs.Exists(x => x.ReferencedTable == table.Name && x.ReferencedTableSchema == table.Schema))
                        {
                            isReferenced = true;
                            break;
                        }
                    }


                    if (!isReferenced)
                    {
                        table.Info.Drop();
                        table.DroppedOriginal = true;
                    }
                }
            }
        }

        private Table GetTableReference(Server server, DBTable dbTable)
        {
            Table sourceTable = server.Databases[dbTable.Database].Tables[dbTable.Name, dbTable.Schema];
            return sourceTable;
        }
    }
}
