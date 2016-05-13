using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class MigrationManager : BaseSQLDataManager
    {
        
        List<DBTable> _DBTables = new List<DBTable>();
        ServerConnection _ServerConnection = null;
        Server _Server;

        public class MigrationCredentials
        {
            public string MigrationServer { get; set; }
            public string MigrationServerUserID { get; set; }
            public string MigrationServerPassword { get; set; }
        }

        #region Public Methods
        public MigrationManager(MigrationCredentials migrationCredentials, List<DBTable> dbTables)
        {
            _ServerConnection = new ServerConnection(migrationCredentials.MigrationServer, migrationCredentials.MigrationServerUserID, migrationCredentials.MigrationServerPassword);
            _Server = new Server(_ServerConnection);
            _DBTables = dbTables;
        }

        public bool PrepareBeforeApplyingRecords()
        {
            bool Executed = false;
            try
            {
                _ServerConnection.BeginTransaction();
                DefineTables();
                CreateTempTables();
                _ServerConnection.CommitTransaction();
                Executed = true;
            }
            catch (Exception ex)
            {
                _ServerConnection.RollBackTransaction();
                throw ex;
            }
            return Executed;
        }

        public bool FinalizeMigration()
        {
            bool Executed = false;
            try
            {
                _ServerConnection.BeginTransaction();
                DropOriginalTables();
                RenameTempTables();
                CreateIndexes();
                CreateForeignKeys();
                _ServerConnection.CommitTransaction();
                Executed = true;
            }
            catch(Exception ex)
            {
                _ServerConnection.RollBackTransaction();
                throw ex;
            }
            return Executed;
        }
        #endregion

        #region Private Methods
        private string ScriptIndexes(Table sourceTable)
        {
            string script = string.Empty;
            if (sourceTable.HasIndex)
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
            if (sourceTable.EnumForeignKeys().Rows.Count > 0)
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
            script = script.Replace("[" + sourceTable.Schema + "].[" + sourceTable.Name + "]", "[" + sourceTable.Schema + "].[" + sourceTable.Name + Constants._Temp + "]");
            return script;
        }

        private void DropOriginalTables()
        {
            //Set Priority by References
            foreach (DBTable table in _DBTables)
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
            foreach (DBTable table in _DBTables)
                if (table.ScriptedFKs != "")
                    _Server.Databases[table.Database].ExecuteNonQuery(table.ScriptedFKs);
        }

        private void CreateIndexes()
        {
            //Create Indexes Keys for Tables
            foreach (DBTable table in _DBTables)
                if (table.ScriptedIndexes != "")
                    _Server.Databases[table.Database].ExecuteNonQuery(table.ScriptedIndexes);
        }

        private void RenameTempTables()
        {
            foreach (DBTable dbTable in _DBTables)
            {
                DBTable dbTempTable = new DBTable();
                dbTempTable.Name = dbTable.Name + Constants._Temp;
                dbTempTable.Schema = dbTable.Schema;
                dbTempTable.Database = dbTable.Database;
                _Server.Databases[dbTempTable.Database].Tables.Refresh();
                Table tempTable = _Server.Databases[dbTempTable.Database].Tables[dbTempTable.Name, dbTempTable.Schema];
                tempTable.Rename(tempTable.Name.Replace(Constants._Temp, ""));
            }
        }

        public void CreateTempTables()
        {
            // Create Temp Tables
            foreach (DBTable dbTable in _DBTables)
            {
                Database database = _Server.Databases[dbTable.Database];
                bool tableExists = database.Tables.Contains(dbTable.Name + Constants._Temp, dbTable.Schema);
                if (tableExists)
                {
                    database.Tables[dbTable.Name + Constants._Temp, dbTable.Schema].Drop();
                }

                database.ExecuteNonQuery(dbTable.ScriptedTempTable);
            }
        }

        private void DefineTables()
        {
            foreach (DBTable table in _DBTables)
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
            bool hasUnDropped = _DBTables.Exists(x => x.DroppedOriginal == false);
            if (hasUnDropped)
            {
                // Drop Original Tables
                foreach (DBTable table in _DBTables.Where(x => x.DroppedOriginal == false))
                {
                    bool isReferenced = false;

                    foreach (DBTable otherTable in _DBTables.Where(x => x.Name != table.Name && !x.DroppedOriginal))
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
            Table sourceTable = _Server.Databases[dbTable.Database].Tables[dbTable.Name, dbTable.Schema];
            return sourceTable;
        }
        #endregion
    }
}
