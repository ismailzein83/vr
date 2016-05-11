using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class MigrationManager
    {
        const string _Temp = "_Temp";
        List<DBTable> _dbTables = new List<DBTable>();
        ServerConnection serverConnection = null;
        Server server;

        public MigrationManager()
        {
            serverConnection = new ServerConnection("192.168.110.185", "development", "dev!123");
            server = new Server(serverConnection);
            _dbTables.Add(new DBTable { Name = "CurrencyExchangeRate", Schema = "Common", Database = "TOneConfiguration_Migration" });
            _dbTables.Add(new DBTable { Name = "Currency", Schema = "Common", Database = "TOneConfiguration_Migration" });
        }


        public bool ExecuteMigrationPhase1()
        {
            bool Executed = false;
            try
            {
                serverConnection.BeginTransaction();
                DefineTables();
                CreateTempTables();
                serverConnection.CommitTransaction();
                Executed = true;
            }
            catch
            {
                serverConnection.RollBackTransaction();
            }
            return Executed;
        }

        public bool ExecuteMigrationPhase2()
        {
            bool Executed = false;
            try
            {
                serverConnection.BeginTransaction();
                DropOriginalTables();
                serverConnection.CommitTransaction();
                Executed = true;
            }
            catch
            {
                serverConnection.RollBackTransaction();
            }
            return Executed;
        }

        public bool ExecuteMigrationPhase3()
        {
            bool Executed = false;
            try
            {
                serverConnection.BeginTransaction();
                RenameTempTables();
                CreateIndexes();
                CreateForeignKeys();
                server.Refresh();
                serverConnection.CommitTransaction();
                Executed = true;
            }
            catch
            {
                serverConnection.RollBackTransaction();
            }
            return Executed;
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

        private void CreateForeignKeys()
        {
            //Create Foreign Keys for Tables
            foreach (DBTable table in _dbTables)
                server.ConnectionContext.ExecuteNonQuery(table.ScriptedFKs);
        }

        private void CreateIndexes()
        {
            //Create Indexes Keys for Tables
            foreach (DBTable table in _dbTables)
                server.ConnectionContext.ExecuteNonQuery(table.ScriptedIndexes);
        }

        private void RenameTempTables()
        {
            foreach (DBTable dbTable in _dbTables)
            {
                DBTable dbTempTable = new DBTable();
                dbTempTable.Name = dbTable.Name + _Temp;
                dbTempTable.Schema = dbTable.Schema;
                dbTempTable.Database = dbTable.Database;

                server.Refresh();
                //server.Databases[dbTempTable.Database].Refresh();
                //server.Databases[dbTempTable.Database].Tables.Refresh();

                Table tempTable = server.Databases[dbTempTable.Database].Tables[dbTempTable.Name, dbTempTable.Schema];
                tempTable.Rename(tempTable.Name.Replace(_Temp, ""));
            }
        }

        public void CreateTempTables()
        {
            // Create Temp Tables
            foreach (DBTable dbTable in _dbTables)
            {
                Database database = server.Databases[dbTable.Database];
                bool tableExists = database.Tables.Contains(dbTable.Name + _Temp, dbTable.Schema);
                if (tableExists)
                {
                    database.Tables[dbTable.Name + _Temp, dbTable.Schema].Drop();
                }
                server.ConnectionContext.ExecuteNonQuery(dbTable.ScriptedTempTable);
            }
        }

        private void DefineTables()
        {
            foreach (DBTable table in _dbTables)
            {
                Table sourceTable = GetTableReference(table);
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

                    foreach (DBTable otherTable in _dbTables.Where(x => x.Name != table.Name && !x.DroppedOriginal))
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
                DropTables();
            }
        }

        private Table GetTableReference(DBTable dbTable)
        {
            Table sourceTable = server.Databases[dbTable.Database].Tables[dbTable.Name, dbTable.Schema];
            return sourceTable;
        }
    }
}
